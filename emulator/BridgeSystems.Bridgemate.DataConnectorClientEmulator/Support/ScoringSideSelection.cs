using BridgeSystems.Bridgemate.DataConnectorClasses.SharedDTO;

namespace BridgeSystems.Bridgemate.DataConnectorClientEmulator.Support;

public class ScoringSideSelection
{
    public static List<ScoringSideSelection> GetScoringSidesList() =>
        new()
        {
            new ScoringSideSelection(0,"     "),
            new ScoringSideSelection(ResultDTO.ScoringDirection_NSEW,"NS/EW (recommended)"),
            new ScoringSideSelection(ResultDTO.ScoringDirection_NS,"NS (include EW result as well."),
            new ScoringSideSelection(ResultDTO.ScoringDirection_EW,"EW (currently ignored)"),
        };

    public ScoringSideSelection(int scoringSide, string description)
    {
        Description = description;
        ScoringSide = scoringSide;
    }
    public string Description
    {
        get; set;
    }
    public int ScoringSide
    {
        get; set;
    }
}
