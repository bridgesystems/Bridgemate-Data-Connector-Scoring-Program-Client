using System;
using System.Collections.Generic;
using System.Linq;

namespace BridgeSystems.Bridgemate.DataConnectorClasses.SharedDTO
{
    /// <summary>
    /// The scoring group holds information on the scoring method of its sections.
    /// A scoring group must have at least one section, though this can be omitted when using this DTO for scoring group updates.
    /// If a scoring group has more than one section the score calculation will treat these sections as if they were one.
    /// </summary>
    public class ScoringGroupDTO
    {
        /// <summary>
        /// The matchscore is expressed in matchpoints.
        /// </summary>
        public const int ScoringType_Pairs = 10;

        /// <summary>
        /// The matchscore is expressed in Butler Imps.
        /// </summary>
        public const int ScoringType_Imp = 20;

        /// <summary>
        /// The matchscore is expressed in Cross Imps.
        /// </summary>
        public const int ScoringType_XImp = 30;

        /// <summary>
        /// The matchscore is expressed in Team Imps.
        /// </summary>
        public const int ScoringType_TeamImps = 40;

        /// <summary>
        /// The matchscore is expressed in discrete (whole number) Victory Points.
        /// </summary>
        public const int ScoringType_TeamVPDiscrete = 50;

        /// <summary>
        /// The matchscore is expressed in continuous (floating point) Victory Points.
        /// </summary>
        public const int ScoringType_TeamVPContinuous = 51;

        /// <summary>
        /// The matchscore is expressed in Board-a-Match points.
        /// </summary>
        public const int ScoringType_Bam = 60;

        /// <summary>
        /// The matchscore is expressed as a Patton score.
        /// </summary>
        public const int ScoringType_Patton = 70;

        /// <summary>
        /// Required, must match the Guid of its SessionDTO parent.
        /// </summary>
        public string SessionGuid
        {
            get; set;
        }
        /// <summary>
        /// Required when the dto is part of the InitDTO, must be at least one. 
        /// Optional when the dto is used for scoringgroup updates.
        /// Must be empty if the IsDeleted property is true.
        /// </summary>
        public SectionDTO[] Sections
        {
            get; set;
        }

        /// <summary>
        /// Required, must be unique and greater than zero.
        /// </summary>
        public int ScoringGroupNumber
        {
            get; set;
        }

        /// <summary>
        /// Required.
        /// Pairs = 10,
        /// IMP = 20,
        /// XIMP = 30,
        /// TeamIMP = 40,
        /// TeamsVPDiscrete = 50,
        /// TeamsVPContinuous = 51,
        /// Bam = 60,
        /// Patton = 70
        /// </summary>
        public int ScoringMethod
        {
            get; set;
        }

        /// <summary>
        /// Optional
        /// </summary>
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// If "True" the scoring group will be deleted if it has no more sections. Be sure to assign other scoring groups to this scoringgroup's sections
        /// if its sections are not deleted. Will be ignored when the dto is part of the InitDTO.
        /// </summary>
        public bool IsDeleted
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
            if (SessionGuid == null || SessionGuid.Length != 32 || SessionGuid.Any(c => !(c >= 'A' && c <= 'F' || c >= '0' && c <= '9')))
            {
                validationMessages.Add("The guid must be exactly 32 character long and can only contain capital A to F or digits 0 to 9.");
            }
            if (ScoringGroupNumber <= 0)
            {
                validationMessages.Add($"{nameof(ScoringGroupNumber)} ({ScoringGroupNumber}) must be greater than zero.");
            }
            if (ScoringMethod < 10 || ScoringMethod > 70)
            {
                validationMessages.Add($"Invalid {nameof(ScoringMethod)} ({ScoringMethod}). The value must be between 10 and 70. ");
            }
            if (IsDeleted)
            {
                if (Sections.Any())
                {
                    validationMessages.Add($"A scoringgroup marked for deletion must not have any sections defined.");
                }
            }
            else if (!Sections.Any())
            {
                validationMessages.Add("The scoringgroup must have at least one section.");
            }
            if (Sections.Select(sg => sg.Letters).Distinct().Count() != Sections.Count())
            {
                validationMessages.Add($"The sections cannot have the same {nameof(SectionDTO.Letters)}");
            }

            foreach (SectionDTO section in Sections)
            {
                if (section.SessionGuid != SessionGuid)
                {
                    validationMessages.Add($"Section '{section.Letters}' must have {nameof(ScoringGroupDTO.SessionGuid)} '{SessionGuid}' " +
                                           $"but it is '{section.SessionGuid}'");
                }
                if (section.ScoringGroupNumber != ScoringGroupNumber)
                {
                    validationMessages.Add($"Section '{section.Letters}' must have {nameof(ScoringGroupDTO.ScoringGroupNumber)} '{ScoringGroupNumber}' " +
                                           $"but it is '{section.ScoringGroupNumber}'");
                }

                if (!section.Validate())
                {
                    var errorMessage = string.Join("; ", section.ValidationMessages);
                    validationMessages.Add($"Section '{section.Letters}' has validation errrors: {errorMessage}.");
                }
            }
            ValidationMessages = validationMessages.ToArray();
            return !ValidationMessages.Any();
        }
    }
}
