
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BridgeSystems.Bridgemate.DataConnectorClasses.SharedDTO
{
    /// <summary>
    /// This class can be used:
    /// 1. To give names for the players in the first round by only specifying their player numbers. 
    ///    For this to work the names belonging to the playernumber must have been passed first using
    ///    PlayerDTO's.
    /// 2. To give names for the players in the first round by omitting their playernumber. In this case the
    ///    players will be registerd for this session only.
    /// The combination of both playernumber and name details is not supported.
    /// </summary>
    public class ParticipationDTO
    {
        /// <summary>
        /// Required. The guid of the session the participation belongs to.
        /// A string built from a Guid, without the curly braces or connecting dashes.
        /// The letters must be capitals
        /// </summary>
        public string SessionGuid
        {
            get; set;
        }
        /// <summary>
        /// Required. The letters of the section the participation belongs to.
        /// Can be A-Z, AA, BB, CC,...ZZ, AAA, BBB, CCC, ... ZZZ
        /// </summa
        public string SectionLetters
        {
            get; set;
        }

        /// <summary>
        /// Required. The number of the table for the player in the round.
        /// </summary>
        public int TableNumber
        {
            get; set;
        }

        /// <summary>
        /// Required. The position on the table for the player in the round.
        /// 1: North
        /// 2: East
        /// 3: South
        /// 4: West
        /// </summary>
        public TableDirection Direction
        {
            get; set;
        }

        /// <summary>
        /// Currently only the values zero and one are supported. They are treated the same as the table where the player sits in the first round.
        /// Optional, specifying a number higher than 1 indicates that the name of the player has become known
        /// in a latter round. BCS will use the known movement to calculate the player's positions in other rounds.
        /// </summary>
        public int RoundNumber
        {
            get; set;
        }

        /// <summary>
        /// Optional, but must be present together with the SessionGuid if no name details are passed.
        /// The playernumber must have been passed beforehand using a PlayerDTO containg the name details.
        /// </summary>
        public string PlayerNumber
        {
            get; set;
        }

        /// <summary>
        /// Optional, must only be used when the playernumber is empty.
        /// </summary>
        public string FirstName
        {
            get; set;
        }

        /// <summary>
        /// Required when the playernumber is empty.Must only be used when the playernumber is empty.
        /// </summary>
        public string LastName
        {
            get; set;
        }

        /// <summary>
        /// Optional, must only be used when the playernumber is empty.
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
            var validationMessages =new List<string>();
            if (SessionGuid.Length != 32)
            {
                validationMessages.Add($"Invalid {nameof(SessionGuid)} ({SessionGuid}). The value must be in capitals and be exactly 32 characters long.");
            }
            if (!Regex.IsMatch(SectionLetters ?? "", @"^([A-Z])\1{0,2}$"))
            {
                validationMessages.Add($"Invalid {nameof(SectionLetters)} ({SectionLetters}). Valid values are: 'A-Z', 'AA-ZZ' or 'AAA','ZZZ'");
            }
            if ((int)Direction < 1 || (int)Direction > 4)
            {
                validationMessages.Add($"Invalid {nameof(Direction)} ({Direction}). The value must be between 1 and 4.");
            }
            if (TableNumber < 1)
            {
                validationMessages.Add($"Invalid {nameof(TableNumber)} ({TableNumber}). The value must be greater than zero.");
            }
            if (RoundNumber < 0 || RoundNumber > 1)
            {
                validationMessages.Add($"Invalid {nameof(RoundNumber)} ({RoundNumber}). Currently only the values zero and one are supported..");
            }
            if (string.IsNullOrWhiteSpace(LastName) && string.IsNullOrWhiteSpace(PlayerNumber))
            {
                validationMessages.Add($"Either the {nameof(LastName)} or the {nameof(PlayerNumber)} must be specified.");
            }
            if (!string.IsNullOrWhiteSpace(LastName) && !string.IsNullOrWhiteSpace(PlayerNumber))
            {
                validationMessages.Add($"Either the {nameof(LastName)} or the {nameof(PlayerNumber)} must be specified, " +
                                       $"but not both.");
            }
          
            ValidationMessages = validationMessages.ToArray();
            return !ValidationMessages.Any();
        }

        public ParticipationDTO Clone() => (ParticipationDTO)MemberwiseClone();

        public override string ToString()
        {
            return $"{SectionLetters}{TableNumber} {Direction} round {RoundNumber}: {PlayerNumber} {FirstName} {LastName}";
        }
    }
}
