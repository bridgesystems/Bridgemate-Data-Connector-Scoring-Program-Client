using BridgeSystems.Bridgemate.DataConnectorClasses.SharedDTO;

namespace BridgeSystems.Bridgemate.DataConnectorClientEmulator.Support;

public class ScoringTypeSelection
{
    public static List<ScoringTypeSelection> GetScoringTypesList()
        => new()
        {
            new ScoringTypeSelection(ScoringGroupDTO.ScoringType_Pairs,"Matchpoints"),
            new ScoringTypeSelection(ScoringGroupDTO.ScoringType_XImp3_Total,"Total Cross IMP, Avg+=3 IMP"),
            new ScoringTypeSelection(ScoringGroupDTO.ScoringType_XImp3_Average,"Average Cross IMP, Avg+=3 IMP"),
            new ScoringTypeSelection(ScoringGroupDTO.ScoringType_XImp2_Total,"Total Cross IMP, Avg+=2 IMP"),
            new ScoringTypeSelection(ScoringGroupDTO.ScoringType_XImp2_Average,"Average Cross IMP, Avg+=2 IMP"),
            new ScoringTypeSelection(ScoringGroupDTO.ScoringType_Imp2_NoCorrection,"Butler IMP, no correction of extremes, Avg+=2 IMP"),
            new ScoringTypeSelection(ScoringGroupDTO.ScoringType_Imp2_10Percent,"Butler IMP, omit 10% of extremes, Avg+=2 IMP"),
            new ScoringTypeSelection(ScoringGroupDTO.ScoringType_Imp2_Weighted,"Butler IMP, weighted correction of extremes, Avg+=2 IMP"),
            new ScoringTypeSelection(ScoringGroupDTO.ScoringType_Imp3_NoCorrection,"Butler IMP, no correction of extremes, Avg+=3 IMP"),
            new ScoringTypeSelection(ScoringGroupDTO.ScoringType_Imp3_10Percent,"Butler IMP, omit 10% of extremes, Avg+=3 IMP"),
            new ScoringTypeSelection(ScoringGroupDTO.ScoringType_Imp3_Weighted,"Butler IMP, weighted correction of extremes, Avg+=3 IMP"),
            new ScoringTypeSelection(ScoringGroupDTO.ScoringType_TeamVPContinuous,"Team IMP, recalculation to continuous VPs"),
            new ScoringTypeSelection(ScoringGroupDTO.ScoringType_TeamVPDiscrete,"Team IMP, recalculation to discrete VPs"),
            new ScoringTypeSelection(ScoringGroupDTO.ScoringType_TeamImps,"Team IMP, no recalculation to VPs"),
            new ScoringTypeSelection(ScoringGroupDTO.ScoringType_Patton,"Team IMP + BAM, recalculation to Patton score"),
            new ScoringTypeSelection(ScoringGroupDTO.ScoringType_Bam,"Board-a-match scoring")
        };

    public ScoringTypeSelection(int scoringType, string description)
    {
        ScoringType = scoringType;
        Description = description;
    }
    public string Description { get; set; }
    public int ScoringType { get; set; }    
}
