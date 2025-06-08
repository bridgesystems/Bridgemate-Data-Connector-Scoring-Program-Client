using BridgeSystems.Bridgemate.DataConnectorClasses.SharedDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BridgeSystems.Bridgemate.DataConnector.ScoringProgramClient
{
    public interface IScoringProgramClient
    {
        bool IsActive { get; }
        bool IsSending { get; set; }
        int LastHandrecordQueueItemId { get; }
        int LastParticipantQueueItemId { get; }
        int LastPlayerDataQueueItemId { get; }
        int LastResultQueueItemId { get; }

        ScoringProgramResponse AcceptQueueData(string sessionGuid, DataConnectorResponseData dataType);
        Task<ScoringProgramResponse> AcceptQueueDataAsync(string sessionGuid, DataConnectorResponseData dataType);
        ScoringProgramResponse AddSession(SessionDTO addedSession);
        Task<ScoringProgramResponse> AddSessionAsync(SessionDTO addedSession);
        bool ClearData();
        Task<bool> ClearDataAsync();
        ScoringProgramResponse Connect();
        Task<ScoringProgramResponse> ConnectAsync();
        ScoringProgramResponse Continue(ContinueDTO continueDTO);
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