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

