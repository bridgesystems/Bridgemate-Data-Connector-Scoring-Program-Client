using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BridgeSystems.Bridgemate.DataConnectorClasses.SharedDTO
{

    /// <summary>
    /// Carries response data for a previously sent BCSManagementRequestDTO.
    /// </summary>
    public class BCSManagementResponseDTO
    {

        /// <summary>
        /// The guid of the event currently being administered. Can be empty if no sessions are active.
        /// </summary>
        public string EventGuid
        {
            get; set;
        }

        /// <summary>
        /// Either information on the sessions currently being administered, or information on all session known to BCS.
        /// </summary>
        public SessionInfoDTO[] SessionInformation
        {
            get; set;
        }

        /// <summary>
        /// True if BCS is running.
        /// </summary>
        public bool IsRunning
        {
            get; set;
        }

        /// <summary>
        /// The location of the scoring file. Can be used for back-up purposes.
        /// </summary>
        public string ScoringFilePath
        {
            get; set;
        }

        /// <summary>
        /// The location of the Data Connector messages file. Can be used for back-up purposes.
        /// </summary>
        public string DataConnectorFilePath { get; set; }

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
            if (EventGuid != null && (EventGuid.Length != 32 || EventGuid.Any(c => !(c >= 'A' && c <= 'F' || c >= '0' && c <= '9'))))
            {
                validationMessages.Add("The event guid must be exactly 32 character long and can only contain capital A to F or digits 0 to 9.");
            }

            if (SessionInformation != null && SessionInformation.Any())
            {
                foreach (SessionInfoDTO sessionInfo in SessionInformation)
                {
                    if (sessionInfo.SessionGuid != null && (sessionInfo.SessionGuid.Length != 32 || sessionInfo.SessionGuid.Any(c => !(c >= 'A' && c <= 'F' || c >= '0' && c <= '9'))))
                    {
                        validationMessages.Add("The session guid must be exactly 32 character long and can only contain capital A to F or digits 0 to 9.");
                    }
                }
            }

            ValidationMessages = validationMessages.ToArray();
            return validationMessages.Any();
        }
    }
}
