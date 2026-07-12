namespace BridgeSystems.Bridgemate.DataConnectorClientEmulator.Support;

public class CardSelection
{
    public static List<CardSelection> GetCardSelectionList() =>
        new()
        {
            new CardSelection(0,"-    "),
            new CardSelection(2,"2    "),
            new CardSelection(3,"3    "),
            new CardSelection(4,"4    "),
            new CardSelection(5,"5    "),
            new CardSelection(6,"6    "),
            new CardSelection(7,"7    "),
            new CardSelection(8,"8    "),
            new CardSelection(9,"9    "),
            new CardSelection(10,"10   "),
            new CardSelection(11,"Jack "),
            new CardSelection(12,"Queen"),
            new CardSelection(13,"King "),
            new CardSelection(14,"Ace  ")
        };
   
    public CardSelection(int card, string description)
    {
        Description = description;
        Card = card;
    }
    public string Description
    {
        get; set;
    }
    public int Card
    {
        get; set;
    }
}