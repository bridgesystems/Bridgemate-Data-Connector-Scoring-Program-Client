using System;
using System.Collections.Generic;
using System.Linq;

namespace BridgeSystems.Bridgemate.DataConnectorClasses.SharedDTO
{
    /// <summary>
    /// A group of sections that are usually played at the same time and place. Pairs will subscribe to participate in a specific session.
    /// If BCS has to administer more than one session at the same time, these sessions must share a comment EventGuid property.
    /// </summary>
    public class SessionDTO
    {
        /// <summary>
        /// Required, must be at least one.
        /// </summary>
        public ScoringGroupDTO[] ScoringGroups
        {
            get; set;
        }

        /// <summary>
        /// Optional. A string built from a Guid, without the curly braces or connecting dashes.
        /// The letters must be capitals
        /// Must be unique if used.
        /// </summary>
        public string EventGuid
        {
            get; set;
        }

        /// <summary>
        /// Required.A string built from a Guid, without the curly braces or connecting dashes.
        /// The letters must be capitals
        /// Must be unique
        /// </summary>
        public string SessionGuid
        {
            get; set;
        }

        /// <summary>
        /// Required. The name of the session.
        /// </summary>
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// Required. The year in which the session is played.
        /// </summary>
        public int Year
        {
            get; set;
        }

        /// <summary>
        /// Required. The month in which the session is played.
        /// </summary>
        public int Month
        {
            get; set;
        }

        /// <summary>
        /// Required. The day on which the session is played.
        /// </summary>
        public int Day
        {
            get; set;
        }

        /// <summary>
        /// Optional. The starting hour for the session.
        /// </summary>
        public int Hour
        {
            get; set;
        }

        /// <summary>
        /// Optional. The starting minutes for the session.
        /// </summary>
        public int Minute
        {
            get; set;
        }

        /// <summary>
        /// If True will show the session in the Bridgemate App, if the Bridgemae App has been configured in BCS.
        /// </summary>
        public bool ShowInApp
        {
            get; set;
        }

        public bool EWReturnHome
        {
            get; set;
        }

        public bool PairsMoveAccrossField
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
            var validationMessages=new List<string>();

            if (SessionGuid == null || SessionGuid.Length != 32 || SessionGuid.Any(c => !(c >= 'A' && c <= 'F' || c >= '0' && c <= '9')))
            {
                validationMessages.Add("The guid must be exactly 32 character long and can only contain capital A to F or digits 0 to 9.");
            }

            if (EventGuid != null && (SessionGuid.Length != 32 || SessionGuid.Any(c => !(c >= 'A' && c <= 'F' || c >= '0' && c <= '9'))))
            {
                validationMessages.Add("The venture guid, if used, must be exactly 32 character long and can only contain capital A to F or digits 0 to 9.");
            }
            if (ScoringGroups == null || ScoringGroups.Length == 0)
            {
                validationMessages.Add("At least one scoringroup is required.");
            }

            if (ScoringGroups.Select(sg => sg.ScoringGroupNumber).Distinct().Count() != ScoringGroups.Count())
            {
                validationMessages.Add($"The scoring groups cannot have the same {nameof(ScoringGroupDTO.ScoringGroupNumber)}");
            }

            foreach (ScoringGroupDTO group in ScoringGroups)
            {
                if (group.SessionGuid != SessionGuid)
                {
                    validationMessages.Add($"Scoring group with id {group.ScoringGroupNumber} must have {nameof(ScoringGroupDTO.SessionGuid)} '{SessionGuid}' " +
                                           $"but it is '{group.SessionGuid}'");
                }
                if (!group.Validate())
                {
                    var errorMessage = string.Join("; ", group.ValidationMessages);
                    validationMessages.Add($"Scoringgroup {group.ScoringGroupNumber} has validation errrors: {errorMessage}.");
                }
            }
            var sections = ScoringGroups.SelectMany(sg => sg.Sections).ToList();
            if (sections.Any(s => s.IsCombiSection))
            {
                var combiSectionGroups = sections.Where(s => s.IsCombiSection)
                                                         .Select(s => new
                                                         {
                                                             s.Letters,
                                                             Sources = new[] { s.NorthSouthPairSectionLetters,
                                                                             s.EastWestPairSectionLetters }
                                                         })
                                                       .ToList();
                foreach (var combiGroup in combiSectionGroups)
                {
                    if (combiGroup.Sources.Distinct().Count() != 2)
                    {
                        validationMessages.Add($"The combisection '{combiGroup.Letters}' must have two different sections as its source, " +
                                               $"but they are for NS '{combiGroup.Sources[0]}' and for EW '{combiGroup.Sources[1]}'");
                    }
                    foreach (var sourceSection in combiGroup.Sources)
                    {
                        if (!sections.Any(s => s.Letters == sourceSection))
                        {
                            validationMessages.Add($"Combisection '{combiGroup.Letters}' specifies section '{sourceSection}' as one of its source sections, " +
                                                   $"but this section does not exist.");
                        }
                    }
                }
            }

            if (Name == null || Name.Length < 1)
            {
                validationMessages.Add("The name of the session is required.");
            }

            if (Year < 2000)
            {
                validationMessages.Add($"The year ({Year}) for the session must be at least 2000.");
            }

            if (Month < 1 || Month > 12)
            {
                validationMessages.Add($"The month ({Month}) for the session must be between 1 and 12.");
            }

            if (Day < 1 || Day > 31)
            {
                validationMessages.Add($"The day ({Day}) for the session must be between 1 and 31.");
            }
            if (Hour < 0 || Hour >= 24)
            {
                validationMessages.Add($"The hour ({Hour}) of the day must be between 0 and 23");
            }
            if (Minute < 0 || Minute >= 60)
            {
                validationMessages.Add($"The minute ({Minute})must be between 0 and 59");
            }
            try
            {
                DateTime date = new DateTime(Year, Month, Day);
            }
            catch
            {
                validationMessages.Add($"The date {Year}-{Month}-{Day} is invalid.");
            }
            ValidationMessages=validationMessages.ToArray();
            return !ValidationMessages.Any();
        }
    }
}
