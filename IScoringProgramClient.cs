using BridgeSystems.Bridgemate.DataConnectorClasses.SharedDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BridgeSystems.Bridgemate.DataConnector.ScoringProgramClient
{
    /// <summary>
    /// Defines a common interface for the <see cref="ScoringProgramDataConnectorPipeClient">ScoringProgramDataConnectorPipeClient</see> and the
    /// <see cref="ScoringProgramDataConnectorHttpClient">ScoringProgramDataConnectorHttpClient</see>.
    /// </summary>
    public interface IScoringProgramClient
    {
        /// <summary>
        /// Signals if the client can be used.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Signals that the client is sending and will not accept any new requests. Try again later.
        /// </summary>
        bool IsSending { get; set; }

        /// <summary>
        /// The id of last handrecord queueitem that was retrieved.
        /// </summary>
        int LastHandrecordQueueItemId { get; }


        /// <summary>
        /// The id of last participant queueitem that was retrieved.
        /// </summary>
        int LastParticipantQueueItemId { get; }


        /// <summary>
        /// The id of last player data queueitem that was retrieved.
        /// </summary>
        int LastPlayerDataQueueItemId { get; }


        /// <summary>
        /// The id of last board result queueitem that was retrieved.
        /// </summary>
        int LastResultQueueItemId { get; }

        /// <summary>
        /// Synchronously accepts the eventqueue data of the given data type.
        /// </summary>
        /// <param name="sessionGuid"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        ScoringProgramResponse AcceptQueueData(string sessionGuid, DataConnectorResponseData dataType);

        /// <summary>
        /// Asynchronously accepts the eventqueue data of the given data type.
        /// </summary>
        /// <param name="sessionGuid"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        Task<ScoringProgramResponse> AcceptQueueDataAsync(string sessionGuid, DataConnectorResponseData dataType);
      
        /// <summary>
        /// Synchronously adds the given session to an existing event.
        /// </summary>
        /// <param name="addedSession"></param>
        /// <returns></returns>
        ScoringProgramResponse AddSession(AddSessionDTO addedSession);

        /// <summary>
        /// Asynchronously adds the given session to an existing event.
        /// </summary>
        /// <param name="addedSession"></param>
        /// <returns></returns>
        Task<ScoringProgramResponse> AddSessionAsync(AddSessionDTO addedSession);
        
        /// <summary>
        /// Synchronously clears all dataconnector data for the club.
        /// </summary>
        /// <returns></returns>
        bool ClearData();

        /// <summary>
        /// Asynchronously clears all dataconnector data for the club.
        /// </summary>
        /// <returns></returns>
        Task<bool> ClearDataAsync();

        /// <summary>
        /// Synchronously attempts to connect to the dataconnector.
        /// </summary>
        /// <returns></returns>
        ScoringProgramResponse Connect();

        /// <summary>
        /// Asynchronously attempts to connect to the dataconnector.
        /// </summary>
        /// <returns></returns>
        Task<ScoringProgramResponse> ConnectAsync();
        
        /// <summary>
        /// Synchronously instructs BCS to continue administering an event it already knows.
        /// </summary>
        /// <param name="continueDTO"></param>
        /// <returns></returns>
        ScoringProgramResponse Continue(ContinueDTO continueDTO);

        /// <summary>
        /// Asynchronously instructs BCS to continue administering an event it already knows.
        /// </summary>
        /// <param name="continueDTO"></param>
        /// <returns></returns>
        Task<ScoringProgramResponse> ContinueAsync(ContinueDTO continueDTO);
        ScoringProgramResponse Disconnect();
        Task<ScoringProgramResponse> DisconnectAsync();
        bool DoesSessionExist(string sessionGuid);
        Task<bool> DoesSessionExistAsync(string sessionGuid);
        SectionDTO[] GetAllMovements(string sessionGuid);
        Task<SectionDTO[]> GetAllMovementsAsync(string sessionGuid);
        SectionDTO GetMovement(string sessionGuid, string sectionLetters);
        Task<SectionDTO> GetMovementAsync(string sessionGuid, string sectionLetters);
        ScoringProgramResponse Initialize(InitDTO initDTO);
        Task<ScoringProgramResponse> InitializeAsync(InitDTO initDTO);
        ScoringProgramResponse IssueManagementCommand(BCSManagementRequestDTO managementDTO);
        Task<ScoringProgramResponse> IssueManagementCommandAsync(BCSManagementRequestDTO managementDTO);
        ScoringProgramResponse Ping();
        Task<ScoringProgramResponse> PingAsync();
        HandrecordDTO[] PollForHandrecords(string sessionGuid, bool all = false);
        Task<HandrecordDTO[]> PollForHandrecordsAsync(string sessionGuid, bool all = false);
        ParticipationDTO[] PollForParticipations(string sessionGuid, bool all = false);
        Task<ParticipationDTO[]> PollForParticipationsAsync(string sessionGuid, bool all = false);
        PlayerDataDTO[] PollForPlayerData(string sessionGuid, bool all = false);
        Task<PlayerDataDTO[]> PollForPlayerDataAsync(string sessionGuid, bool all = false);
        ResultDTO[] PollForResults(string sessionGuid, bool all = false);
        Task<ResultDTO[]> PollForResultsAsync(string sessionGuid, bool all = false);
        ScoringProgramResponse SendBridgemate2Settings(string sessionGuid, Bridgemate2SettingsDTO[] bridgemate2Settings);
        Task<ScoringProgramResponse> SendBridgemate2SettingsAsync(string sessionGuid, Bridgemate2SettingsDTO[] bridgemate2Settings);
        ScoringProgramResponse SendBridgemate3Settings(string sessionGuid, Bridgemate3SettingsDTO[] bridgemate3Settings);
        Task<ScoringProgramResponse> SendBridgemate3SettingsAsync(string sessionGuid, Bridgemate3SettingsDTO[] bridgemate3Settings);
        ScoringProgramResponse SendHandrecords(string sessionGuid, HandrecordDTO[] dtos);
        Task<ScoringProgramResponse> SendHandrecordsAsync(string sessionGuid, HandrecordDTO[] dtos);
        ScoringProgramResponse SendParticipations(string sessionGuid, ParticipationDTO[] dtos);
        Task<ScoringProgramResponse> SendParticipationsAsync(string sessionGuid, ParticipationDTO[] dtos);
        ScoringProgramResponse SendPlayerData(string sessionGuid, PlayerDataDTO[] playerData);
        Task<ScoringProgramResponse> SendPlayerDataAsync(string sessionGuid, PlayerDataDTO[] playerData);
        ScoringProgramResponse SendResults(string sessionGuid, ResultDTO[] dtos);
        Task<ScoringProgramResponse> SendResultsAsync(string sessionGuid, ResultDTO[] dtos);
        ScoringProgramResponse UpdateMovement(SectionUpdateDTO updatedSection);
        Task<ScoringProgramResponse> UpdateMovementAsync(SectionUpdateDTO updatedSection);
        ScoringProgramResponse UpdateScoringGroups(IEnumerable<ScoringGroupDTO> scoringGroups);
        Task<ScoringProgramResponse> UpdateScoringGroupsAsync(IEnumerable<ScoringGroupDTO> scoringGroups);
    }
}