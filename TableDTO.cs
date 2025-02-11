using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BridgeSystems.Bridgemate.DataConnectorClasses.SharedDTO
{
    /// <summary>
    /// A class describing a real or virtual bridge table where each round two pairs will compete.
    /// </summary>
    public class TableDTO
    {
        /// <summary>
        /// Required. The guid of the session the table belongs to.
        /// </summary>
        public string SessionGuid
        {
            get; set;
        }
        /// <summary>
        /// Required, the letters of the section the table belongs to. The section must exist.
        /// </summary>
        public string SectionLetters
        {
            get; set;
        }

        /// <summary>
        /// Required, must be greater than zero and must be unique within the section.
        /// </summary>
        public int TableNumber
        {
            get; set;
        }

        /// <summary>
        /// Optional: specifies if the Bridgemate on the table should work in online mode (default) or offline mode.
        /// In offline mode the Bridgemate will not communicate with the Bridgemate 3 server during the session, 
        /// but will try to send its results after the last board had been played. 
        /// Because of this the Bridgemate will not respond to movement updates.
        /// Mind: this setting will only have effect when the Bridgemate first connects to the session. 
        /// Once a Bridgemate has been initialized with movement data the ConnectionMode cannot be changed until the Bridgemate is restarted.
        /// </summary>
        public Bm3ConnectionModeOption ConnectionMode{ get; set; }

        /// <summary>
        /// Required. An array of RoundDTOs describing which pairs will meet on this table and which boards they will play.
        /// </summary>
        public RoundDTO[] Rounds
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
            if (!Regex.IsMatch(SectionLetters ?? "", @"^([A-Z])\1{0,2}$"))
            {
                validationMessages.Add($"Invalid {nameof(SectionLetters)} ({SectionLetters}). Valid values are: 'A-Z', 'AA-ZZ' or 'AAA','ZZZ'");
            }
            if (TableNumber <= 0)
            {
                validationMessages.Add($"{nameof(TableNumber)} ({TableNumber}) must be greater than zero.");
            }
            var roundNumbers = Rounds.Select(r => r.RoundNumber).OrderBy(number => number).ToList();
            if (roundNumbers.Any())
            {
                if (roundNumbers.Distinct().Count() != Rounds.Count())
                {
                    validationMessages.Add($"The roundnumbers on a table must be unique.");
                }
                var x = roundNumbers.Select((i, j) => i - j);
                if (roundNumbers.Select((i, j) => i - j)
                                 .Distinct()
                                 .Count() != 1)
                {
                    validationMessages.Add($"The roundnumbers must be consecutive.");
                }
                if (roundNumbers.Any() && roundNumbers.Min() > 1)
                {
                    validationMessages.Add($"The round numbers must start with 1, but the lowest round is {roundNumbers.Min()}");
                }
            }

            foreach (RoundDTO round in Rounds)
            {
                if (round.SessionGuid != SessionGuid)
                {
                    validationMessages.Add($"Round {round.RoundNumber} on table '{SectionLetters}{TableNumber}' must have {nameof(RoundDTO.SessionGuid)} '{SessionGuid}' " +
                                           $"but it is '{round.SessionGuid}'");
                }
                if (round.SectionLetters != SectionLetters)
                {
                    validationMessages.Add($"Round {round.RoundNumber} on table ' {SectionLetters} {TableNumber}' must have {nameof(RoundDTO.SectionLetters)} '{SectionLetters}' " +
                                           $"but it is '{round.SectionLetters}'");
                }
                if (round.TableNumber != TableNumber)
                {
                    validationMessages.Add($"Round {round.RoundNumber} on table ' {SectionLetters} {TableNumber}' must have {nameof(RoundDTO.TableNumber)} '{TableNumber}' " +
                                           $"but it is '{round.TableNumber}'");
                }
                if (!round.Validate())
                {
                    var errorMessage = string.Join("; ", round.ValidationMessages);
                    validationMessages.Add($"Round {round.RoundNumber} on '{SectionLetters}{TableNumber}' has validation errrors: {errorMessage}.");
                }
            }

            ValidationMessages = validationMessages.ToArray();
            return !ValidationMessages.Any();
        }
    }
}
