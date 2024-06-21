
using BridgeSystems.Bridgemate.DataConnectorClasses.SharedDTO;

namespace BridgeSystems.Bridgemate.DataConnector.ScoringProgramClient
{
    /// <summary>
    /// Specifies the data type that the <see cref="ScoringProgramResponse">ScoringProgramResponse</see>'s SerializedData property should contain.
    /// </summary>
    public enum DataConnectorResponseData
    {
        /// <summary>
        /// There is no accompanying data.
        /// </summary>
        None = 0,

        /// <summary>
        /// Acknowledges a command that does not return data
        /// </summary>
        OK,

        /// <summary>
        /// Not in use.
        /// </summary>
        Paused,

        /// <summary>
        /// Signals a non essential error.
        /// </summary>
        Warning,

        /// <summary>
        /// Signals that an error occurred. The accompanying data will be a serialized string explaining the error.
        /// </summary>
        Error,

        /// <summary>
        /// Data to create a new event. An <see cref="InitDTO">InitDTO</see>.
        /// </summary>
        InitData = 5,

        /// <summary>
        /// Data to continue a previously created event. A <see cref="ContinueDTO">ContinueDTO</see>.
        /// </summary>
        ContinueData,

        /// <summary>
        /// Data to update sections and their scoringgroups. A collection of <see cref="ScoringGroupDTO">ScoringGroupDTO</see>.
        /// </summary>
        SectionData,

        /// <summary>
        /// Data to update the BM2 settings. A <see cref="Bridgemate2SettingsDTO">Bridgemate2SettingsDTO</see>.
        /// </summary>
        Bridgemate2Settings,

        /// <summary>
        /// Data to update the BM3 settings. A <see cref="Bridgemate3SettingsDTO">Bridgemate2SettingsDTO</see>.
        /// </summary>
        Bridgemate3Settings,

        /// <summary>
        /// First name, last name, country code for players. A <see cref="PlayerDataDTO">PlayerDataDTO</see>.
        /// </summary>
        PlayerData = 10,

        /// <summary>
        /// Position of players at a table in a round. A <see cref="ParticipationDTO">ParticipationDTO</see>.
        /// </summary>
        Participations,

        /// <summary>
        /// Boardresults. A collection of <see cref="ResultDTO">ResultDTO</see>.
        /// </summary>
        Results,
        /// <summary>
        /// Handrecords. A collection of <see cref="HandrecordDTO">HandrecordDTO</see>.
        /// </summary>
        Handrecords,

        /// <summary>
        /// Data to update the movement for a section. A <see cref="SectionDTO">SectionDTO</see>.
        /// </summary>
        Movement,

        /// <summary>
        /// Sessions, their scoringroups and their sections.
        /// </summary>
        Sessions = 15,

        /// <summary>
        /// Guid-name pairs of the sessions currently being administered.
        /// Embedded in a <see cref="BCSManagementResponseDTO">BCSManagementResponseDTO</see>.
        /// </summary>
        EventInfo,

        /// <summary>
        /// Guid-name-scoringfile location  for all sessions that are present in BCS' data.
        /// Embedded in a <see cref="BCSManagementResponseDTO">BCSManagementResponseDTO</see>.
        /// </summary>
        AllSessionsInfo,

        /// <summary>
        /// The location of the scoring file for BCS.
        /// Embedded in a <see cref="BCSManagementResponseDTO">BCSManagementResponseDTO</see>.
        /// </summary>
        ScoringFileLocation,

        /// <summary>
        /// The data is a shutdown request.
        /// Embedded in a <see cref="BCSManagementRequestDTO">BCSManagementRequesetDTO</see>.
        /// </summary>
        ShutDownRequest, 

        /// <summary>
        /// Currently not supported.
        /// </summary>
        TestData,
       
    }
}