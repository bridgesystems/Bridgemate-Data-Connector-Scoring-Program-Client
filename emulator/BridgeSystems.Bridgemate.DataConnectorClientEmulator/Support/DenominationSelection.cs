using BridgeSystems.Bridgemate.DataConnectorClasses.SharedDTO;

namespace BridgeSystems.Bridgemate.DataConnectorClientEmulator.Support;

public class DenominationSelection
{
    public static List<DenominationSelection> GetDenominationSelectionsList() =>
        new()
        {
            new DenominationSelection(0,"         "),
            new DenominationSelection(ResultDTO.Denomination_Clubs,"Clubs    "),
            new DenominationSelection(ResultDTO.Denomination_Diamonds,"Diamonds "),
            new DenominationSelection(ResultDTO.Denomination_Hearts,"Hearts   "),
            new DenominationSelection(ResultDTO.Denomination_Spades,"Spades   "),
            new DenominationSelection(ResultDTO.Denomination_NoTrump,"No Trump")
        };

    public static List<DenominationSelection> GetSuitSelectionsList() =>
       new()
       {
            new DenominationSelection(0,"        "),
            new DenominationSelection(ResultDTO.Denomination_Clubs,"Clubs"),
            new DenominationSelection(ResultDTO.Denomination_Diamonds,"Diamonds"),
            new DenominationSelection(ResultDTO.Denomination_Hearts,"Hearts"),
            new DenominationSelection(ResultDTO.Denomination_Spades,"Spades")
       };

    public DenominationSelection(int denomination, string description)
    {
        Description = description;
        Denomination = denomination;
    }
    public string Description
    {
        get; set;
    }
    public int Denomination
    {
        get; set;
    }
}
