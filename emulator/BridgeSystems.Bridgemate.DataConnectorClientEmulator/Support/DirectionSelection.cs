using BridgeSystems.Bridgemate.DataConnectorClasses.SharedDTO;

namespace BridgeSystems.Bridgemate.DataConnectorClientEmulator.Support;
public class DirectionSelection
{
    public static List<DirectionSelection> GetDirectionsList() =>
        new()
        {
            new DirectionSelection(TableDirection.North,"North"),
            new DirectionSelection(TableDirection.East,"East"),
            new DirectionSelection(TableDirection.South,"South"),
            new DirectionSelection(TableDirection.West,"West"),
        };

    public DirectionSelection(TableDirection direction, string description)
    {
        Description = description;
        Direction = direction;
    }
    public string Description
    {
        get; set;
    }
    public TableDirection Direction
    {
        get; set;
    }
}
