namespace BridgeSystems.Bridgemate.DataConnectorClientEmulator.Movements;
public class Seating
{
    public int RoundNumber
    {
        get; set;
    }
    public int TableNumber
    {
        get; set;
    }
    public int NorthSouthPair
    {
        get; set;
    }
    public int EastWestPair
    {
        get; set;
    }
    public int BoardSet
    {
        get; set;
    }

    public bool RemovePair(int pairNumber)
    {
        var removed = false;
        if (NorthSouthPair == pairNumber)
        {
            NorthSouthPair = 0;
            removed = true;
        }
        else if (EastWestPair == pairNumber)
        {
            EastWestPair = 0;
            removed = true;
        }

        return removed;
    }
}

