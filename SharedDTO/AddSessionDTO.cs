using System.Collections.Generic;
using System.Linq;

namespace BridgeSystems.Bridgemate.DataConnectorClasses.SharedDTO
{
    /// <summary>
    /// Use this DTO to add a session to an existing event.
    /// </summary>
    public class AddSessionDTO
    {
        /// <summary>
        /// Initializes the DTO.
        /// </summary>
        public AddSessionDTO()
        {
            ClubId = string.Empty;
        }

        /// <summary>
        /// Required. The guid of the event to add this session to.
        /// </summary>
        public string EventGuid
        {
            get; set;
        }

        /// <summary>
        /// Required when using http for communication with the data connector.
        /// </summary>
        public string ClubId
        { get; set; }


        /// <summary>
        /// Required
        /// </summary>
        public SessionDTO Session
        {
            get; set;
        }

        /// <summary>
        /// Optional: the data of at least the players that will participate in the session.
        /// <see cref="PlayerDataDTO">Player data</see> is required for every <see cref="ParticipationDTO">participation</see> that has its a SessionGuid and PlayerNumber properties set.
        /// </summary>
        public PlayerDataDTO[] PlayerData
        {
            get; set;
        }

        /// <summary>
        /// Optional. Each <see cref="ParticipationDTO">participation</see> with its SessionGuid and PlayerNumber properties set must have a corresponding
        /// <see cref="PlayerDataDTO">player data</see> element that specifies at least its name.
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
            get; set;
        }

        /// <summary>
        /// Validates the DTO. Produces validation messages if there are problems. 
        /// </summary>
        /// <returns>True if there are no validation errors.</returns>
        public bool Validate()
        {
            var validationMessages = new List<string>();

            if (Session == null)
            {
                validationMessages.Add("A session is required.");
                ValidationMessages = validationMessages.ToArray();
                return false;
            }

            if (EventGuid == null || (EventGuid.Length != 32 || EventGuid.Any(c => !(c >= 'A' && c <= 'F' || c >= '0' && c <= '9'))))
            {
                validationMessages.Add("The event guid is required and must be exactly 32 character long and can only contain capital A to F or digits 0 to 9.");
                ValidationMessages = validationMessages.ToArray();
                return false;
            }

            if (EventGuid != Session.EventGuid)
            {
                validationMessages.Add($"The event guid in the session must be equal to the {nameof(EventGuid)} property: '{EventGuid}', " +
                                       $"but it is '{Session.EventGuid}'");
            }

            if (!Session.Validate(forAdding: true))
            {
                validationMessages.AddRange(Session.ValidationMessages);
            }

            if (PlayerData != null && PlayerData.Any())
            {
                var sessionGuid = Session.SessionGuid;
                foreach (var data in PlayerData)
                {
                    if (!data.Validate())
                    {
                        var errorMessage = string.Join(", ", data.ValidationMessages);
                        validationMessages.Add($"{nameof(PlayerData)} '{data.FirstName} {data.LastName} ({data.SessionGuid}-{data.PlayerNumber})': " +
                                               $"{errorMessage} ");
                    }
                    if (sessionGuid != data.SessionGuid)
                    {
                        validationMessages.Add($"{nameof(PlayerData)}.{nameof(PlayerDataDTO.SessionGuid)} ('{data.SessionGuid}') " +
                            $"must be equal to the sessions' guids({sessionGuid}).");
                    }
                }
                var ids = PlayerData.GroupBy(data => data.PlayerNumber ?? "");
                foreach (var id in ids.Where(id => id.Count() > 1))
                {
                    validationMessages.Add($"Duplicate ({id.Count()}) entries for player data " +
                                           $"'{id.First().FirstName}+{id.First().LastName}'");
                }
            }
            if (Participations != null && Participations.Any())
            {
                if (PlayerData == null)
                {
                    validationMessages.Add($"No {nameof(PlayerDataDTO)} defined, but there are " +
                                           $"{Participations.Length} {nameof(ParticipationDTO)}s defined. " +
                                           $"Each {nameof(ParticipationDTO)} with its SessionGuid and PlayerNumber properties set " +
                                           $"must have a corresponding {nameof(PlayerDataDTO)} that specifies at least its name.");
                }
                else
                {
                    foreach (var participation in Participations)
                    {
                        if (!participation.Validate(allowPlayerNumberAndName: false))
                        {
                            var errorMessage = string.Join(", ", participation.ValidationMessages);
                            validationMessages.Add($"{nameof(ParticipationDTO)}  '{participation.SessionGuid}-{participation.PlayerNumber}': " +
                                                   $"{errorMessage} ");
                        }
                    }
                    foreach (var participation in Participations)
                    {
                        var id = (participation.SessionGuid ?? "") + (participation.PlayerNumber ?? "");
                        if (string.IsNullOrEmpty(id) || id == (participation.SessionGuid ?? ""))
                            continue;
                        if (PlayerData.Any(data => ((data.SessionGuid ?? "") + (data.PlayerNumber ?? "")) == id))
                            continue;
                        validationMessages.Add($"{nameof(ParticipationDTO)} '{participation.SessionGuid}-{participation.PlayerNumber}' " +
                                               $"has no corresponding {nameof(PlayerDataDTO)}");
                    }
                }
            }
            if (Handrecords!=null && Handrecords.Any())
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

            ValidationMessages = validationMessages.ToArray();
            return !ValidationMessages.Any();
        }
    }
}
