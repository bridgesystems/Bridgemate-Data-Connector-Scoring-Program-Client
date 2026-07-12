using BridgeSystems.Bridgemate.DataConnectorClasses.SharedDTO;

namespace BridgeSystems.Bridgemate.DataConnectorClientEmulator.Support;

public class StakeSelection
{
    public static List<StakeSelection> GetStakeSelectionsList() =>
        new()
        {
            new StakeSelection(ResultDTO.Stake_Normal,"  "),
            new StakeSelection(ResultDTO.Stake_Doubled,"x "),
            new StakeSelection(ResultDTO.Stake_Redoubled,"xx"),
        };
    public StakeSelection(int stake, string description)
    {
        Description = description;
        Stake = stake;
    }
    public string Description
    {
        get; set;
    }
    public int Stake
    {
        get; set;
    }
}
