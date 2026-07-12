using System.Collections.Generic;
using System.Linq;

namespace BridgeSystems.Bridgemate.DataConnectorClasses.SharedDTO
{
    /// <summary>
    /// This class carries information on a player: name, country and playernumber.
    /// Each ParticipationDTO carrying a non empty playernumber property must have a corresponding PlayerDataDTO woith the given playernumber
    /// (and SessionGuid) that was sent before the ParticipationDTO was sent.
    /// </summary>
    public class PlayerDataDTO
    {
        /// <summary>
        /// Required, the guid of the session the player participates in.
        /// </summary>
        public string SessionGuid
        {
            get; set;
        }

        /// <summary>
        /// Required when using the http interface for the dataconnector
        /// </summary>
        public string ClubId
        { get; set; }

        /// <summary>
        /// Required, must uniquely identify the player within the session.
        /// </summary>
        public string PlayerNumber
        {
            get; set;
        }

        /// <summary>
        /// Optional
        /// </summary>
        public string FirstName
        {
            get; set;
        }

        /// <summary>
        /// Required
        /// </summary>
        public string LastName
        {
            get; set;
        }

        /// <summary>
        /// Optional
        /// </summary>
        public string CountryCode
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
           
            if (SessionGuid == null || SessionGuid.Length != 32 || SessionGuid.Any(c => !(c >= 'A' && c <= 'F' || c >= '0' && c <= '9')))
            {
                validationMessages.Add("The guid must be exactly 32 character long and can only contain capital A to F or digits 0 to 9.");
            }
            if (string.IsNullOrWhiteSpace(PlayerNumber))
            {
                validationMessages.Add($"The {nameof(PlayerNumber)} is required.");
            }
            if (string.IsNullOrWhiteSpace(LastName))
            {
                validationMessages.Add($"The {nameof(LastName)} is required.");
            }

            ValidationMessages=validationMessages.ToArray();
            return !ValidationMessages.Any();
        }

    }
}
