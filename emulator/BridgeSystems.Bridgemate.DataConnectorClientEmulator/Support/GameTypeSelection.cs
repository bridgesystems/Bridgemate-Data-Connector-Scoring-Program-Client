using BridgeSystems.Bridgemate.DataConnectorClasses.SharedDTO;

namespace BridgeSystems.Bridgemate.DataConnectorClientEmulator.Support;

public class GameTypeSelection
{
    public static List<GameTypeSelection> GetGameTypesList()
        => new()
        {
            new GameTypeSelection(SectionDTO.GameType_Pairs,"Pairs"),
            new GameTypeSelection(SectionDTO.GameType_Teams,"Teams"),
            new GameTypeSelection(SectionDTO.GameType_Individual,"Individual")
        };

    public GameTypeSelection(int gameType, string description)
    {
        GameType = gameType;
        Description = description;
    }
    public string Description
    {
        get; set;
    }
    public int GameType
    {
        get; set;
    }
}
