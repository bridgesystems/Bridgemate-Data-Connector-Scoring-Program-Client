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
        /// The id of last td call queueitem that was retrieved.
        /// </summary>
        int LastTdCallQueueItemId { get; }

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

        /// <summary>
        /// Synchronously disconnects the scoringprogram client from the data connector. Enabling a new connection as only one connection can exist at a time.
        /// </summary>
        /// <returns></returns>
        ScoringProgramResponse Disconnect();

        /// <summary>
        /// Asynchronously disconnects the scoringprogram client from the data connector. Enabling a new connection as only one connection can exist at a time.
        /// </summary>
        /// <returns></returns>
        Task<ScoringProgramResponse> DisconnectAsync();

        /// <summary>
        /// Synchronously checks if the session with the given guid exists.
        /// </summary>
        /// <param name="sessionGuid"></param>
        /// <returns></returns>
        bool DoesSessionExist(string sessionGuid);

        /// <summary>
        /// Asynchronously checks if the session with the given guid exists.
        /// </summary>
        /// <param name="sessionGuid"></param>
        /// <returns></returns>
        Task<bool> DoesSessionExistAsync(string sessionGuid);

        /// <summary>
        /// Synchronously retrieves all movements for the session with the given guid.
        /// </summary>
        /// <param name="sessionGuid">The unique identifier of the session.</param>
        /// <returns>An array of section data transfer objects containing movement information.</returns>
        SectionDTO[] GetAllMovements(string sessionGuid);

        /// <summary>
        /// Asynchronously retrieves all movements for the session with the given guid.
        /// </summary>
        /// <param name="sessionGuid">The unique identifier of the session.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an array of section data transfer objects containing movement information.</returns>
        Task<SectionDTO[]> GetAllMovementsAsync(string sessionGuid);

        /// <summary>
        /// Synchronously retrieves the movement for a specific section within a session.
        /// </summary>
        /// <param name="sessionGuid">The unique identifier of the session.</param>
        /// <param name="sectionLetters">The section letters identifying the specific section.</param>
        /// <returns>A section data transfer object containing movement information.</returns>
        SectionDTO GetMovement(string sessionGuid, string sectionLetters);

        /// <summary>
        /// Asynchronously retrieves the movement for a specific section within a session.
        /// </summary>
        /// <param name="sessionGuid">The unique identifier of the session.</param>
        /// <param name="sectionLetters">The section letters identifying the specific section.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a section data transfer object containing movement information.</returns>
        Task<SectionDTO> GetMovementAsync(string sessionGuid, string sectionLetters);

        /// <summary>
        /// Synchronously initializes a new event with the given initialization data.
        /// </summary>
        /// <param name="initDTO">The initialization data transfer object containing event setup information.</param>
        /// <returns>A response indicating the success or failure of the initialization.</returns>
        ScoringProgramResponse Initialize(InitDTO initDTO);

        /// <summary>
        /// Asynchronously initializes a new event with the given initialization data.
        /// </summary>
        /// <param name="initDTO">The initialization data transfer object containing event setup information.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a response indicating the success or failure of the initialization.</returns>
        Task<ScoringProgramResponse> InitializeAsync(InitDTO initDTO);

        /// <summary>
        /// Synchronously issues a management command to BCS.
        /// </summary>
        /// <param name="managementDTO">The management request data transfer object containing the command details.</param>
        /// <returns>A response indicating the success or failure of the management command.</returns>
        ScoringProgramResponse IssueManagementCommand(BCSManagementRequestDTO managementDTO);

        /// <summary>
        /// Asynchronously issues a management command to BCS.
        /// </summary>
        /// <param name="managementDTO">The management request data transfer object containing the command details.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a response indicating the success or failure of the management command.</returns>
        Task<ScoringProgramResponse> IssueManagementCommandAsync(BCSManagementRequestDTO managementDTO);

        /// <summary>
        /// Synchronously pings the dataconnector to check if it is reachable.
        /// </summary>
        /// <returns>A response indicating the success or failure of the ping.</returns>
        ScoringProgramResponse Ping();

        /// <summary>
        /// Asynchronously pings the dataconnector to check if it is reachable.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a response indicating the success or failure of the ping.</returns>
        Task<ScoringProgramResponse> PingAsync();

        /// <summary>
        /// Synchronously polls for new handrecords from the session with the given guid.
        /// </summary>
        /// <param name="sessionGuid">The unique identifier of the session.</param>
        /// <param name="all">If true, retrieves all handrecords; otherwise, retrieves only new handrecords since the last poll.</param>
        /// <returns>An array of handrecord data transfer objects.</returns>
        HandrecordDTO[] PollForHandrecords(string sessionGuid, bool all = false);

        /// <summary>
        /// Asynchronously polls for new handrecords from the session with the given guid.
        /// </summary>
        /// <param name="sessionGuid">The unique identifier of the session.</param>
        /// <param name="all">If true, retrieves all handrecords; otherwise, retrieves only new handrecords since the last poll.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an array of handrecord data transfer objects.</returns>
        Task<HandrecordDTO[]> PollForHandrecordsAsync(string sessionGuid, bool all = false);

        /// <summary>
        /// Synchronously polls for new participations from the session with the given guid.
        /// </summary>
        /// <param name="sessionGuid">The unique identifier of the session.</param>
        /// <param name="all">If true, retrieves all participations; otherwise, retrieves only new participations since the last poll.</param>
        /// <returns>An array of participation data transfer objects.</returns>
        ParticipationDTO[] PollForParticipations(string sessionGuid, bool all = false);

        /// <summary>
        /// Asynchronously polls for new participations from the session with the given guid.
        /// </summary>
        /// <param name="sessionGuid">The unique identifier of the session.</param>
        /// <param name="all">If true, retrieves all participations; otherwise, retrieves only new participations since the last poll.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an array of participation data transfer objects.</returns>
        Task<ParticipationDTO[]> PollForParticipationsAsync(string sessionGuid, bool all = false);

        /// <summary>
        /// Synchronously polls for new player data from the session with the given guid.
        /// </summary>
        /// <param name="sessionGuid">The unique identifier of the session.</param>
        /// <param name="all">If true, retrieves all player data; otherwise, retrieves only new player data since the last poll.</param>
        /// <returns>An array of player data transfer objects.</returns>
        PlayerDataDTO[] PollForPlayerData(string sessionGuid, bool all = false);

        /// <summary>
        /// Asynchronously polls for new player data from the session with the given guid.
        /// </summary>
        /// <param name="sessionGuid">The unique identifier of the session.</param>
        /// <param name="all">If true, retrieves all player data; otherwise, retrieves only new player data since the last poll.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an array of player data transfer objects.</returns>
        Task<PlayerDataDTO[]> PollForPlayerDataAsync(string sessionGuid, bool all = false);

        /// <summary>
        /// Synchronously polls for new results from the session with the given guid.
        /// </summary>
        /// <param name="sessionGuid">The unique identifier of the session.</param>
        /// <param name="all">If true, retrieves all results; otherwise, retrieves only new results since the last poll.</param>
        /// <returns>An array of result data transfer objects.</returns>
        ResultDTO[] PollForResults(string sessionGuid, bool all = false);

        /// <summary>
        /// Asynchronously polls for new results from the session with the given guid.
        /// </summary>
        /// <param name="sessionGuid">The unique identifier of the session.</param>
        /// <param name="all">If true, retrieves all results; otherwise, retrieves only new results since the last poll.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an array of result data transfer objects.</returns>
        Task<ResultDTO[]> PollForResultsAsync(string sessionGuid, bool all = false);

        /// <summary>
        /// Synchronously polls for new TD calls from the session with the given guid.
        /// </summary>
        /// <param name="sessionGuid">The unique identifier of the session.</param>
        /// <param name="all">If true, retrieves all TD calls; otherwise, retrieves only new TD calls since the last poll.</param>
        /// <returns>An array of TD call data transfer objects.</returns>
        TdCallDTO[] PollForTdCalls(string sessionGuid, bool all = false);

        /// <summary>
        /// Asynchronously polls for new TD calls from the session with the given guid.
        /// </summary>
        /// <param name="sessionGuid">The unique identifier of the session.</param>
        /// <param name="all">If true, retrieves all TD calls; otherwise, retrieves only new TD calls since the last poll.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an array of TD call data transfer objects.</returns>
        Task<TdCallDTO[]> PollForTdCallsAsync(string sessionGuid, bool all = false);

        /// <summary>
        /// Synchronously sends Bridgemate 2 settings to the dataconnector for the given session.
        /// </summary>
        /// <param name="sessionGuid">The unique identifier of the session.</param>
        /// <param name="bridgemate2Settings">An array of Bridgemate 2 settings data transfer objects.</param>
        /// <returns>A response indicating the success or failure of the operation.</returns>
        ScoringProgramResponse SendBridgemate2Settings(string sessionGuid, Bridgemate2SettingsDTO[] bridgemate2Settings);

        /// <summary>
        /// Asynchronously sends Bridgemate 2 settings to the dataconnector for the given session.
        /// </summary>
        /// <param name="sessionGuid">The unique identifier of the session.</param>
        /// <param name="bridgemate2Settings">An array of Bridgemate 2 settings data transfer objects.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a response indicating the success or failure of the operation.</returns>
        Task<ScoringProgramResponse> SendBridgemate2SettingsAsync(string sessionGuid, Bridgemate2SettingsDTO[] bridgemate2Settings);

        /// <summary>
        /// Synchronously sends Bridgemate 3 settings to the dataconnector for the given session.
        /// </summary>
        /// <param name="sessionGuid">The unique identifier of the session.</param>
        /// <param name="bridgemate3Settings">An array of Bridgemate 3 settings data transfer objects.</param>
        /// <returns>A response indicating the success or failure of the operation.</returns>
        ScoringProgramResponse SendBridgemate3Settings(string sessionGuid, Bridgemate3SettingsDTO[] bridgemate3Settings);

        /// <summary>
        /// Asynchronously sends Bridgemate 3 settings to the dataconnector for the given session.
        /// </summary>
        /// <param name="sessionGuid">The unique identifier of the session.</param>
        /// <param name="bridgemate3Settings">An array of Bridgemate 3 settings data transfer objects.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a response indicating the success or failure of the operation.</returns>
        Task<ScoringProgramResponse> SendBridgemate3SettingsAsync(string sessionGuid, Bridgemate3SettingsDTO[] bridgemate3Settings);

        /// <summary>
        /// Synchronously sends handrecords to the dataconnector for the given session.
        /// </summary>
        /// <param name="sessionGuid">The unique identifier of the session.</param>
        /// <param name="dtos">An array of handrecord data transfer objects.</param>
        /// <returns>A response indicating the success or failure of the operation.</returns>
        ScoringProgramResponse SendHandrecords(string sessionGuid, HandrecordDTO[] dtos);

        /// <summary>
        /// Asynchronously sends handrecords to the dataconnector for the given session.
        /// </summary>
        /// <param name="sessionGuid">The unique identifier of the session.</param>
        /// <param name="dtos">An array of handrecord data transfer objects.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a response indicating the success or failure of the operation.</returns>
        Task<ScoringProgramResponse> SendHandrecordsAsync(string sessionGuid, HandrecordDTO[] dtos);

        /// <summary>
        /// Synchronously sends participations to the dataconnector for the given session.
        /// </summary>
        /// <param name="sessionGuid">The unique identifier of the session.</param>
        /// <param name="dtos">An array of participation data transfer objects.</param>
        /// <returns>A response indicating the success or failure of the operation.</returns>
        ScoringProgramResponse SendParticipations(string sessionGuid, ParticipationDTO[] dtos);

        /// <summary>
        /// Asynchronously sends participations to the dataconnector for the given session.
        /// </summary>
        /// <param name="sessionGuid">The unique identifier of the session.</param>
        /// <param name="dtos">An array of participation data transfer objects.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a response indicating the success or failure of the operation.</returns>
        Task<ScoringProgramResponse> SendParticipationsAsync(string sessionGuid, ParticipationDTO[] dtos);

        /// <summary>
        /// Synchronously sends player data to the dataconnector for the given session.
        /// </summary>
        /// <param name="sessionGuid">The unique identifier of the session.</param>
        /// <param name="playerData">An array of player data transfer objects.</param>
        /// <returns>A response indicating the success or failure of the operation.</returns>
        ScoringProgramResponse SendPlayerData(string sessionGuid, PlayerDataDTO[] playerData);

        /// <summary>
        /// Asynchronously sends player data to the dataconnector for the given session.
        /// </summary>
        /// <param name="sessionGuid">The unique identifier of the session.</param>
        /// <param name="playerData">An array of player data transfer objects.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a response indicating the success or failure of the operation.</returns>
        Task<ScoringProgramResponse> SendPlayerDataAsync(string sessionGuid, PlayerDataDTO[] playerData);

        /// <summary>
        /// Synchronously sends results to the dataconnector for the given session.
        /// </summary>
        /// <param name="sessionGuid">The unique identifier of the session.</param>
        /// <param name="dtos">An array of result data transfer objects.</param>
        /// <returns>A response indicating the success or failure of the operation.</returns>
        ScoringProgramResponse SendResults(string sessionGuid, ResultDTO[] dtos);

        /// <summary>
        /// Synchronously sends TD calls to the dataconnector for the given session.
        /// </summary>
        /// <param name="sessionGuid">The unique identifier of the session.</param>
        /// <param name="dtos">An array of TD call data transfer objects.</param>
        /// <returns>A response indicating the success or failure of the operation.</returns>
        ScoringProgramResponse SendTdCalls(string sessionGuid, TdCallDTO[] dtos);

        /// <summary>
        /// Asynchronously sends TD calls to the dataconnector for the given session.
        /// </summary>
        /// <param name="sessionGuid">The unique identifier of the session.</param>
        /// <param name="dtos">An array of TD call data transfer objects.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a response indicating the success or failure of the operation.</returns>
        Task<ScoringProgramResponse> SendTdCallsAsync(string sessionGuid, TdCallDTO[] dtos);

        /// <summary>
        /// Asynchronously sends results to the dataconnector for the given session.
        /// </summary>
        /// <param name="sessionGuid">The unique identifier of the session.</param>
        /// <param name="dtos">An array of result data transfer objects.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a response indicating the success or failure of the operation.</returns>
        Task<ScoringProgramResponse> SendResultsAsync(string sessionGuid, ResultDTO[] dtos);

        /// <summary>
        /// Synchronously updates the movement for a specific section.
        /// </summary>
        /// <param name="updatedSection">The section update data transfer object containing the updated movement information.</param>
        /// <returns>A response indicating the success or failure of the update operation.</returns>
        ScoringProgramResponse UpdateMovement(SectionUpdateDTO updatedSection);

        /// <summary>
        /// Asynchronously updates the movement for a specific section.
        /// </summary>
        /// <param name="updatedSection">The section update data transfer object containing the updated movement information.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a response indicating the success or failure of the update operation.</returns>
        Task<ScoringProgramResponse> UpdateMovementAsync(SectionUpdateDTO updatedSection);

        /// <summary>
        /// Synchronously updates the scoring groups for the current event.
        /// </summary>
        /// <param name="scoringGroups">A collection of scoring group data transfer objects.</param>
        /// <returns>A response indicating the success or failure of the update operation.</returns>
        ScoringProgramResponse UpdateScoringGroups(IEnumerable<ScoringGroupDTO> scoringGroups);

        /// <summary>
        /// Asynchronously updates the scoring groups for the current event.
        /// </summary>
        /// <param name="scoringGroups">A collection of scoring group data transfer objects.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a response indicating the success or failure of the update operation.</returns>
        Task<ScoringProgramResponse> UpdateScoringGroupsAsync(IEnumerable<ScoringGroupDTO> scoringGroups);
    }
}