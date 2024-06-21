using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BridgeSystems.Bridgemate.DataConnectorClasses.SharedDTO
{
    /// <summary>
    /// Contains information on how to construct a new event, with its sessions, sections, tables and rounds.
    /// Optionally player data, participations and handrecords can be added. Doing this more performant than adding them later.
    /// </summary>
    public class InitDTO
    {
        /// <summary>
        /// Instructs the Data Connector to start BCS.
        /// </summary>
        public const int StartBCS = 1;

        /// <summary>
        /// Instructs BCS to create a new event from the data in this DTO.
        /// </summary>
        public const int Command_Reset = 2;

        /// <summary>
        /// Instructs BCS to start reading from the Data Connector.
        /// </summary>
        public const int Command_StartReading = 4;

        /// <summary>
        /// Instructs BCS to upload the sessions to the Bridgemate App server.
        /// </summary>
        public const int Command_ShowInApp = 8;

        /// <summary>
        /// Instructs BCS to start minimized.
        /// </summary>
        public const int Command_Minimize = 16;

        /// <summary>
        /// Instructs BCS to shut down after the last result has been processed.
        /// </summary>
        public const int Command_AutoShutDownBPC = 32;

        /// <summary>
        /// Sets the log level to "Debug" rather than "Info".
        /// </summary>
        public const int Command_LogLevel_Debug = 64;

        /// <summary>
        /// Instructs the Data Connector to clear all its previous data and start with a clean slate.
        /// </summary>
        public const int Command_ClearData = 128;

        /// <summary>
        /// Instructions to BCS. Add up the values to combine.<br/>
        /// 1. Start BCS (if not already started).<br/>
        /// 2: Reset the Bridgemates (and App) and fill them with the provided data.<br/>
        /// 4: Start Reading from the Bridgemates, Data Connector (and App).<br/>
        /// 8: Upload the sessions to the app.<br/>
        /// 16: Minimize BCS<br/>
        /// 32:Close BCS when all results have been processed.<br/>
        /// 64: Lower the logging level from "Info" to "Debug".<br/>
        /// 128:Clear all previous data from the Data Connector queue.
        /// </summary>
        public int Commands
        {
            get; set;
        }

        /// <summary>
        /// Required if the number of sessions is greater than one. This signals that BCS should administer these sessions together.
        /// </summary>
        public string EventGuid
        {
            get; set;
        }

        /// <summary>
        /// Optional. Specifies a different directory for the BCS scoring file. Only use in advanced scenarios.
        /// If used make sure that the directory exists.
        /// </summary>
        public string AlternativeDataFolder
        {
            get;set;
        }

        /// <summary>
        /// Required, must be at least one.
        /// </summary>
        public SessionDTO[] Sessions
        {
            get; set;
        }

        /// <summary>
        /// Optional: the data of at least the players that will participate in the sessions.
        /// Player data is required for every participation that specifies an OrganizationId-PlayerNumber identification.
        /// </summary>
        public PlayerDataDTO[] PlayerData
        {
            get; set;
        }

        /// <summary>
        /// Optional. Each participation with an OrganizationId-PlayerNumber must have a corresponding
        /// PlayerData element that specifies at least its name.
        /// </summary>
        public ParticipationDTO[] Participations
        {
            get; set;
        }

        /// <summary>
        /// Optional. The handrecords for each session.
        /// </summary>
        public HandrecordDTO[] Handrecords
        {
            get; set;
        }

        /// <summary>
        /// Optional. The settings for the Bridgemate 2 for all sections (if in use).
        /// </summary>
        public Bridgemate2SettingsDTO[] Bridgemate2Settings
        {
            get; set;
        }

        /// <summary>
        /// Optional. The settings for the Bridgemate 3 for all sections (if in use).
        /// </summary>
        public Bridgemate3SettingsDTO[] Bridgemate3Settings
        {
            get; set;
        }

        /// <summary>
        /// An array of messages describing problems in data integrity and invalid values.
        /// </summary>
        public string[] ValidationMessages
        {
            get;set;
        }

        /// <summary>
        /// Validates the DTO. Produces validation messages if there are problems. 
        /// </summary>
        /// <returns>True if there are no validation errors.</returns>
        public bool Validate()
        {
            var validationMessages=new List<string>();

            if (Commands < 0 || Commands > 255)
            {
                validationMessages.Add($"The Commands ({Commands}) must be between 0 and 63.");
            }
            if (!string.IsNullOrWhiteSpace(AlternativeDataFolder))
            {
                if (!Directory.Exists(AlternativeDataFolder)) 
                {
                    validationMessages.Add($"The specified alternative data folder ('{AlternativeDataFolder}' does not exist.)");
                }
            }
            if (Sessions == null || Sessions.Length == 0)
            {
                validationMessages.Add("At least one session is required.");
            }

            if (Sessions.Length > 1)
            {
                if (string.IsNullOrWhiteSpace(EventGuid))
                {
                    validationMessages.Add($"If there is more than one session the {nameof(EventGuid)} must be specified and it must be identical " +
                        $"to the sessions' {nameof(SessionDTO.EventGuid)}.");
                }
                else
                {
                    foreach (var session in Sessions)
                    {
                        if (session.EventGuid != EventGuid)
                            validationMessages.Add($"The {nameof(SessionDTO.EventGuid)} ('{session.EventGuid}') of session " +
                                $"'{session.Name}' ({session.SessionGuid}) are not the same.");
                    }
                }

            }
            if (EventGuid != null && (EventGuid.Length != 32 || EventGuid.Any(c => !(c >= 'A' && c <= 'F' || c >= '0' && c <= '9'))))
            {
                validationMessages.Add("The event guid, if used, must be exactly 32 character long and can only contain capital A to F or digits 0 to 9.");
            }

            if (PlayerData.Any())
            {
                var sessionGuids = Sessions.Select(s => s.SessionGuid).ToArray();
                foreach (var data in PlayerData)
                {
                    if (!data.Validate())
                    {
                        var errorMessage = string.Join(", ", data.ValidationMessages);
                        validationMessages.Add($"{nameof(PlayerData)} '{data.FirstName} {data.LastName} ({data.SessionGuid}-{data.PlayerNumber})': " +
                                               $"{errorMessage} ");
                    }
                    if (!sessionGuids.Contains(data.SessionGuid))
                    {
                        validationMessages.Add($"{nameof(PlayerData)}.{nameof(PlayerDataDTO.SessionGuid)} ('{data.SessionGuid}') " +
                            $"must be one of the sessions' guids ({string.Join(", ", sessionGuids)}).");
                    }
                }
                var ids = PlayerData.GroupBy(data => data.PlayerNumber ?? "");
                foreach (var id in ids.Where(id => id.Count() > 1))
                {
                    validationMessages.Add($"Duplicate ({id.Count()}) entries for player data " +
                                           $"'{id.First().FirstName}+{id.First().LastName}'");
                }
            }
            if (Participations.Any())
            {
                foreach (var participation in Participations)
                {
                    if (!participation.Validate())
                    {
                        var errorMessage = string.Join(", ", participation.ValidationMessages);
                        validationMessages.Add($"{nameof(ParticipationDTO)}  '{participation.SessionGuid}-{participation.PlayerNumber}': " +
                                               $"{errorMessage} ");
                    }
                }
                foreach (var participation in Participations)
                {
                    var id = participation.SessionGuid ?? "" + participation.PlayerNumber ?? "";
                    if (string.IsNullOrEmpty(id) || id == (participation.SessionGuid ?? ""))
                        continue;
                    if (PlayerData.Any(data => (data.SessionGuid ?? "" + data.PlayerNumber ?? "") == id))
                        continue;
                    validationMessages.Add($"{nameof(ParticipationDTO)} '{participation.SessionGuid}-{participation.PlayerNumber}' " +
                                           $"has no corresponding {nameof(PlayerDataDTO)}");
                }
            }
            if (Handrecords.Any())
            {
                foreach (var handrecord in Handrecords)
                {
                    if (!handrecord.Validate())
                    {
                        var errorMessage = string.Join(", ", handrecord.ValidationMessages);
                        validationMessages.Add($"{nameof(HandrecordDTO)}  '{handrecord.SectionLetters}-{handrecord.BoardNumber}': " +
                                               $"{errorMessage} ");
                    }
                }
            }
            if (Bridgemate2Settings != null && Bridgemate2Settings.Any())
            {
                var settingsErrors = false;
                foreach (var settings in Bridgemate2Settings)
                {
                    if (!settings.Validate())
                    {
                        settingsErrors = true;
                        var errorMessage = string.Join(", ", settings.ValidationMessages);
                        validationMessages.Add($"{nameof(Bridgemate2SettingsDTO)}  '{settings.SectionLetters}': " +
                                               $"{errorMessage} ");
                    }
                }
                if (!settingsErrors)
                {
                    var sectionLettersGroups = Bridgemate2Settings.GroupBy(settings => settings.SectionLetters).ToList();
                    foreach (var group in sectionLettersGroups.Where(g => g.Count() > 1))
                        validationMessages.Add($"Duplicate ({group.Count()}) settings for section '{group.Key}'");
                }
            }
            if (Bridgemate3Settings != null && Bridgemate3Settings.Any())
            {
                foreach (var settings in Bridgemate3Settings)
                {
                    if (!settings.Validate())
                    {
                        var errorMessage = string.Join(", ", settings.ValidationMessages);
                        validationMessages.Add($"{nameof(Bridgemate3SettingsDTO)}  '{settings.SectionLetters}': " +
                                               $"{errorMessage} ");
                    }
                }
            }

            ValidationMessages=validationMessages.ToArray();
            return !ValidationMessages.Any();
        }
    }
}
