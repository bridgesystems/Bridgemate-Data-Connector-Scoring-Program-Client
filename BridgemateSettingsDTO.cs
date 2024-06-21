using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BridgeSystems.Bridgemate.DataConnectorClasses.SharedDTO
{
    /// <summary>
    /// The base class for the <see cref="Bridgemate2SettingsDTO">Bridgemate2SettingsDTO</see> and 
    /// the <see cref="Bridgemate3SettingsDTO">Bridgemate3SettingsDTO</see>
    /// </summary>
    public abstract class BridgemateSettingsDTO
    {
        /// <summary>
        /// Required. Specifies the guid of the session for which the settings apply.
        /// </summary>
        public string SessionGuid
        {
            get; set;
        }

        /// <summary>
        /// Required. Specifies for which section the settings apply.
        /// </summary>
        public string SectionLetters
        {
            get; set;
        }

        /// <summary>
        /// Validates the DTO. Produces validation messages if there are problems. 
        /// </summary>
        /// <returns>True if there are no validation errors.</returns>
        public virtual bool Validate()
        {
            var validationErrors = new List<string>();
            if (SessionGuid == null || SessionGuid.Length != 32 || SessionGuid.Any(c => !(c >= 'A' && c <= 'F' || c >= '0' && c <= '9')))
            {
                validationErrors.Add("The guid must be exactly 32 character long and can only contain capital A to F or digits 0 to 9.");
            }
            if (!Regex.IsMatch(SectionLetters ?? "", @"^([A-Z])\1{0,2}$"))
            {
                validationErrors.Add($"Invalid {nameof(SectionLetters)} ({SectionLetters}). Valid values are: 'A-Z', 'AA-ZZ' or 'AAA','ZZZ'");
            }
            ValidationMessages = validationErrors.ToArray();
            return !validationErrors.Any();

        }
        /// <summary>
        /// An array of messages describing problems in data integrity and invalid values.
        /// </summary>
        public virtual string[] ValidationMessages
        {
            get;set;
        }
    }
}

