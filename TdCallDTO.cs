using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BridgeSystems.Bridgemate.DataConnectorClasses.SharedDTO
{
    /// <summary>
    /// Contains information about a TD call.
    /// </summary>
    public class TdCallDTO
    {
        /// <summary>
        /// The TD call status has been iniiated.
        /// </summary>
        public const int TdCallStatus_Launched = 1;

       /// <summary>
       /// The TD is at the table.
       /// </summary>
        public const int TdCallStatus_InProgress = 2;

        /// <summary>
        /// The call has been resolved.
        /// </summary>
        public const int TdCallStatus_Resolved = 3;

        /// <summary>
        /// Resolution of the call has been deferred.
        /// </summary>
        public const int TdCallStatus_Deferred = 4;
        /// <summary>
        /// Required. The guid of the session the td call belongs to.
        /// </summary>
        public string SessionGuid
        {
            get; set;
        }
        /// <summary>
        /// Required, the letters of the section the td call belongs to. The section must exist.
        /// </summary>
        public string SectionLetters
        {
            get; set;
        }

        /// <summary>
        /// Required, the table in the section where the td call originates from.
        /// </summary>
        public int TableNumber
        {
            get; set;
        }

        /// <summary>
        /// Required. Valid values are:
        /// 1: Launched
        /// 2: InProgress
        /// 3. Resolved
        /// 4: Deferred
        /// </summary>
        public int Status
        { get; set; }

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
            if (!Regex.IsMatch(SectionLetters ?? "", @"^([A-Z])\1{0,2}$"))
            {
                validationMessages.Add($"Invalid {nameof(SectionLetters)} ({SectionLetters}). Valid values are: 'A-Z', 'AA-ZZ' or 'AAA','ZZZ'");
            }
            if (TableNumber <= 0)
            {
                validationMessages.Add($"{nameof(TableNumber)} ({TableNumber}) must be greater than zero.");
            }
            if(Status <= 0 || Status>4)
            {
                validationMessages.Add($"Invalide {nameof(Status)}:({Status}). Valid values are 1,2,3 or 4.");
            }
          
            ValidationMessages = validationMessages.ToArray();
            return !ValidationMessages.Any();
        }
    }
}
