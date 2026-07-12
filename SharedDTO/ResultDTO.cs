using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BridgeSystems.Bridgemate.DataConnectorClasses.SharedDTO
{
    /// <summary>
    /// This class carries information on the result of a board that was played in a specific round on a specific table.
    /// The Level property is also used to express "No Play" and artificial results (using negative values).
    /// </summary>
    public class ResultDTO
    {
        /// <summary>
        /// The result pertains to North-South only. i.e.: The East-West pair may have a different result and a ResultDTO with its ScoringDirection property 
        /// set to ScoringDirection_EW must be provided.
        /// </summary>
        public const int ScoringDirection_NS = 1;

        /// <summary>
        /// The result pertains to East-West only. i.e.: The North-South pair may have a different result and a ResultDTO with its ScoringDirection property 
        /// set to ScoringDirection_NS must be provided.
        /// </summary>
        public const int ScoringDirection_EW = 2;

        /// <summary>
        /// The result pertains to both North-South and East-West.
        /// </summary>
        public const int ScoringDirection_NSEW = 3;

        /// <summary>
        /// The denomination of the contract is Clubs.
        /// </summary>
        public const int Denomination_Clubs = 1;

        /// <summary>
        /// The denomination of the contract is Diamonds.
        /// </summary>
        public const int Denomination_Diamonds = 2;

        /// <summary>
        /// The denomination of the contract is Hearts.
        /// </summary>
        public const int Denomination_Hearts = 3;

        /// <summary>
        /// The denomination of the contract is Spades.
        /// </summary>
        public const int Denomination_Spades = 4;

        /// <summary>
        /// The denomination of the contract is No Trump.
        /// </summary>
        public const int Denomination_NoTrump = 5;

        /// <summary>
        /// Declarer is the player on the North seat.
        /// </summary>
        public const int Direction_North = 1;

        /// <summary>
        /// Declarer is the player on the East seat.
        /// </summary>
        public const int Direction_East = 2;

        /// <summary>
        /// Declarer is the player on the South seat.
        /// </summary>
        public const int Direction_South = 3;

        /// <summary>
        /// Declarer is the player on the West seat.
        /// </summary>
        public const int Direction_West = 4;

        /// <summary>
        /// The contract of the board was "Pass".
        /// </summary>
        public const int ContractLevel_Pass = 0;

        /// <summary>
        /// The board was not played.
        /// </summary>
        public const int ContractLevel_NoPLay = -10;

        /// <summary>
        /// The contract was not doubled or redoubled.
        /// </summary>
        public const int Stake_Normal = 0;

        /// <summary>
        /// The contract was doubled.
        /// </summary>
        public const int Stake_Doubled = 1;

        /// <summary>
        /// The contract was redoubled.
        /// </summary>
        public const int Stake_Redoubled = 2;

        /// <summary>
        /// An artificial score of Average Minus for the North-South pair and Average Minus for the East-West pair if the ScoringDirection property is
        /// ScoringDirection_NS  or ScoringDirection_NSEW. 
        /// If the ScoringDirection is ScoringDirection_EW then the values are reversed.
        /// </summary>
        public const int AvgMinMin = -1;

        /// <summary>
        /// An artificial score of Average Minus for the North-South pair and Average for the East-West pair.
        /// </summary>
        public const int AvgMinAvg = -2;

        /// <summary>
        /// An artificial score of Average Minus for the North-South pair and Average Plus for the East-West pair if the ScoringDirection property is
        /// ScoringDirection_NS  or ScoringDirection_NSEW. 
        /// If the ScoringDirection is ScoringDirection_EW then the values are reversed.
        /// </summary>
        public const int AvgMinPlus = -3;

        /// <summary>
        /// An artificial score of Average for the North-South pair and Average Minus for the East-West pair if the ScoringDirection property is
        /// ScoringDirection_NS  or ScoringDirection_NSEW. 
        /// If the ScoringDirection is ScoringDirection_EW then the values are reversed.
        /// </summary>
        public const int AvgAvgMin = -4;

        /// <summary>
        /// An artificial score of Average for the North-South pair and Average for the East-West pair.
        /// </summary>
        public const int AvgAvgAvg = -5;

        /// <summary>
        /// An artificial score of Average for the North-South pair and Average Plus for the East-West pair if the ScoringDirection property is
        /// ScoringDirection_NS  or ScoringDirection_NSEW. 
        /// If the ScoringDirection is ScoringDirection_EW then the values are reversed.
        /// </summary>
        public const int AvgAvgPlus = -6;

        /// <summary>
        /// An artificial score of Average Plus for the North-South pair and Average Minus for the East-West pair if the ScoringDirection property is
        /// ScoringDirection_NS  or ScoringDirection_NSEW. 
        /// If the ScoringDirection is ScoringDirection_EW then the values are reversed.
        /// </summary>
        public const int AvgPlusMin = -7;

        /// <summary>
        /// An artificial score of Average Plus for the North-South pair and Average for the East-West pair if the ScoringDirection property is
        /// ScoringDirection_NS  or ScoringDirection_NSEW. 
        /// If the ScoringDirection is ScoringDirection_EW then the values are reversed.
        /// </summary>
        public const int AvgPlusAvg = -8;

        /// <summary>
        /// An artificial score of Average Plus for the North-South pair and Average Plus for the East-West pair.
        /// </summary>
        public const int AvgPlusPlus = -9;

        /// <summary>
        /// Required.The guid of the session the result belongs to.
        /// </summary>
        public string SessionGuid
        {
            get; set;
        }
        /// <summary>
        /// Required, the letters of the section in which the result was obtained.
        /// The section must exist.
        /// </summary>
        public string SectionLetters
        {
            get;set;
        }  
        /// <summary>
        /// Required, must be greater than zero.
        /// The table must exist.
        /// </summary>
        public int TableNumber
        {
            get; set;
        }

        /// <summary>
        /// Required, must be greater than zero.
        /// The round must exist.
        /// </summary>
        public int RoundNumber
        {
            get; set;
        }

        /// <summary>
        /// Required, must be greater than zero.
        /// The board must exist, i.e.: it must have been specified in the RoundDTO's LowBoardNumber and HighBoardNumber properties.
        /// </summary>
        public int BoardNumber
        {
            get; set;
        }

        /// <summary>
        /// Required. The pair for which the result will count towards their result.
        /// 1: NortSouth
        /// 2: EastWest
        /// 3: For NorthSouth as wel as for EastWest
        /// Note: currently split scores (scores different for NS and EW) are not supported. BCS will work with the NS value.
        /// </summary>
        public int ScoringDirection
        {
            get; set;
        }

        /// <summary>
        /// Marks the result as deleted. All properties below will be ignored.
        /// </summary>
        public bool IsDeleted
        {
            get;set;
        }

        /// <summary>
        /// Required: The pairnumber for NorthSouth
        /// </summary>
        public int PairNorthSouth
        {
            get; set;
        }

        /// <summary>
        /// Required:The pairnumber for EastWest
        /// </summary>
        public int PairEastWest
        {
            get; set;
        }

        /// <summary>
        /// Required: either the PairNorthSouth or the PairEastwest number.
        /// </summary>
        public int DeclaringPair
        {
            get; set;
        }

        /// <summary>
        /// The direction of the declarer:
        /// 0: NA
        /// 1: North or NorthSouth
        /// 2: East or EastWest
        /// 3: South
        /// 4: West
        /// </summary>
        public int DeclarerDirection
        {
            get; set;
        }

        /// <summary>
        /// The contract level:
        /// 1-7 natural contract
        /// 0 Pass
        /// -1 Avg-/Avg-
        /// -2 Avg-/Avg
        /// -3 Avg-/Av+
        /// -4 Avg/Avg-
        /// -5 Avg/Avg
        /// -6 Avg/Avg+
        /// -7 Avg+/Avg-
        /// -8 Avg+/Avg
        /// -9 Avg+/Avg+
        ///-10 No play
        /// </summary>s
        public int Level
        {
            get; set;
        }

        /// <summary>
        /// The denomination:
        /// 0: NA
        /// 1: Clubs
        /// 2: Diamonds
        /// 3: Hearts
        /// 4: Spades
        /// 5: NoTrump
        /// </summary>
        public int Denomination
        {
            get; set;
        }

        /// <summary>
        /// The stake of the contract:
        /// 0: normal
        /// 1: doubled
        /// 2: redoubled
        /// </summary>
        public int Stake
        {
            get; set;
        }

        /// <summary>
        /// A number between 0 and 13 describing the number of tricks made.
        /// Zero tricks can also mean: pass, or artificial result (see Level)
        /// </summary>
        public int TotalTricks
        {
            get; set;
        }

        /// <summary>
        /// Optional:
        /// 2-10: the card value
        /// 11: Jack
        /// 12: Queen
        /// 13: King
        /// 14: Ace
        /// </summary>
        public int LeadCardRank
        {
            get; set;
        }

        /// <summary>
        /// Required when LeadCardRank > 0
        /// 1: Clubs
        /// 2: Diamonds
        /// 3: Hearts
        /// 4: Spades
        /// </summary>
        public int LeadCardSuit
        {
            get; set;
        }

        /// <summary>
        /// Calculates if the board was played int the NS direction by the EW pair or vice versa.
        /// </summary>
        /// <returns></returns>
        public bool IsSwitched()
        {
            if (DeclarerDirection == 1 || DeclarerDirection == 3)
            {
                if (DeclaringPair == PairNorthSouth)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            if (DeclarerDirection == 2 || DeclarerDirection == 4)
            {
                if (DeclaringPair == PairEastWest)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
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
            var validationResults = new List<string>();
            if (string.IsNullOrEmpty(SessionGuid) || SessionGuid.Length != 32)
            {
                validationResults.Add($"Invalid {nameof(SessionGuid)} ({SessionGuid}). The value must be in capitals and be exactly");
            }
            if (!Regex.IsMatch(SectionLetters ?? "", @"^([A-Z])\1{0,2}$"))
            {
                validationResults.Add($"Invalid {nameof(SectionLetters)} ({SectionLetters}). Valid values are: 'A-Z', 'AA-ZZ' or 'AAA','ZZZ'");
            }
            if (TableNumber < 1)
            {
                validationResults.Add($"Invalid {nameof(TableNumber)} ({TableNumber}). The value must be greater than zero.");
            }
            if (RoundNumber < 1)
            {
                validationResults.Add($"Invalid {nameof(RoundNumber)} ({RoundNumber}). The value must be greater than zero.");
            }
            if (BoardNumber < 1)
            {
                validationResults.Add($"Invalid {nameof(BoardNumber)} ({BoardNumber}). The value must be greater than zero.");
            }
            if (IsDeleted)
            {
                ValidationMessages = validationResults.ToArray();
                return !validationResults.Any();
            }
            if (PairEastWest < 1)
            {
                validationResults.Add($"Invalid {nameof(PairEastWest)} ({PairEastWest}). The value must be greater than zero.");
            }
            if (PairNorthSouth < 1)
            {
                validationResults.Add($"Invalid {nameof(PairNorthSouth)} ({PairNorthSouth}). The value must be greater than zero.");
            }
            if (DeclaringPair != PairNorthSouth && DeclaringPair != PairEastWest)
            {
                validationResults.Add($"Invalid {nameof(DeclaringPair)} ({DeclaringPair}). The value must be either {PairNorthSouth} or {PairEastWest}.");
            }
            if (Level >= 1 && (DeclarerDirection < 1 || DeclarerDirection > 4))
            {
                validationResults.Add($"Invalid {nameof(DeclarerDirection)} ({DeclarerDirection}). The value must be between 1 and 4.");
            }
            if (Level >= 1 && (ScoringDirection < 1 || ScoringDirection > 3))
            {
                validationResults.Add($"Invalid {nameof(ScoringDirection)} ({ScoringDirection}). The value must be between 1 and 3.");
            }
            if (Level < -10 || Level > 7)
            {
                validationResults.Add($"Invalid {nameof(Level)} ({Level}). The value must be between -10 and +7.");
            }
            if ((Denomination < 1 || Denomination > 5) && Level >= 1)
            {
                validationResults.Add($"Invalid {nameof(Denomination)} ({Denomination}). The value must be between 1 and 5.");
            }
            if (Stake < 0 || Stake > 2)
            {
                validationResults.Add($"Invalid {nameof(Stake)} ({Stake}). The value must be between 0 and 2.");
            }
            if (TotalTricks < 0 || TotalTricks > 13)
            {
                validationResults.Add($"Invalid {nameof(TotalTricks)} ({TotalTricks}). The value must be between 0 and 13.");
            }
            if ((LeadCardRank < 2 || LeadCardRank > 14) && LeadCardRank != 0)
            {
                validationResults.Add($"Invalid {nameof(LeadCardRank)} ({LeadCardRank}). The value must be between 2 and 14 (Ace).");
            }
            else if ((LeadCardSuit < 1 || LeadCardSuit > 4) && LeadCardRank != 0)
            {
                validationResults.Add($"Invalid {nameof(LeadCardSuit)} ({LeadCardSuit}). If LeadCardRank>0  the value must be between 1 and 4.");
            }

            ValidationMessages = validationResults.ToArray();
            return !validationResults.Any();

        }

        public override string ToString()
        {
            if (IsDeleted)
            {
                return $"Deleted on table {SectionLetters}{TableNumber} round {RoundNumber} board {BoardNumber}.";
            }
            var level = string.Empty;
            switch (Level)
            {
                case 0:
                    level = "pass";
                    break;
                case 1:
                    level = "1";
                    break;
                case 2:
                    level = "2";
                    break;
                case 3:
                    level = "3";
                    break;
                case 4:
                    level = "4";
                    break;
                case 5:
                    level = "5";
                    break;
                case 6:
                    level = "6";
                    break;
                case 7:
                    level = "7";
                    break;
                case -1:
                    level = "A--";
                    break;
                case -2:
                    level = "A-=";
                    break;
                case -3:
                    level = "A-+";
                    break;
                case -4:
                    level = "A=-";
                    break;
                case -5:
                    level = "A==";
                    break;
                case -6:
                    level = "A=+";
                    break;
                case -7:
                    level = "A+-";
                    break;
                case -8:
                    level = "A+=";
                    break;
                case -9:
                    level = "A++";
                    break;
                case -10:
                    level = "NP";
                    break;
                default:
                    level = string.Empty;
                    break;
            };
            var denom = string.Empty;
            switch (Denomination)
            {
                case 1:
                    denom= "C";
                    break;
                case 2:
                    denom = "D";
                    break;
                case 3:
                    denom= "H";
                    break;
                case 4:
                    denom = "S";
                    break;
                case 5:
                    denom = "NT";
                    break;
                default : 
                    denom = string.Empty; 
                    break;
            };

            string stake;
            switch (Stake)
            {
                case Stake_Doubled: stake = "x";break;
                case Stake_Redoubled: stake = "xx";break;
                default: stake = string.Empty;break;
            };

            string scoringSide;
            switch (ScoringDirection)
            {
                case ResultDTO.ScoringDirection_NS:scoringSide = "NS";break;
                case ResultDTO.ScoringDirection_EW:scoringSide = "EW";break;
                default:scoringSide=string.Empty;break;
            };

            string declarer;
             switch(DeclarerDirection)
            {
                case ResultDTO.Direction_North: declarer = "N";break;
                case ResultDTO.Direction_South: declarer = "S";break;
                case ResultDTO.Direction_West: declarer = "W";break;
                case ResultDTO.Direction_East: declarer = "E";break;
                default : declarer = string.Empty; break;
            };

            var resultTricks = TotalTricks - Level - 6;
            var sign = resultTricks > 0 ? "+" : resultTricks < 0 ? "-" : "";
            var result = Level >= 1 && Level <= 7 ?
                TotalTricks == Level + 6 ? "=" :
                    sign + (TotalTricks - Level - 6).ToString() : string.Empty;

            var rslt = $"{scoringSide}: {SectionLetters}{TableNumber} round {RoundNumber} board {BoardNumber}; {PairNorthSouth}-{PairEastWest}: " +
                $"{level}{denom}{stake} {result} by {DeclaringPair} ({declarer})";

            return rslt;
        }
    }
}
