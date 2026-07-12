using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using CsvHelper;

namespace BridgeSystems.Bridgemate.DataConnectorClientEmulator.Support;

public class ExternalPlayerDataTextFileCommunicator : IExternalPlayerDataCommunication
{
    public class ImportedPlayer
    {
        public string PlayerNumberId
        {
            get; set;
        }
        public string FirstName
        {
            get; set;
        }
        public string LastName
        {
            get; set;
        }
        public string CountryCode
        {
            get; set;
        }

    }

    public ExternalPlayerDataTextFileCommunicator(Func<string> getFilePath)
    {
        Players = new List<ImportedPlayer>();
        ImportErrors = new List<string>();
        _getFilePath = getFilePath;
    }

    private readonly Func<string> _getFilePath;
    protected Func<string> GetFilePath => _getFilePath;

    ImportedPlayer IExternalPlayerDataCommunication.GetPlayer(int playerID)
    {
        try
        {
            if (!Players.Any())
            {
                ReadData();
            }

            var player = Players.FirstOrDefault(p => int.TryParse(p.PlayerNumberId, out var id) && id == playerID);
            return player;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    List<ImportedPlayer> IExternalPlayerDataCommunication.GetPlayers()
    {
        try
        {
            var result = ReadData();
            Players = result.players;
            ImportErrors = result.errors;
            return Players;
        }
        catch (Exception ex)
        {
            return new List<ImportedPlayer>();
        }
    }

    protected virtual (List<string> errors, List<ImportedPlayer> players) ReadData()
    {
        ImportErrors.Clear();
        using var reader = new StreamReader(_getFilePath());
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var records = new List<ImportedPlayer>();
        records = csv.GetRecords<ImportedPlayer>().ToList();
        Players = records.ToList();
        return (new List<string>(), Players);
    }

    public List<ImportedPlayer> Players
    {
        get; private set;
    }

    public List<string> ImportErrors
    {
        get; private set;
    }
}
