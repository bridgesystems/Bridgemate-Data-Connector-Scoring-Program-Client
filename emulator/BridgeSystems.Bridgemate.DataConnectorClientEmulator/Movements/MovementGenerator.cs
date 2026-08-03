using System.Diagnostics.Metrics;
using System.Windows.Documents.DocumentStructures;
using BridgeSystems.Bridgemate.DataConnectorClasses.SharedDTO;

namespace BridgeSystems.Bridgemate.DataConnectorClientEmulator.Movements;

public static class MovementGenerator
{
    public static List<Seating> GenerateMitchellSeatings(int numberOfPairs,int numberOfRounds, bool fixedBoards, int numberOfWinners,bool consecutiveNumbering,
        int ewOffset=0, int arrowShiftRound=0)
    {



        var hasSitOut = numberOfPairs % 2 != 0;

        //Auto apply arrowshift when there is one winner.
        if (numberOfWinners == 1 && arrowShiftRound == 0)
            arrowShiftRound = 2;

        if (hasSitOut)
            numberOfPairs++; // add phantom pair

        var numberOfTables = numberOfPairs / 2;

        //Skip one round (one less total rounds) when the number of tables is even.
        var useSkip =numberOfTables % 2 == 0;

        //There will be one less round if a skip is applied.
        var maxRounds = useSkip ? numberOfTables - 1 : numberOfTables;
        
        var rounds=Math.Min(maxRounds, numberOfRounds);

        var seatings = new List<Seating>();

        for (var round = 1; round <= rounds; round++)
        {
            var arrowShift = arrowShiftRound == round;
            for (var table = 1; table <= numberOfTables; table++)
            {
                var nsPair = table;

                var skip = (numberOfTables % 2 == 0 && round > numberOfTables / 2) ? 1 : 0;

                var ewPair =
                    numberOfTables + ((table + round - 2 + skip) % numberOfTables) + 1;

                if (arrowShift)
                {
                    var temp = nsPair;
                    nsPair= ewPair;
                    ewPair= temp;
                }

                var boardSet =
                    ((table - round + numberOfTables * 10) % numberOfTables) + 1;

                seatings.Add(new Seating
                {
                    RoundNumber = round,
                    TableNumber = table,
                    NorthSouthPair = nsPair,
                    EastWestPair = ewPair,
                    BoardSet = boardSet
                });
            }
        }

        if (fixedBoards)
            ApplyFixedBoards(seatings);

        //Always apply fixed start positions: the pairs playing in the first round will be dependant on the table number.
        ApplyFixedStartPosition(seatings, numberOfPairs, twoWinnerMovement: numberOfWinners == 2, ewOffset: consecutiveNumbering ? -1 : ewOffset);

        //remove the phantompair if present.
        if (hasSitOut)
        {
            foreach (Seating seating in seatings)
                seating.RemovePair(numberOfPairs);
        }

        return seatings;
    }

    public static void ApplyFixedBoards(List<Seating> seatings)
    {
        foreach (Seating seating in seatings)
        {
            seating.TableNumber = seating.BoardSet;
        }
    }

    public static void ApplyFixedStartPosition(List<Seating> seatings, int numberOfPairs, bool twoWinnerMovement = false, int ewOffset = 0)
    {
        var firstRoundSeatings = seatings.Where(seating => seating.RoundNumber == 1).OrderBy(seating => seating.TableNumber).ToList();
        List<int> currentFirstRoundPairs = firstRoundSeatings.SelectMany(seating => new[] { seating.NorthSouthPair, seating.EastWestPair }).ToList();
        var swapList = new Dictionary<(int number, string direction), int>();
        var tableCounter = 0;
        foreach (var pairNumberPair in currentFirstRoundPairs.Chunk(2))
        {
            var nsPair = pairNumberPair.First();
            var ewPair = pairNumberPair.Last();
            tableCounter++;
            var newNsNumber = twoWinnerMovement && ewOffset>=0 ? tableCounter : (tableCounter - 1) * 2 + 1;
            var newEwNumber = twoWinnerMovement && ewOffset>=0? 
                tableCounter + ewOffset :
                (tableCounter - 1) * 2 + 2;
            if (twoWinnerMovement)
            {
                swapList.Add((nsPair, "NS"), newNsNumber);
                swapList.Add((ewPair, "EW"), newEwNumber);
            }
            else
            {
                swapList.Add((nsPair,""), newNsNumber);
                swapList.Add((ewPair, ""), newEwNumber);
            }
        }
        foreach (var seating in seatings)
        {
            if (twoWinnerMovement)
            {
                if (swapList.TryGetValue((seating.NorthSouthPair,"NS"), out var newNsNumber))
                {
                    if (swapList.TryGetValue((seating.EastWestPair, "EW"), out var newEwNumber))
                    {
                        seating.NorthSouthPair = newNsNumber;
                        seating.EastWestPair = newEwNumber;
                    }
                }
            }
            else
            {
                if (swapList.TryGetValue((seating.NorthSouthPair, ""), out var newNsNumber))
                {
                    if (swapList.TryGetValue((seating.EastWestPair, ""), out var newEwNumber))
                    {
                        seating.NorthSouthPair = newNsNumber;
                        seating.EastWestPair = newEwNumber;
                    }
                }
            }
        }
    }


    /// <summary>
    /// Generates the movement for an individual session the way Bridge-It sends it: stable pair
    /// numbers equal to the odd player numbers (4·T players, T tables → pairs 1,3,..,4·T−1).
    /// Pair k is owned by player k, who sits North when the pair is placed North-South in a round
    /// and East when it is placed East-West — never South or West. The even-numbered partner
    /// changes every round (see <see cref="IndividualPartnerOf"/>).
    /// Uses the circle method over the pair owners, so every owner meets every other owner exactly
    /// once; the maximum number of rounds is (numberOfPlayers / 2) − 1. All tables play the same
    /// boards each round (barometer style).
    /// </summary>
    public static List<Seating> GenerateIndividualSeatings(int numberOfPlayers, int numberOfRounds)
    {
        if (numberOfPlayers < 8 || numberOfPlayers % 4 != 0)
            throw new ArgumentException("An individual movement needs a multiple of four players, at least eight.",
                                        nameof(numberOfPlayers));

        var numberOfPairs = numberOfPlayers / 2;
        var numberOfTables = numberOfPairs / 2;
        var maxRounds = numberOfPairs - 1;
        var rounds = Math.Min(numberOfRounds, maxRounds);

        var seatings = new List<Seating>();
        for (var round = 1; round <= rounds; round++)
        {
            //Circle method over the 0-based owner indexes: owner 0 stays put, the rest rotate.
            var order = new List<int> { 0 };
            for (var i = 1; i < numberOfPairs; i++)
                order.Add((i - 1 + (round - 1)) % (numberOfPairs - 1) + 1);

            for (var table = 1; table <= numberOfTables; table++)
            {
                var first = order[table - 1];
                var second = order[numberOfPairs - table];
                //Alternate the NS/EW placement per round so the owners sit both North and East
                //during the session.
                var nsOwner = round % 2 == 1 ? first : second;
                var ewOwner = round % 2 == 1 ? second : first;
                seatings.Add(new Seating
                {
                    RoundNumber = round,
                    TableNumber = table,
                    NorthSouthPair = 2 * nsOwner + 1,
                    EastWestPair = 2 * ewOwner + 1,
                    BoardSet = round
                });
            }
        }
        return seatings;
    }

    /// <summary>
    /// The partner of a pair owner in the given round: the even player numbers rotate over the
    /// owners, so every owner gets a different partner each round.
    /// </summary>
    public static int IndividualPartnerOf(int pairNumber, int roundNumber, int numberOfPlayers)
    {
        var numberOfPairs = numberOfPlayers / 2;
        var ownerIndex = (pairNumber - 1) / 2;
        return 2 * ((ownerIndex + roundNumber - 1) % numberOfPairs) + 2;
    }

    /// <summary>
    /// The explicit participations for every round of an individual movement: the pair owner on
    /// North (NS) or East (EW), the rotating partner on South or West. One DTO per
    /// (table, round, position), player numbers 1..numberOfPlayers.
    /// </summary>
    public static List<ParticipationDTO> CreateIndividualParticipations(IEnumerable<Seating> seatings,
        string sessionGuid, string sectionLetters, int numberOfPlayers)
    {
        var participations = new List<ParticipationDTO>();
        foreach (Seating seating in seatings)
        {
            if (seating.NorthSouthPair > 0)
            {
                participations.Add(CreateParticipation(seating.NorthSouthPair, TableDirection.North));
                participations.Add(CreateParticipation(
                    IndividualPartnerOf(seating.NorthSouthPair, seating.RoundNumber, numberOfPlayers), TableDirection.South));
            }
            if (seating.EastWestPair > 0)
            {
                participations.Add(CreateParticipation(seating.EastWestPair, TableDirection.East));
                participations.Add(CreateParticipation(
                    IndividualPartnerOf(seating.EastWestPair, seating.RoundNumber, numberOfPlayers), TableDirection.West));
            }

            ParticipationDTO CreateParticipation(int playerNumber, TableDirection direction) => new()
            {
                SessionGuid = sessionGuid,
                SectionLetters = sectionLetters,
                TableNumber = seating.TableNumber,
                RoundNumber = seating.RoundNumber,
                Direction = direction,
                PlayerNumber = playerNumber.ToString()
            };
        }
        return participations;
    }

    private static readonly string[] IndividualFirstNames =
    {
        "Anna", "Ben", "Carla", "Daan", "Els", "Frits", "Greet", "Hugo",
        "Ineke", "Joris", "Karin", "Loek", "Marja", "Niels", "Olga", "Piet"
    };

    /// <summary>
    /// Player data for the individual players 1..numberOfPlayers. The last name carries the player
    /// number so the seating is verifiable at a glance on the Bridgemates.
    /// </summary>
    public static List<PlayerDataDTO> CreateIndividualPlayerData(string sessionGuid, int numberOfPlayers)
    {
        var players = new List<PlayerDataDTO>();
        for (var number = 1; number <= numberOfPlayers; number++)
        {
            players.Add(new PlayerDataDTO
            {
                SessionGuid = sessionGuid,
                PlayerNumber = number.ToString(),
                FirstName = IndividualFirstNames[(number - 1) % IndividualFirstNames.Length],
                LastName = $"Player {number:D2}"
            });
        }
        return players;
    }

    public static List<RoundDTO> CreateRoundDTOs(IEnumerable<Seating> seatings, string sessionGuid, string sectionLetters, int numberOfBoardsPerRound)
    {
        return seatings.Select(seating => CreateRoundDTO(seating, sessionGuid, sectionLetters, numberOfBoardsPerRound)).ToList();
    }

    public static RoundDTO CreateRoundDTO(Seating seating, string sessionGuid, string sectionLetters, int numberOfBoardsPerRound)
    {
        var lowestBoardNumber = (seating.BoardSet - 1) * numberOfBoardsPerRound + 1;
        var highesBoardNumber = seating.BoardSet * numberOfBoardsPerRound;

        return new RoundDTO
        {
            SessionGuid = sessionGuid,
            SectionLetters = sectionLetters,
            TableNumber = seating.TableNumber,
            RoundNumber = seating.RoundNumber,
            PairNS = seating.NorthSouthPair,
            PairEW = seating.EastWestPair,
            LowBoardNumber = lowestBoardNumber,
            HighBoardNumber = highesBoardNumber
        };
    }

}

