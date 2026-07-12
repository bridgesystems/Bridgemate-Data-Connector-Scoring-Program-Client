using BridgeSystems.Bridgemate.DataConnectorClasses.SharedDTO;

namespace BridgeSystems.Bridgemate.DataConnectorClientEmulator.Support;

public class SpecialResultSelection
{
    public static List<SpecialResultSelection> GetSpecialResultSelectionsList() =>
        new()
        {
          new SpecialResultSelection(ResultDTO.ContractLevel_Pass,"Pass"),
          new SpecialResultSelection(ResultDTO.ContractLevel_NoPLay,"No Play"),
          new SpecialResultSelection(ResultDTO.AvgMinMin,"A-/A-"),
          new SpecialResultSelection(ResultDTO.AvgMinAvg,"A-/A"),
          new SpecialResultSelection(ResultDTO.AvgMinPlus,"A-/A+"),
          new SpecialResultSelection(ResultDTO.AvgAvgMin,"A/A-"),
          new SpecialResultSelection(ResultDTO.AvgAvgAvg,"A/A"),
          new SpecialResultSelection(ResultDTO.AvgAvgPlus,"A/A+"),
          new SpecialResultSelection(ResultDTO.AvgPlusMin,"A+/A-"),
          new SpecialResultSelection(ResultDTO.AvgPlusAvg,"A+/A"),
          new SpecialResultSelection(ResultDTO.AvgPlusPlus,"A+/A+"),
        };
    public SpecialResultSelection(int specialResultValue, string description)
    {
        Description = description;
        SpecialResultValue = specialResultValue;
    }
    public string Description
    {
        get; set;
    }
    public int SpecialResultValue
    {
        get; set;
    }
}
