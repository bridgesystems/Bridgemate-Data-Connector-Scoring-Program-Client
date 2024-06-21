using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BridgeSystems.Bridgemate.DataConnectorClasses.SharedDTO
{
    /// <summary>
    /// Contains the data for a handrecord and optionally its double dummy values.
    /// </summary>
    public class HandrecordDTO
    {
        /// <summary>
        /// Required. The guid of the session the handrecord belongs to. A string built from a Guid, without the curly braces or connecting dashes.
        /// The letters must be capitals.
        /// Must be unique
        /// </summary>
        public string SessionGuid
        {
            get; set;
        }

        /// <summary>
        /// Required. The number of the scoringgroup that the handrecord belongs to.
        /// </summary>
        public int ScoringGroupNumber
        {
            get; set;
        }

        /// <summary>
        /// Required, any of the sections that are part of the scoringgroup.
        /// </summary>
        public string SectionLetters
        {
            get; set;
        }

        /// <summary>
        /// Required. Must be greater than zero.
        /// </summary>
        public int BoardNumber
        {
            get; set;
        }

        /// <summary>
        /// Valid card values are AKQJT98765432
        /// Indicate a void with an empty string. Null will be disqualified.
        /// </summary>
        public string NorthClubs
        {
            get; set;
        }

        /// <summary>
        /// Valid card values are AKQJT98765432
        /// Indicate a void with an empty string. Null will be disqualified.
        /// </summary>
        public string NorthDiamonds
        {
            get; set;
        }

        /// <summary>
        /// Valid card values are AKQJT98765432
        /// Indicate a void with an empty string. Null will be disqualified.
        /// </summary>
        public string NorthHearts
        {
            get; set;
        }


        /// <summary>
        /// Valid card values are AKQJT98765432
        /// Indicate a void with an empty string. Null will be disqualified.
        /// </summary>
        public string NorthSpades
        {
            get; set;
        }

        /// <summary>
        /// Valid card values are AKQJT98765432
        /// Indicate a void with an empty string. Null will be disqualified.
        /// </summary>
        public string EastClubs
        {
            get; set;
        }

        /// <summary>
        /// Valid card values are AKQJT98765432
        /// Indicate a void with an empty string. Null will be disqualified.
        /// </summary>
        public string EastDiamonds
        {
            get; set;
        }

        /// <summary>
        /// Valid card values are AKQJT98765432
        /// Indicate a void with an empty string. Null will be disqualified.
        /// </summary>
        public string EastHearts
        {
            get; set;
        }

        /// <summary>
        /// Valid card values are AKQJT98765432
        /// Indicate a void with an empty string. Null will be disqualified.
        /// </summary>
        public string EastSpades
        {
            get; set;
        }

        /// <summary>
        /// Valid card values are AKQJT98765432
        /// Indicate a void with an empty string. Null will be disqualified.
        /// </summary>
        public string SouthClubs
        {
            get; set;
        }

        /// <summary>
        /// Valid card values are AKQJT98765432
        /// Indicate a void with an empty string. Null will be disqualified.
        /// </summary>
        public string SouthDiamonds
        {
            get; set;
        }

        /// <summary>
        /// Valid card values are AKQJT98765432
        /// Indicate a void with an empty string. Null will be disqualified.
        /// </summary>
        public string SouthHearts
        {
            get; set;
        }

        /// <summary>
        /// Valid card values are AKQJT98765432
        /// Indicate a void with an empty string. Null will be disqualified.
        /// </summary>
        public string SouthSpades
        {
            get; set;
        }

        /// <summary>
        /// Valid card values are AKQJT98765432
        /// Indicate a void with an empty string. Null will be disqualified.
        /// </summary>
        public string WestClubs
        {
            get; set;
        }

        /// <summary>
        /// Valid card values are AKQJT98765432
        /// Indicate a void with an empty string. Null will be disqualified.
        /// </summary>
        public string WestDiamonds
        {
            get; set;
        }

        /// <summary>
        /// Valid card values are AKQJT98765432
        /// Indicate a void with an empty string. Null will be disqualified.
        /// </summary>
        public string WestHearts
        {
            get; set;
        }

        /// <summary>
        /// Valid card values are AKQJT98765432
        /// Indicate a void with an empty string. Null will be disqualified.
        /// </summary>
        public string WestSpades
        {
            get; set;
        }

        /// <summary>
        /// It "True" the double dummy values will be processed by the Data Connector.
        /// </summary>
        public bool HasHandEvaluationData
        {
            get; set;
        }


        /// <summary>
        /// The number of tricks that can be made with North as declarer and Clubs as denomination.
        /// </summary>
        public int DoubleDummyNorthClubs
        {
            get; set;
        }

        /// <summary>
        /// The number of tricks that can be made with North as declarer and Diamonds as denomination.
        /// </summary>
        public int DoubleDummyNorthDiamonds
        {
            get; set;
        }

        /// <summary>
        /// The number of tricks that can be made with North as declarer and Hearts as denomination.
        /// </summary>
        public int DoubleDummyNorthHearts
        {
            get; set;
        }

        /// <summary>
        /// The number of tricks that can be made with North as declarer and Spades as denomination.
        /// </summary>
        public int DoubleDummyNorthSpades
        {
            get; set;
        }

        /// <summary>
        /// The number of tricks that can be made with North as declarer and No Trump as denomination.
        /// </summary>
        public int DoubleDummyNorthNoTrump
        {
            get; set;
        }

        /// <summary>
        /// The number of tricks that can be made with East as declarer and Clubs as denomination.
        /// </summary>
        public int DoubleDummyEastClubs
        {
            get; set;
        }

        /// <summary>
        /// The number of tricks that can be made with East as declarer and Diamonds as denomination.
        /// </summary>
        public int DoubleDummyEastDiamonds
        {
            get; set;
        }

        /// <summary>
        /// The number of tricks that can be made with East as declarer and Hearts as denomination.
        /// </summary>
        public int DoubleDummyEastHearts
        {
            get; set;
        }

        /// <summary>
        /// The number of tricks that can be made with East as declarer and Spades as denomination.
        /// </summary>
        public int DoubleDummyEastSpades
        {
            get; set;
        }

        /// <summary>
        /// The number of tricks that can be made with East as declarer and No Trump as denomination.
        /// </summary>
        public int DoubleDummyEastNoTrump
        {
            get; set;
        }

        /// <summary>
        /// The number of tricks that can be made with South as declarer and Clubs as denomination.
        /// </summary>
        public int DoubleDummySouthClubs
        {
            get; set;
        }

        /// <summary>
        /// The number of tricks that can be made with South as declarer and Diamonds as denomination.
        /// </summary>
        public int DoubleDummySouthDiamonds
        {
            get; set;
        }

        /// <summary>
        /// The number of tricks that can be made with South as declarer and Hearts as denomination.
        /// </summary>
        public int DoubleDummySouthHearts
        {
            get; set;
        }

        /// <summary>
        /// The number of tricks that can be made with South as declarer and Spades as denomination.
        /// </summary>
        public int DoubleDummySouthSpades
        {
            get; set;
        }

        /// <summary>
        /// The number of tricks that can be made with South as declarer and No Trump as denomination.
        /// </summary>
        public int DoubleDummySouthNoTrump
        {
            get; set;
        }

        /// <summary>
        /// The number of tricks that can be made with West as declarer and Clubs as denomination.
        /// </summary>
        public int DoubleDummyWestClubs
        {
            get; set;
        }

        /// <summary>
        /// The number of tricks that can be made with West as declarer and Diamonds as denomination.
        /// </summary>
        public int DoubleDummyWestDiamonds
        {
            get; set;
        }

        /// <summary>
        /// The number of tricks that can be made with West as declarer and Hearts as denomination.
        /// </summary>
        public int DoubleDummyWestHearts
        {
            get; set;
        }

        /// <summary>
        /// The number of tricks that can be made with West as declarer and Spades as denomination.
        /// </summary>
        public int DoubleDummyWestSpades
        {
            get; set;
        }

        /// <summary>
        /// The number of tricks that can be made with West as declarer and No Trump as denomination.
        /// </summary>
        public int DoubleDummyWestNoTrump
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
        public virtual bool Validate()
        {
            var validationMessages=new List<string>();
            if (SessionGuid == null || SessionGuid.Length != 32 || SessionGuid.Any(c => !(c >= 'A' && c <= 'F' || c >= '0' && c <= '9')))
            {
                validationMessages.Add($"The guid ({SessionGuid}) must be exactly 32 character long and can only contain capital A to F or digits 0 to 9.");
            }
            if (ScoringGroupNumber <= 0)
            {
                validationMessages.Add($"{nameof(ScoringGroupNumber)} ({ScoringGroupNumber}) must be greater than zero.");
            }
            if (!Regex.IsMatch(SectionLetters ?? "", @"^([A-Z])\1{0,2}$"))
            {
                validationMessages.Add($"Invalid {nameof(SectionLetters)} ({SectionLetters}). Valid values are: 'A-Z', 'AA-ZZ' or 'AAA','ZZZ'");
            }
            if (BoardNumber <= 0)
            {
                validationMessages.Add($"{nameof(BoardNumber)} ({BoardNumber}) must be greater than zero.");
            }
            var allClubs = new[] { (nameof(NorthClubs), NorthClubs), (nameof(EastClubs), EastClubs), (nameof(SouthClubs), SouthClubs), (nameof(WestClubs), WestClubs) };
            var allDiamonds = new[] { (nameof(NorthDiamonds), NorthDiamonds), (nameof(EastDiamonds), EastDiamonds), (nameof(SouthDiamonds), SouthDiamonds), (nameof(WestDiamonds), WestDiamonds) };
            var allHearts = new[] { (nameof(NorthHearts), NorthHearts), (nameof(EastHearts), EastHearts), (nameof(SouthHearts), SouthHearts), (nameof(WestHearts), WestHearts) };
            var allSpades = new[] { (nameof(NorthSpades), NorthSpades), (nameof(EastSpades), EastSpades), (nameof(SouthSpades), SouthSpades), (nameof(WestSpades), WestSpades) };

            var allNorth = new[] { (nameof(NorthClubs), NorthClubs), (nameof(NorthDiamonds), NorthDiamonds), (nameof(NorthHearts), NorthHearts), (nameof(NorthSpades), NorthSpades) };
            var allEast = new[] { (nameof(EastClubs), EastClubs), (nameof(EastDiamonds), EastDiamonds), (nameof(EastHearts), EastHearts), (nameof(EastSpades), EastSpades) };
            var allSouth = new[] { (nameof(SouthClubs), SouthClubs), (nameof(SouthDiamonds), SouthDiamonds), (nameof(SouthHearts), SouthHearts), (nameof(SouthSpades), SouthSpades) };
            var allWest = new[] { (nameof(WestClubs), WestClubs), (nameof(WestDiamonds), WestDiamonds), (nameof(WestHearts), WestHearts), (nameof(WestSpades), WestSpades) };

            var allSuits = new[]
            {
                (nameof(NorthClubs), NorthClubs),(nameof(NorthDiamonds), NorthDiamonds),(nameof(NorthHearts), NorthHearts),(nameof(NorthSpades), NorthSpades),
                (nameof(EastClubs), EastClubs),(nameof(EastDiamonds), EastDiamonds),(nameof(EastHearts), EastHearts),(nameof(EastSpades),EastSpades),
                (nameof(SouthClubs), SouthClubs),(nameof(SouthDiamonds), SouthDiamonds),(nameof(SouthHearts), SouthHearts),(nameof(SouthSpades), SouthSpades),
                (nameof(WestClubs), WestClubs), (nameof(WestDiamonds), WestDiamonds), (nameof(WestHearts), WestHearts),(nameof(WestSpades), WestSpades)
            };

            var correctCards = new string("AKQJT98765432".OrderBy(card => card).ToArray());

            var invalidSuits = (from suit in allSuits
                                where suit.Item2 == null ||
                                    (!suit.Item2.All(card => correctCards.Contains(card)) && suit.Item2 != string.Empty)
                                select suit.Item1).ToList();

            if (invalidSuits.Any())
            {
                var errorMessage = string.Join(", ", invalidSuits);
                validationMessages.Add($"Invalid suits (null value or invalid card) in {errorMessage}. Valid cards are '{correctCards}'");
            }
            else
            {
                var allClubsSuit = allClubs.SelectMany(ac => ac.Item2);
                var numberOfClubs = allClubsSuit.Count();
                if (numberOfClubs != 13)
                {
                    validationMessages.Add($"Invalid number of clubs ({numberOfClubs}). The suit must add up to 13 cards.");
                }
                else
                {
                    var suitString = new string(allClubsSuit.OrderBy(card => card).ToArray());
                    if (suitString != correctCards)
                    {
                        validationMessages.Add($"Duplicate cards in the clubs suit '{suitString}'");
                    }
                }

                var allDiamondsSuit = allDiamonds.SelectMany(ac => ac.Item2);
                var numberOfDiamonds = allDiamondsSuit.Count();
                if (numberOfDiamonds != 13)
                {
                    validationMessages.Add($"Invalid number of Diamonds ({numberOfDiamonds}). The suit must add up to 13 cards.");
                }
                else
                {
                    var suitString = new string(allDiamondsSuit.OrderBy(card => card).ToArray());
                    if (suitString != correctCards)
                    {
                        validationMessages.Add($"Duplicate cards in the Diamonds suit '{suitString}'");
                    }
                }

                var allHeartsSuit = allHearts.SelectMany(ac => ac.Item2);
                var numberOfHearts = allHeartsSuit.Count();
                if (numberOfHearts != 13)
                {
                    validationMessages.Add($"Invalid number of Hearts ({numberOfHearts}). The suit must add up to 13 cards.");
                }
                else
                {
                    var suitString = new string(allHeartsSuit.OrderBy(card => card).ToArray());
                    if (suitString != correctCards)
                    {
                        validationMessages.Add($"Duplicate cards in the Hearts suit '{suitString}'");
                    }
                }

                var allSpadesSuit = allSpades.SelectMany(ac => ac.Item2);
                var numberOfSpades = allSpadesSuit.Count();
                if (numberOfSpades != 13)
                {
                    validationMessages.Add($"Invalid number of Spades ({numberOfSpades}). The suit must add up to 13 cards.");
                }
                else
                {
                    var suitString = new string(allSpadesSuit.OrderBy(card => card).ToArray());
                    if (suitString != correctCards)
                    {
                        validationMessages.Add($"Duplicate cards in the Spades suit '{suitString}'");
                    }
                }

                var numberOfNorthCards = allNorth.SelectMany(ac => ac.Item2).Count();
                if (numberOfNorthCards != 13)
                {
                    validationMessages.Add($"Invalid number of cards for North ({numberOfNorthCards}). The hand must add up to 13 cards.");
                }

                var numberOfEastCards = allEast.SelectMany(ac => ac.Item2).Count();
                if (numberOfEastCards != 13)
                {
                    validationMessages.Add($"Invalid number of cards for East ({numberOfEastCards}). The hand must add up to 13 cards.");
                }

                var numberOfSouthCards = allSouth.SelectMany(ac => ac.Item2).Count();
                if (numberOfSouthCards != 13)
                {
                    validationMessages.Add($"Invalid number of cards for South ({numberOfSouthCards}). The hand must add up to 13 cards.");
                }

                var numberOfWestCards = allWest.SelectMany(ac => ac.Item2).Count();
                if (numberOfWestCards != 13)
                {
                    validationMessages.Add($"Invalid number of cards for West ({numberOfWestCards}). The hand must add up to 13 cards.");
                }
            }
            ValidationMessages=validationMessages.ToArray();
            return !ValidationMessages.Any();
        }

        public override string ToString()
        {
            var header = $"{SectionLetters}{BoardNumber} ({ScoringGroupNumber}) ";
            var north = $"S:{NorthSpades}-" +
                        $"H:{NorthHearts}-" +
                        $"D:{NorthDiamonds}-" +
                        $"C:{NorthClubs}";
            return $"{header}: {north}";

        }
    }
}