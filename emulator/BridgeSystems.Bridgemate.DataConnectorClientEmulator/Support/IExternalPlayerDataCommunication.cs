using static BridgeSystems.Bridgemate.DataConnectorClientEmulator.Support.ExternalPlayerDataTextFileCommunicator;

namespace BridgeSystems.Bridgemate.DataConnectorClientEmulator.Support;

public interface IExternalPlayerDataCommunication
{
    ImportedPlayer GetPlayer(int playerID);
    List<ImportedPlayer> GetPlayers();

    List<string> ImportErrors
    {
        get;
    }
}