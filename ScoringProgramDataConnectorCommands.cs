namespace BridgeSystems.Bridgemate.DataConnector.ScoringProgramClient
{
    /// <summary>
    /// These enum values comprise the command values that can be set on the <see cref="ScoringProgramRequest">ScoringProgramRequest</see>'s Command property.
    /// Not all of these are currently supported. In the comments after the values the associated Data Transfer Object (DTO) is mentioned if applicable.
    /// </summary>
    public enum ScoringProgramDataConnectorCommands
    {
        /// <summary>
        /// Invalid, never use
        /// </summary>
        None = 0,                               //Invalid

        /// <summary>
        /// Connect to the Data Connector
        /// </summary>
        Connect,
        /// <summary>
        /// Not supported
        /// </summary>
        StopBCS,                                //Not supported
        /// <summary>
        /// Disconnects from the Data Connector
        /// </summary>
        Disconnect,
        /// <summary>
        /// Checks if the Data Connnector responds
        /// </summary>
        Ping,

        /// <summary>
        /// Initialize an event with at least one session, its scoringgroups, sections, tables and rounds.
        /// </summary>
        InitializeEvent = 5,                    //InitDTO

        /// <summary>
        /// Update the movement for a section, or delete the section.
        /// </summary>
        UpdateMovement,                         //SectionDTO

        /// <summary>
        /// Update the scoringmethod of the scoringgroups and/or rearrange the assignment of the sections to them.
        /// </summary>
        UpdateScoringGroups,                    //ScoringGroupDTO[]

        /// <summary>
        /// Not supported
        /// </summary>
        PutTableGraph,                          //Not supported
        /// <summary>
        /// Upload board results to the Data Connector
        /// </summary>
        PutResults,                             //ResultDTO[]
        /// <summary>
        /// Upload player data  to the Data Connector
        /// </summary>
        PutPlayerData = 10,                     //PlayerDataDTO[]
        /// <summary>
        /// Upload participations to the Data Connector
        /// </summary>
        PutParticipations,                      //ParticipationDTO[]
        /// <summary>
        /// Upload handrecords to the Data Connector
        /// </summary>
        PutHandrecords,                         //HandrecordDTO[],
        /// <summary>
        /// Upload Bridgemate 2 settings to the Data Connector
        /// </summary>
        PutBridgemate2Settings,                 //Bridgemate2SettingsDTO[]
        /// <summary>
        /// Upload Bridgemate 3 settings results to the Data Connector
        /// </summary>
        PutBridgemate3Settings,                 //Bridgemate3SettingsDTOO[]
        /// <summary>
        /// Not supported
        /// </summary>
        UpdateSession = 15,                     //Not supported
        /// <summary>
        /// Not supported
        /// </summary>
        UpdateSection,                          //Not supported
        /// <summary>
        /// Not supported
        /// </summary>
        UpdateTable,                            //Not supported
        /// <summary>
        /// Query the Data Connector for new board results.
        /// </summary>
        PollQueueForNewResults,                 //Returns ResultDTO[]
        /// <summary>
        /// Query the Data Connector for new player data.
        /// </summary>
        PollQueueForNewPlayerData,              //ReturnsPlayerDataDTO
        /// <summary>
        /// Query the Data Connector for new participations.
        /// </summary>
        PollQueueForNewParticipations = 20,     //ReturnsParticipationDTO[]
        /// <summary>
        /// Query the Data Connector for new handrecords.
        /// </summary>
        PollQueueForNewHandrecords,             //Returns HandrecordDTO[]
        /// <summary>
        /// Request all board results for the session from the Data Connector.
        /// </summary>
        PollQueueForAllResults,                 //Returns ResultDTO[]
         /// <summary>
        /// Request all player data for the session from the Data Connector.
        /// </summary>
        PollQueueForAllPlayerData,              //Returns PlayerDataDTO
        /// <summary>
        /// Request all participations results for the session from the Data Connector.
        /// </summary>
        PollQueueForAllParticipations,          //Returns ParticipationDTO[]
        /// <summary>
        /// Request all handrecords for the session from the Data Connector.
        /// </summary>
        PollQueueForAllHandrecords = 25,        //Returns HandrecordDTO[]
        /// <summary>
        /// Accept the new board results: the Data Connector will not send them again, unless the PollQueueForAllResults command is used.
        /// </summary>
        AcceptResultQueueItems,
        /// <summary>
        /// Accept the new player data: the Data Connector will not send them again, unless the PollQueueForAllPlayerData command is used.
        /// </summary>
        AcceptPlayerDataQueueItems,
        /// <summary>
        /// Accept the new participations: the Data Connector will not send them again, unless the PollQueueForAllParticipations command is used.
        /// </summary>
        AcceptParticipantQueueItems,
        /// <summary>
        /// Accept the new handrecords: the Data Connector will not send them again, unless the PollQueueForAllHandrecords command is used.
        /// </summary>
        AcceptHandrecordQueueItems,
        /// <summary>
        /// Continue an event that was start before using the InitializeEvent command.
        /// </summary>
        ContinueEvent = 30,                     //ContinueDTO,
        /// <summary>
        /// Requests the movement for a specific section from the Data Connector.
        /// </summary>
        GetMovement,                            //Returns SectionDTO
        /// <summary>
        /// Requests all movements of a session from the Data Connector
        /// </summary>
        GetAllMovements,                        //Returns SectionDTO[]
        /// <summary>
        /// Queries BCS for either the running session or all known sessions. Also used to request BCS to shut down.
        /// </summary>
        ManageBCS,                              //BCSManagementDTO
        /// <summary>
        /// Not supported
        /// </summary>
        TestData                                //Not supported
    }
}