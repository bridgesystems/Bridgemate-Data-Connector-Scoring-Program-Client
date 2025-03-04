﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using BridgeSystems.Bridgemate.DataConnector.ScoringProgramClient.DataConnector;
using BridgeSystems.Bridgemate.DataConnectorClasses.SharedDTO;
using NLog;
using static BridgeSystems.Bridgemate.DataConnector.ScoringProgramClient.DataConnectorLogCreator<BridgeSystems.Bridgemate.DataConnector.ScoringProgramClient.ScoringProgramDataConnectorCommands>;

namespace BridgeSystems.Bridgemate.DataConnector.ScoringProgramClient
{

    /// <summary>
    /// Handles request from the scoring programs. This code is freely available for external programmers to adapt to their
    /// specific needs. This code, however, is guaranteed to work. Interop options are available for many programming languages.
    /// <br/>
    /// Note that this class is written as a singleton: there should never be more than one instance communicating over the pipe. 
    /// The constructor is protected however, so by inheriting from this class a non singleton instance can be used.
    /// <br/>
    /// The class implements IDisopable through its base class, as the base class contains a stream writer and stream reader that must be disposed
    /// when the communication shuts down. Be sure to call Dispose on the class any time it is no longer in use, otherwise communication will stall.
    /// <br/>
    /// The code has no dependencies, except one on NLog (and Net Standard 2.0). If you decide to copy the code make sure to add a dependency to the NLog NuGet package
    /// or implement your own logging.
    /// <br/>
    /// All public functions have synchronous and asynchronous implementations (ending in "Async").
    /// <br/>
    /// To get started inspect the following functions:<br/>
    /// <see cref="ScoringProgramPipeClient.Connect">Connect</see> and <see cref="ScoringProgramPipeClient.ConnectAsync">ConnectAsync</see>: Connection to the Data Connector.<br/>
    /// <see cref="ScoringProgramPipeClient.Initialize(InitDTO)">Initialize</see> and <see cref="ScoringProgramPipeClient.InitializeAsync(InitDTO)">InitializeAsync</see>: Initialization of a new event. <br/>
    /// <see cref="IssueManagementCommand(BCSManagementRequestDTO)">IssueManagementCommand</see> and <see cref="IssueManagementCommandAsync(BCSManagementRequestDTO)">IssueManagementCommandAsync</see>: Query the Bridgemate Control Software.
    /// </summary>
    public class ScoringProgramPipeClient : DataConnectorPipeClient<ScoringProgramDataConnectorCommands>
    {
        /// <summary>
        /// Cached values of the ids of the several types of eventqueue items that have been sent to the Data Connector.
        /// If the scoring program does not include the id of the last received queue item when accepting them,the cached values will be used.
        /// </summary>      
        private int _lastResultQueueItemId;         //A cached value of the id of the last sent board result queue item.
        private int _lastPlayeDataQueueItemId;      //A cached value of the id of the last sent participant queue item.
        private int _lastParticipantQueueItemId;    //A cached value of the id of the last sent participant queue item.
        private int _lastHandrecordQueueItemId;     //A cached value of the id of the last sent handrecord queue item.

        /// <summary>
        /// The logger for this class.
        /// </summary>
        public static readonly Logger ScoringProgramClientLogger = LogManager.GetLogger(nameof(ScoringProgramClientLogger));

        /// <summary>
        /// The source of the logging records.
        /// </summary>
        protected override DataConnectorLoggingSource LoggingSource => DataConnectorLoggingSource.ScoringProgramClient;

        /// <summary>
        /// The name of the pipe that handles the bidirectional communication with the Data Connector.
        /// Each Windows account must have its own pipe, hence the username of the logged in user (as Windows knows it) is appended.
        /// Tip: you can find the usernames as Windows knows them by going to the C:\Users folder.
        /// </summary>
        public string PipeName = $"BridgeSystems.Bridgemate.DataConnectorService.ScoringProgram.{Environment.UserName}";

        /// <summary>
        /// The implementation of the singleton pattern. Atypically the constructor is protected rather than private.
        /// </summary>
        private static ScoringProgramPipeClient _instance;

        /// <summary>
        /// The single instance of the pipe client. Use this property to retrieve the client and use it in the external program.
        /// </summary>
        public static ScoringProgramPipeClient Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ScoringProgramPipeClient();
                return _instance;
            }
        }

        /// <summary>
        /// Initializes the class.
        /// </summary>
        protected ScoringProgramPipeClient()
        {
        }
        // End of singleton implementation

        /// <summary>
        /// The id of the last participant queue item downloaded from the Data Connector.
        /// You can use this to internally accept this item (and all before of the same type).
        /// </summary>
        public int LastParticipantQueueItemId => _lastParticipantQueueItemId;

        /// <summary>
        /// The id of the last player data queue item downloaded from the Data Connector.
        /// You can use this to internally accept this item (and all before of the same type).
        /// </summary>
        public int LastPlayerDataQueueItemId => _lastPlayeDataQueueItemId;

        /// <summary>
        /// The id of the last handrecord queue item downloaded from the Data Connector.
        /// You can use this to internally accept this item (and all before of the same type).
        /// </summary>
        public int LastHandrecordQueueItemId => _lastHandrecordQueueItemId;

        /// <summary>
        /// The id of the last result queue item downloaded from the Data Connector.
        /// You can use this to internally accept this item (and all before of the same type).
        /// </summary>
        public int LastResultQueueItemId => _lastResultQueueItemId;

        private bool _isSending;
        /// <summary>
        /// Only one request can be sent at the same time. So let asynchronous code always check, set and reset this
        /// property before, while and after sending a request. Feel free to make more nifty implementations.
        /// </summary>
        public bool IsSending
        {
            get => _isSending;
            set => _isSending = value;
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="ex"></param>
        protected override void LogError(Exception ex)
        {
            ScoringProgramClientLogger.Error(ex);
        }

        /// <summary>
        /// Logs the entry to a method with its parameters (if any).
        /// </summary>
        /// <param name="entry"></param>
        protected override void LogMethodEntry(string entry)
        {
            ScoringProgramClientLogger.Debug(entry);
        }

        /// <summary>
        /// Connect to the Data Connector asynchronously.
        /// </summary>
        /// <returns></returns>
        public async Task<ScoringProgramResponse> ConnectAsync()
        {
            try
            {
                (DataConnectorResponseData result, string message, ErrorType errorType) result;
                var success = BridgemateDataConnectorManager.EnsureDataConnectorServiceIsRunning(forceRestart: false);
                if (!success)
                {
                    return new ScoringProgramResponse
                    {
                        RequestCommand = ScoringProgramDataConnectorCommands.Connect,
                        DataType = DataConnectorResponseData.Error,
                        ErrorType = ErrorType.NoConnection,
                        SerializedData = JsonSerializer.Serialize("The Bridgemate Data Connector is not running and could not be restarted.")
                    };
                }
                result = await ConnectAsync(PipeName);
                return new ScoringProgramResponse
                {
                    RequestCommand = ScoringProgramDataConnectorCommands.Connect,
                    DataType = result.result,
                    ErrorType = result.errorType,
                    SerializedData = JsonSerializer.Serialize(result.message)
                };
            }
            catch (Exception ex)
            {
                LogError(ex);
                return new ScoringProgramResponse
                {
                    RequestCommand = ScoringProgramDataConnectorCommands.Connect,
                    DataType = DataConnectorResponseData.Error,
                    SerializedData = JsonSerializer.Serialize(ex.Message)
                };
            }
        }

        /// <summary>
        /// Connect to the Data Connector synchronously
        /// </summary>
        /// <returns></returns>
        public ScoringProgramResponse Connect()
        {
            (DataConnectorResponseData result, string message, ErrorType errorType) result;
            var success = BridgemateDataConnectorManager.EnsureDataConnectorServiceIsRunning(forceRestart: false);
            if (!success)
            {
                return new ScoringProgramResponse
                {
                    RequestCommand = ScoringProgramDataConnectorCommands.Connect,
                    DataType = DataConnectorResponseData.Error,
                    ErrorType = ErrorType.NoConnection,
                    SerializedData = JsonSerializer.Serialize("The Bridgemate Data Connector is not running and could not be restarted.")
                };
            }
            try
            {
                result = Connect(PipeName);
                return new ScoringProgramResponse
                {
                    RequestCommand = ScoringProgramDataConnectorCommands.Connect,
                    DataType = result.result,
                    ErrorType = result.errorType,
                    SerializedData = JsonSerializer.Serialize(result.message)
                };
            }
            catch (Exception ex)
            {
                LogError(ex);
                return new ScoringProgramResponse
                {
                    RequestCommand = ScoringProgramDataConnectorCommands.Connect,
                    DataType = DataConnectorResponseData.Error,
                    SerializedData = JsonSerializer.Serialize(ex.Message)
                };
            }
        }

        /// <summary>
        /// Disconnects from the Data Connector asynchronously, causing the Data Connector to close the pipe and reopen it.
        /// </summary>
        /// <returns></returns>
        public async Task<ScoringProgramResponse> DisconnectAsync()
        {
            LogMethodEntry(nameof(DisconnectAsync));

            //The pipe only supports one request being sent at the same time.
            if (_isSending)
            {
                return new ScoringProgramResponse
                {
                    RequestCommand = ScoringProgramDataConnectorCommands.Disconnect,
                    DataType = DataConnectorResponseData.Error,
                    ErrorType = ErrorType.Busy,
                    SerializedData = JsonSerializer.Serialize("Data is being sent from another command. Try again later.")
                };
            }
            try
            {
                if (DataConnectorWriter != null)
                {
                    var request = new ScoringProgramRequest { Command = ScoringProgramDataConnectorCommands.Disconnect };
                    var requestSerialized = JsonSerializer.Serialize(request);
                    await DataConnectorWriter.WriteLineAsync(requestSerialized);
                    CloseWriter();
                }
                if (DataConnectorReader != null)
                {
                    try
                    {
                        //Usually the code crashses on the line below because the connection has died. So that is expected behaviour.
                        //If an answer comes it will be an error message.
                        var response = await DataConnectorReader.ReadLineAsync();
                        if (response != null)
                        {
                            var responseDeserialized = JsonSerializer.Deserialize<ScoringProgramResponse>(response);
                        }
                    }
                    catch
                    {
                    }
                    CloseReader();
                }
                CloseConnection();
                return new ScoringProgramResponse
                {
                    RequestCommand = ScoringProgramDataConnectorCommands.Disconnect,
                    DataType = DataConnectorResponseData.OK,
                    SerializedData = "Data Connector stopped."
                };
            }
            catch (Exception ex)
            {
                LogError(ex);
                return new ScoringProgramResponse
                {
                    RequestCommand = ScoringProgramDataConnectorCommands.Disconnect,
                    DataType = DataConnectorResponseData.Error,
                    SerializedData = ex.ToString()
                };
            }
            finally { CloseWriter(); }
        }

        /// <summary>
        /// Disconnects from the Data Connector synchronously, causing the Data Connector to close the pipe and reopen it.
        /// </summary>
        /// <returns></returns>
        public ScoringProgramResponse Disconnect()
        {
            LogMethodEntry(nameof(Disconnect));

            //Do not proceed if sending is already in progress (for an other request). There can be only on request be sent at the same time.
            if (_isSending)
            {
                return new ScoringProgramResponse
                {
                    RequestCommand = ScoringProgramDataConnectorCommands.Disconnect,
                    DataType = DataConnectorResponseData.Error,
                    ErrorType = ErrorType.Busy,
                    SerializedData = JsonSerializer.Serialize("Data is being sent from another command. Try again later.")
                };
            }
            try
            {
                if (DataConnectorWriter != null)
                {
                    var request = new ScoringProgramRequest { Command = ScoringProgramDataConnectorCommands.Disconnect };
                    var requestSerialized = JsonSerializer.Serialize(request);
                    DataConnectorWriter.WriteLine(requestSerialized);
                    CloseWriter();
                }
                if (DataConnectorReader != null)
                {
                    try
                    {
                        //Usually the code crashses on the line below because the connection has died. So that is expected behaviour.
                        //If an answer comes it will be an error message.
                        var response = DataConnectorReader.ReadLine();
                        if (response != null)
                        {
                            var responseDeserialized = JsonSerializer.Deserialize<ScoringProgramResponse>(response);
                        }
                    }
                    catch
                    {
                    }
                    CloseReader();
                }
                CloseConnection();
                return new ScoringProgramResponse
                {
                    RequestCommand = ScoringProgramDataConnectorCommands.Disconnect,
                    DataType = DataConnectorResponseData.OK,
                    SerializedData = "Data Connector stopped."
                };
            }
            catch (Exception ex)
            {
                LogError(ex);
                return new ScoringProgramResponse
                {
                    RequestCommand = ScoringProgramDataConnectorCommands.Disconnect,
                    DataType = DataConnectorResponseData.Error,
                    SerializedData = ex.ToString()
                };
            }
            finally { CloseWriter(); }
        }

        /// <summary>
        /// Communicates to the Data Connector asynchronously to see if it is responsive.
        /// This is done by sending a random piece of data and checking of the Data Connector returns it.
        /// </summary>
        /// <returns></returns>
        public async Task<ScoringProgramResponse> PingAsync()
        {
            LogMethodEntry(nameof(PingAsync));

            var requestTicks = DateTime.Now.Ticks.ToString();
            var serializedTicks = JsonSerializer.Serialize(requestTicks);
            var response = await SendDataAsync(sessionGuid: string.Empty, ScoringProgramDataConnectorCommands.Ping, serializedTicks);
            if (response.RequestCommand != ScoringProgramDataConnectorCommands.Ping)
            {
                return new ScoringProgramResponse
                {
                    RequestCommand = ScoringProgramDataConnectorCommands.Ping,
                    DataType = DataConnectorResponseData.Error,
                    ErrorType = ErrorType.Unknown,
                    SerializedData = JsonSerializer.Serialize($"Invalid command in reponse to {nameof(ScoringProgramDataConnectorCommands.Ping)}: " +
                                                              $"'{response.RequestCommand}'")
                };
            }
            if (response.DataType != DataConnectorResponseData.OK)
                return response;

            var responseTicks = JsonSerializer.Deserialize<string>(response.SerializedData);
            var error = responseTicks != requestTicks;
            return new ScoringProgramResponse
            {
                RequestCommand = ScoringProgramDataConnectorCommands.Ping,
                DataType = error ? DataConnectorResponseData.Error : DataConnectorResponseData.OK,
                ErrorType = error ? ErrorType.Validation : ErrorType.None,
                SerializedData = response.SerializedData
            };
        }

        /// <summary>
        /// Communicates to the Data Connector synchronously to see if it is responsive.
        /// This is done by sending a random piece of data and checking of the Data Connector returns it.
        /// </summary>
        /// <returns></returns>
        public ScoringProgramResponse Ping()
        {
            LogMethodEntry(nameof(Ping));

            var requestTicks = DateTime.Now.Ticks.ToString();
            var serializedTicks = JsonSerializer.Serialize(requestTicks);
            var response = SendData(sessionGuid: string.Empty, ScoringProgramDataConnectorCommands.Ping, serializedTicks);
            if (response.RequestCommand != ScoringProgramDataConnectorCommands.Ping)
            {
                return new ScoringProgramResponse
                {
                    RequestCommand = ScoringProgramDataConnectorCommands.Ping,
                    DataType = DataConnectorResponseData.Error,
                    ErrorType = ErrorType.Unknown,
                    SerializedData = JsonSerializer.Serialize($"Invalid command in reponse to {nameof(ScoringProgramDataConnectorCommands.Ping)}: " +
                                                              $"'{response.RequestCommand}'")
                };
            }
            if (response.DataType != DataConnectorResponseData.OK)
                return response;

            var responseTicks = JsonSerializer.Deserialize<string>(response.SerializedData);
            var error = responseTicks != requestTicks;
            return new ScoringProgramResponse
            {
                RequestCommand = ScoringProgramDataConnectorCommands.Ping,
                DataType = error ? DataConnectorResponseData.Error : DataConnectorResponseData.OK,
                ErrorType = error ? ErrorType.Validation : ErrorType.None,
                SerializedData = response.SerializedData
            };
        }

        /// <summary>
        /// Instructs BCS to create new sessions with the provided table domain data (scoring groups, sections, tables, rounds). 
        /// Player names, Results and Handrecords can be included or can be uploaded later. The latter option is considerably less performant.
        /// </summary>
        /// <param name="initDTO">Carries the initialization data.</param>
        /// <returns></returns>
        public Task<ScoringProgramResponse> InitializeAsync(InitDTO initDTO)
        {
            LogMethodEntry(nameof(InitializeAsync));
            var serializedData = JsonSerializer.Serialize(initDTO);
            return SendDataAsync("", ScoringProgramDataConnectorCommands.InitializeEvent, serializedData);
        }

        /// <summary>
        /// Instructs BCS to create a new sessions with the provided table domain data (scoring groups, sections, tables, rounds). 
        /// Player names, Results and Handrecords can be included or can be uploaded later. The latter option is considerably less performant.
        /// </summary>
        /// <param name="initDTO">Carries the initialization data.</param>
        /// <returns></returns>
        public ScoringProgramResponse Initialize(InitDTO initDTO)
        {
            LogMethodEntry(nameof(Initialize));

            var serializedData = JsonSerializer.Serialize(initDTO);
            return SendData("", ScoringProgramDataConnectorCommands.InitializeEvent, serializedData);
        }

        /// <summary>
        /// Instructs BCS asynchronously to continue working with a previously created event.
        /// </summary>
        /// <param name="continueDTO">A DTO containing the guid for the event that must be administered again.</param>
        /// <returns></returns>
        public Task<ScoringProgramResponse> ContinueAsync(ContinueDTO continueDTO)
        {
            LogMethodEntry(nameof(ContinueAsync));

            var serializedData = JsonSerializer.Serialize(continueDTO);
            return SendDataAsync("", ScoringProgramDataConnectorCommands.ContinueEvent, serializedData);
        }

        /// <summary>
        /// Instructs BCS  synchronously to continue working with a previously created event.
        /// </summary>
        /// <param name="continueDTO">A DTO containing the guid for the event that must be administered again.</param>
        /// <returns></returns>
        public ScoringProgramResponse Continue(ContinueDTO continueDTO)
        {
            LogMethodEntry(nameof(Continue));

            var serializedData = JsonSerializer.Serialize(continueDTO);
            return SendData("", ScoringProgramDataConnectorCommands.ContinueEvent, serializedData);
        }

        /// <summary>
        /// Sends boardresults asynchronously to the BCS queue
        /// </summary>
        /// <param name="dtos"></param>
        /// <param name="sessionGuid"></param>
        /// <returns></returns>
        public async Task<ScoringProgramResponse> SendResultsAsync(string sessionGuid, ResultDTO[] dtos)
        {
            LogMethodEntry(nameof(SendResultsAsync), (nameof(sessionGuid), sessionGuid));

            var dtosForSession = dtos.Where(dto => dto.SessionGuid == sessionGuid).ToList();
            var serializedData = JsonSerializer.Serialize(dtosForSession);
            return await SendDataAsync(sessionGuid, ScoringProgramDataConnectorCommands.PutResults, serializedData);
        }

        /// <summary>
        /// Sends boardresults synchronously to the BCS queue
        /// </summary>
        /// <param name="dtos"></param>
        /// <param name="sessionGuid"></param>
        /// <returns></returns>
        public ScoringProgramResponse SendResults(string sessionGuid, ResultDTO[] dtos)
        {
            LogMethodEntry(nameof(SendResults), (nameof(sessionGuid), sessionGuid));

            var dtosForSession = dtos.Where(dto => dto.SessionGuid == sessionGuid).ToList();
            var serializedData = JsonSerializer.Serialize(dtosForSession);
            return SendData(sessionGuid, ScoringProgramDataConnectorCommands.PutResults, serializedData);
        }

        /// <summary>
        /// Sends handrecords asynchronously to the BCS queue.
        /// </summary>
        /// <param name="sessionGuid"></param>
        /// <param name="dtos"></param>
        /// <returns></returns>
        public Task<ScoringProgramResponse> SendHandrecordsAsync(string sessionGuid, HandrecordDTO[] dtos)
        {
            LogMethodEntry(nameof(SendHandrecordsAsync), (nameof(sessionGuid), sessionGuid));

            var dtosForSession = dtos.Where(dto => dto.SessionGuid == sessionGuid).ToList();
            var serializedData = JsonSerializer.Serialize(dtosForSession);
            return SendDataAsync(sessionGuid, ScoringProgramDataConnectorCommands.PutHandrecords, serializedData);
        }

        /// <summary>
        /// Sends handrecords synchronously to the BCS queue.
        /// </summary>
        /// <param name="sessionGuid"></param>
        /// <param name="dtos"></param>
        /// <returns></returns>
        public ScoringProgramResponse SendHandrecords(string sessionGuid, HandrecordDTO[] dtos)
        {
            LogMethodEntry(nameof(SendHandrecords), (nameof(sessionGuid), sessionGuid));

            var dtosForSession = dtos.Where(dto => dto.SessionGuid == sessionGuid).ToList();
            var serializedData = JsonSerializer.Serialize(dtosForSession);
            return SendData(sessionGuid, ScoringProgramDataConnectorCommands.PutHandrecords, serializedData);
        }

        /// <summary>
        /// Sends PlayerData asynchronously to the BCS queue.
        /// </summary>
        /// <param name="sessionGuid">The guid of the session that player participates in.</param>
        /// <param name="playerData">a DTO containing the player data:playernumber, first name, last name, country code</param>
        /// <returns></returns>
        /// <remarks>
        /// Best practice is to send all player data with the <see cref="InitDTO">InitDTO</see>. After the session starts
        /// preferably the data is not changed to prevent unpredictable behaviour. By sending ParticipationDTOs, again preferable with the InitDTO,
        /// you can specify which players start on which direction on which table.
        /// If all players are known before the session starts only the corresponding player data of the participants of the session needs to be uploaded.
        /// If players will make themselves known at session start-up by entering their playernumber on the Bridgemate then all 
        /// playernumbers that could be entered should be uploaded using a PlayerDataDTO.
        /// </remarks>
        public Task<ScoringProgramResponse> SendPlayerDataAsync(string sessionGuid, PlayerDataDTO[] playerData)
        {
            LogMethodEntry(nameof(SendPlayerDataAsync), (nameof(sessionGuid), sessionGuid));

            var dtosForSession = playerData.Where(dto => dto.SessionGuid == sessionGuid).ToList();
            var serializedData = JsonSerializer.Serialize(dtosForSession);
            //Mind: this method (ab)uses the sessionGuid parameter to pass on the clubId.
            return SendDataAsync(sessionGuid: sessionGuid,
                                 command: ScoringProgramDataConnectorCommands.PutPlayerData,
                                 serializedData: serializedData);
        }

        /// <summary>
        /// Sends PlayerData synchronously to the BCS queue.
        /// </summary>
        /// <param name="sessionGuid">The guid of the session that player participates in.</param>
        /// <param name="playerData">a DTO containing the player data:playernumber, first name, last name, country code</param>
        /// <returns></returns>
        /// <remarks>
        /// Best practice is to send all player data with the <see cref="InitDTO">InitDTO</see>. After the session starts
        /// preferably the data is not changed to prevent unpredictable behaviour. By sending ParticipationDTOs, again preferable with the InitDTO,
        /// you can specify which players start on which direction on which table.
        /// If all players are known before the session starts only the corresponding player data of the participants of the session needs to be uploaded.
        /// If players will make themselves known at session start-up by entering their playernumber on the Bridgemate then all 
        /// playernumbers that could be entered should be uploaded using a PlayerDataDTO.
        /// </remarks>
        public ScoringProgramResponse SendPlayerData(string sessionGuid, PlayerDataDTO[] playerData)
        {
            LogMethodEntry(nameof(SendPlayerData), (nameof(sessionGuid), sessionGuid));

            var dtosForSession = playerData.Where(dto => dto.SessionGuid == sessionGuid).ToList();
            var serializedData = JsonSerializer.Serialize(dtosForSession);
            //Mind: this method (ab)uses the sessionGuid parameter to pass on the clubId.
            return SendData(sessionGuid: sessionGuid,
                                 command: ScoringProgramDataConnectorCommands.PutPlayerData,
                                 serializedData: serializedData);
        }

        /// <summary>
        /// Sends participations asynchronously to the BCS queue.
        /// </summary>
        /// <param name="dtos"></param>
        /// <param name="sessionGuid">The guid of the session that all participations must belong to.</param>
        /// <returns></returns>
        /// <remarks>
        /// Participations can be sent in two ways:
        /// 1. Setting the SessionGuid and PlayerNumber property only. In this case a PlayerDataDTO with this SessionGuid-PlayerNumber must have been uploaded too.
        /// 2.Specifying the SessionGuid and at least the LastName property, but not the PlayerNumber. In this case no corresponding PlayerData having been sent
        /// before is expected.
        /// Setting both the PlayerNumber and LastName property will result in an error.
        /// </remarks>
        public async Task<ScoringProgramResponse> SendParticipationsAsync(string sessionGuid, ParticipationDTO[] dtos)
        {
            LogMethodEntry(nameof(SendParticipationsAsync), (nameof(sessionGuid), sessionGuid));

            var dtosForSession = dtos.Where(dto => dto.SessionGuid == sessionGuid).ToList();
            if (dtosForSession.Any())
            {
                var serializedData = JsonSerializer.Serialize(dtosForSession);
                return await SendDataAsync(sessionGuid, ScoringProgramDataConnectorCommands.PutParticipations, serializedData);
            }
            else
                return new ScoringProgramResponse
                {
                    RequestCommand = ScoringProgramDataConnectorCommands.PutParticipations,
                    DataType = DataConnectorResponseData.Error,
                    ErrorType = ErrorType.NoData,
                    SerializedData = JsonSerializer.Serialize("Empty data")
                };
        }

        /// <summary>
        /// Sends participations asynchronously to the BCS queue.
        /// </summary>
        /// <param name="dtos"></param>
        /// <param name="sessionGuid">The guid of the session that all participations must belong to.</param>
        /// <returns></returns>
        /// <remarks>
        /// Participations can be sent in two ways:
        /// 1. Setting the SessionGuid and PlayerNumber property only. In this case a PlayerDataDTO with this SessionGuid-PlayerNumber must have been uploaded too.
        /// 2.Specifying the SessionGuid and at least the LastName property, but not the PlayerNumber. In this case no corresponding PlayerData having been sent
        /// before is expected.
        /// Setting both the PlayerNumber and LastName property will result in an error.
        /// </remarks>
        public ScoringProgramResponse SendParticipations(string sessionGuid, ParticipationDTO[] dtos)
        {
            LogMethodEntry(nameof(SendParticipations), (nameof(sessionGuid), sessionGuid));

            var dtosForSession = dtos.Where(dto => dto.SessionGuid == sessionGuid).ToList();
            if (dtosForSession.Any())
            {
                var serializedData = JsonSerializer.Serialize(dtosForSession);
                return SendData(sessionGuid, ScoringProgramDataConnectorCommands.PutParticipations, serializedData);
            }
            else
                return new ScoringProgramResponse
                {
                    RequestCommand = ScoringProgramDataConnectorCommands.PutParticipations,
                    DataType = DataConnectorResponseData.Error,
                    ErrorType = ErrorType.NoData,
                    SerializedData = JsonSerializer.Serialize("Empty data")
                };
        }

        /// <summary>
        /// Adds or updates the Bridgemate 2 settings for the given sections asynchronously. There can and must be one dto per section. 
        /// The section letters cannot be left out.
        /// </summary>
        /// <param name="sessionGuid"></param>
        /// <param name="bridgemate2Settings"></param>
        /// <returns></returns>
        public async Task<ScoringProgramResponse> SendBridgemate2SettingsAsync(string sessionGuid, Bridgemate2SettingsDTO[] bridgemate2Settings)
        {
            LogMethodEntry(nameof(SendBridgemate2SettingsAsync), (nameof(sessionGuid), sessionGuid));

            var dtosForSession = bridgemate2Settings.Where(dto => dto.SessionGuid == sessionGuid);
            var serialized = JsonSerializer.Serialize(dtosForSession);
            return await SendDataAsync(sessionGuid, ScoringProgramDataConnectorCommands.PutBridgemate2Settings, serialized);
        }

        /// <summary>
        /// Adds or updates the Bridgemate 2 settings for the given sections synchronously. There can and must be one dto per section. 
        /// The section letters cannot be left out!
        /// </summary>
        /// <param name="sessionGuid"></param>
        /// <param name="bridgemate2Settings"></param>
        /// <returns></returns>
        public ScoringProgramResponse SendBridgemate2Settings(string sessionGuid, Bridgemate2SettingsDTO[] bridgemate2Settings)
        {
            LogMethodEntry(nameof(SendBridgemate2Settings), (nameof(sessionGuid), sessionGuid));

            var dtosForSession = bridgemate2Settings.Where(dto => dto.SessionGuid == sessionGuid);
            var serialized = JsonSerializer.Serialize(dtosForSession);
            return SendData(sessionGuid, ScoringProgramDataConnectorCommands.PutBridgemate2Settings, serialized);
        }

        /// <summary>
        /// Adds or updates the Bridgemate 3 settings for the given sections asynchronously. There can and must be one dto per section. 
        /// The section letters cannot be left out!
        /// </summary>
        /// <param name="sessionGuid"></param>
        /// <param name="bridgemate3Settings"></param>
        /// <returns></returns>
        public async Task<ScoringProgramResponse> SendBridgemate3SettingsAsync(string sessionGuid, Bridgemate3SettingsDTO[] bridgemate3Settings)
        {
            LogMethodEntry(nameof(SendBridgemate3SettingsAsync), (nameof(sessionGuid), sessionGuid));

            var dtosForSession = bridgemate3Settings.Where(dto => dto.SessionGuid == sessionGuid);
            var serialized = JsonSerializer.Serialize(dtosForSession);
            return await SendDataAsync(sessionGuid, ScoringProgramDataConnectorCommands.PutBridgemate3Settings, serialized);
        }

        /// <summary>
        /// Adds or updates the Bridgemate 3 settings for the given sections synchronously. There can and must be one dto per section. 
        /// The section letters cannot be left out!
        /// </summary>
        /// <param name="sessionGuid"></param>
        /// <param name="bridgemate3Settings"></param>
        /// <returns></returns>
        public ScoringProgramResponse SendBridgemate3Settings(string sessionGuid, Bridgemate3SettingsDTO[] bridgemate3Settings)
        {
            LogMethodEntry(nameof(SendBridgemate3Settings), (nameof(sessionGuid), sessionGuid));

            var dtosForSession = bridgemate3Settings.Where(dto => dto.SessionGuid == sessionGuid);
            var serialized = JsonSerializer.Serialize(dtosForSession);
            return SendData(sessionGuid, ScoringProgramDataConnectorCommands.PutBridgemate3Settings, serialized);
        }

        /// <summary>
        /// The code that handles the actual sending of requests and reading their reponses aynchronously.
        /// </summary>
        /// <param name="sessionGuid">Specifies which session the request targets (if any)</param>
        /// <param name="command">The command to the Data Connector</param>
        /// <param name="serializedData">The data to send to the Data Connector as json data. (If any)</param>
        /// <returns></returns>
        private async Task<ScoringProgramResponse> SendDataAsync(string sessionGuid,
            ScoringProgramDataConnectorCommands command, string serializedData)
        {
            var logRecord = new DataConnectorLogRecord(DataConnectorLogLevel.Debug, LoggingSource, command, serializedData, nameof(SendDataAsync));
            DataConnectorClientLogger.LogRecord(logRecord);

            //Construct the request to the Data Connector.
            var request = new ScoringProgramRequest
            {
                Command = command,
                SessionGuid = sessionGuid,
                SerializedData = serializedData
            };

            //Serialize it.
            var requestSerialized = JsonSerializer.Serialize(request);

            //Do not proceed if sending is already in progress (for an other request). There can be only on request be sent at the same time.
            if (_isSending)
            {
                return new ScoringProgramResponse
                {
                    RequestCommand = command,
                    SessionGuid = sessionGuid,
                    DataType = DataConnectorResponseData.Error,
                    ErrorType = ErrorType.Busy,
                    SerializedData = JsonSerializer.Serialize($"Client is busy, please retry later.")
                };
            }
            try
            {
                _isSending = true;

                //Do not continue if the connection has been broken. Call the Connect method again then resend.
                var errorResponse = CheckConnection(command);
                if (errorResponse != null)
                {
                    return errorResponse;
                }

                //Reconnect to the Data Connector if needed.
                if (!DataConnectorStream.IsConnected)
                {
                    await DataConnectorStream.ConnectAsync(5000);
                }

                //Send the request to the Data Connector. Mind: as it is written now this a blocking call.
                //However, in .Net an exception will be thrown if the connection has gone dead for whatever reason.
                //Mind: the Data ConnectorWriter is defined in the base class.
                await DataConnectorWriter.WriteLineAsync(requestSerialized);

                //Wait for the responses. This too is a blocking call. But in .Net a broken connection will throw an exception.
                //Mind: the Data ConnectorReader is defined in the base class.
                var response = await DataConnectorReader.ReadLineAsync();
                if (response != null)
                {
                    var responseDeserialized = JsonSerializer.Deserialize<ScoringProgramResponse>(response);
                    return responseDeserialized ??
                             new ScoringProgramResponse
                             {
                                 RequestCommand = command,
                                 DataType = DataConnectorResponseData.Error,
                                 SerializedData = JsonSerializer.Serialize("Empty response")
                             };
                }
                else
                {
                    return new ScoringProgramResponse
                    {
                        RequestCommand = command,
                        DataType = DataConnectorResponseData.Error,
                        SerializedData = JsonSerializer.Serialize("Empty response")
                    };
                }
            }
            catch (IOException)
            {
                //CloseConnection is defined in the base class.
                CloseConnection();
                return
                new ScoringProgramResponse
                {
                    RequestCommand = command,
                    DataType = DataConnectorResponseData.Error,
                    SerializedData = JsonSerializer.Serialize("Pipe broken")
                };
            }
            catch (Exception ex)
            {
                CloseConnection();
                LogError(ex);
                return
                new ScoringProgramResponse
                {
                    RequestCommand = command,
                    DataType = DataConnectorResponseData.Error,
                    SerializedData = JsonSerializer.Serialize(ex.Message)
                };
            }

            finally
            {
                //Always signal that the client is free for the next items to send.
                //Otherwise after an exception further communication will be blocked.
                _isSending = false;
            }
        }


        /// <summary>
        /// The code that handles the actual sending of requests and reading their reponses aynchronously.
        /// </summary>
        /// <param name="sessionGuid">Specifies which session the request targets (if any)</param>
        /// <param name="command">The command to the Data Connector</param>
        /// <param name="serializedData">The data to send to the Data Connector as json data. (If any)</param>
        /// <returns></returns>
        private ScoringProgramResponse SendData(string sessionGuid, ScoringProgramDataConnectorCommands command, string serializedData)
        {
            var logRecord = new DataConnectorLogRecord(DataConnectorLogLevel.Debug, LoggingSource, command, serializedData, nameof(SendData));
            DataConnectorClientLogger.LogRecord(logRecord);

            //Construct the request to the Data Connector.
            var request = new ScoringProgramRequest
            {
                Command = command,
                SessionGuid = sessionGuid,
                SerializedData = serializedData
            };

            //Serialize it.
            var requestSerialized = JsonSerializer.Serialize(request);

            //Do not proceed if sending is already in progress (for an other request). There can be only on request be sent at the same time.
            if (_isSending)
            {
                return new ScoringProgramResponse
                {
                    RequestCommand = command,
                    SessionGuid = sessionGuid,
                    DataType = DataConnectorResponseData.Error,
                    ErrorType = ErrorType.Busy,
                    SerializedData = JsonSerializer.Serialize($"Client is busy, please retry later.")
                };
            }
            try
            {
                _isSending = true;

                //Do not continue if the connection has been broken. Call the Connect method again then resend.
                var errorResponse = CheckConnection(command);
                if (errorResponse != null)
                {
                    return errorResponse;
                }

                //Reconnect to the Data Connector if needed.
                if (!DataConnectorStream.IsConnected)
                {
                    DataConnectorStream.Connect(5000);
                }

                //Send the request to the Data Connector. Mind: as it is written now this a blocking call.
                //However, in .Net an exception will be thrown if the connection has gone dead for whatever reason.
                //Mind: the Data ConnectorWriter is defined in the base class.
                DataConnectorWriter.WriteLine(requestSerialized);

                //Wait for the responses. This too is a blocking call. But in .Net a broken connection will throw an exception.
                //Mind: the Data ConnectorReader is defined in the base class.
                var response = DataConnectorReader.ReadLine();
                if (response != null)
                {
                    var responseDeserialized = JsonSerializer.Deserialize<ScoringProgramResponse>(response);
                    return responseDeserialized ??
                             new ScoringProgramResponse
                             {
                                 RequestCommand = command,
                                 DataType = DataConnectorResponseData.Error,
                                 SerializedData = JsonSerializer.Serialize("Empty response")
                             };
                }
                else
                {
                    return new ScoringProgramResponse
                    {
                        RequestCommand = command,
                        DataType = DataConnectorResponseData.Error,
                        SerializedData = JsonSerializer.Serialize("Empty response")
                    };
                }
            }
            catch (IOException)
            {
                //CloseConnection is defined in the base class.
                CloseConnection();
                return
                new ScoringProgramResponse
                {
                    RequestCommand = command,
                    DataType = DataConnectorResponseData.Error,
                    SerializedData = JsonSerializer.Serialize("Pipe broken")
                };
            }
            catch (Exception ex)
            {
                CloseConnection();
                LogError(ex);
                return
                new ScoringProgramResponse
                {
                    RequestCommand = command,
                    DataType = DataConnectorResponseData.Error,
                    SerializedData = JsonSerializer.Serialize(ex.Message)
                };
            }

            finally
            {
                //Always signal that the client is free for the next items to send.
                //Otherwise after an exception further communication will be blocked.
                _isSending = false;
            }
        }

        /// <summary>
        /// Polls the client queue for new boardresults asynchronously.
        /// <param name="sessionGuid">Required. The guid of the session to poll participations for.</param>
        /// <param name="all">Poll all results for the session, included ones polled before.</param>
        /// </summary>
        /// <returns></returns>
        public async Task<ResultDTO[]> PollForResultsAsync(string sessionGuid, bool all = false)
        {
            LogMethodEntry(nameof(PollForResultsAsync), (nameof(sessionGuid), sessionGuid), (nameof(all), all));

            var command = all ? ScoringProgramDataConnectorCommands.PollQueueForAllResults :
                                ScoringProgramDataConnectorCommands.PollQueueForNewResults;
            var response = await SendDataAsync(sessionGuid, command, serializedData: "");
            if (response.DataType != DataConnectorResponseData.Results)
            {
                LogError(new Exception($"ERROR: {response.ErrorType},'{JsonSerializer.Deserialize<string>(response.SerializedData)}'"));
                return new ResultDTO[] { };
            }
            var data = string.IsNullOrWhiteSpace(response.SerializedData) ? new ResultDTO[] { } :
                                                                            JsonSerializer.Deserialize<ResultDTO[]>(response.SerializedData);

            //Cache the id for the last queue item. We can use this to accept up to this id included.
            if (data.Any())
                _lastResultQueueItemId = response.LastQueueItemId;

            return data;

        }

        /// <summary>
        /// Polls the client queue for new boardresults synchronously.
        /// <param name="sessionGuid">Required. The guid of the session to poll participations for.</param>
        /// <param name="all">Poll all results for the session, included ones polled before.</param>
        /// </summary>
        /// <returns></returns>
        public ResultDTO[] PollForResults(string sessionGuid, bool all = false)
        {
            LogMethodEntry(nameof(PollForResults), (nameof(sessionGuid), sessionGuid), (nameof(all), all));

            var command = all ? ScoringProgramDataConnectorCommands.PollQueueForAllResults :
                                ScoringProgramDataConnectorCommands.PollQueueForNewResults;
            var response = SendData(sessionGuid, command, serializedData: "");
            if (response.DataType != DataConnectorResponseData.Results)
            {
                LogError(new Exception($"ERROR: {response.ErrorType},'{JsonSerializer.Deserialize<string>(response.SerializedData)}'"));
                return new ResultDTO[] { };
            }
            var data = string.IsNullOrWhiteSpace(response.SerializedData) ? new ResultDTO[] { } :
                                                                            JsonSerializer.Deserialize<ResultDTO[]>(response.SerializedData);

            //Cache the id for the last queue item. We can use this to accept up to this id included.
            if (data.Any())
                _lastResultQueueItemId = response.LastQueueItemId;

            return data;
        }

        /// <summary>
        /// Polls the client queue asynchronously for new player data for the specified session.
        /// </summary>
        /// <param name="sessionGuid">Required. The guid of the session to poll participations for.</param>
        /// <param name="all">Poll all player data for the session, included ones polled before.</param>
        /// <returns></returns>
        public async Task<PlayerDataDTO[]> PollForPlayerDataAsync(string sessionGuid, bool all = false)
        {
            LogMethodEntry(nameof(PollForPlayerDataAsync), (nameof(sessionGuid), sessionGuid), (nameof(all), all));

            var command = all ? ScoringProgramDataConnectorCommands.PollQueueForAllPlayerData :
                                ScoringProgramDataConnectorCommands.PollQueueForNewPlayerData;
            var response = await SendDataAsync(sessionGuid, command, serializedData: "");
            if (response.DataType != DataConnectorResponseData.PlayerData)
            {
                LogError(new Exception($"ERROR: {response.ErrorType},'{JsonSerializer.Deserialize<string>(response.SerializedData)}'"));
                return new PlayerDataDTO[] { };
            }
            var data = string.IsNullOrWhiteSpace(response.SerializedData) ? new PlayerDataDTO[] { } :
                                                                            JsonSerializer.Deserialize<PlayerDataDTO[]>(response.SerializedData);

            //Cache the id for the last queue item. We can use this to accept up to this id included.
            if (data.Any())
                _lastPlayeDataQueueItemId = response.LastQueueItemId;

            return data;

        }

        /// <summary>
        /// Polls the client queue asynchronously for new player data for the specified session.
        /// </summary>
        /// <param name="sessionGuid">Required. The guid of the session to poll participations for.</param>
        /// <param name="all">Poll all player data for the session, included ones polled before.</param>
        /// <returns></returns>
        public PlayerDataDTO[] PollForPlayerData(string sessionGuid, bool all = false)
        {
            LogMethodEntry(nameof(PollForPlayerData), (nameof(sessionGuid), sessionGuid), (nameof(all), all));

            var command = all ? ScoringProgramDataConnectorCommands.PollQueueForAllPlayerData :
                                ScoringProgramDataConnectorCommands.PollQueueForNewPlayerData;
            var response = SendData(sessionGuid, command, serializedData: "");
            if (response.DataType != DataConnectorResponseData.PlayerData)
            {
                LogError(new Exception($"ERROR: {response.ErrorType},'{JsonSerializer.Deserialize<string>(response.SerializedData)}'"));
                return new PlayerDataDTO[] { };
            }
            var data = string.IsNullOrWhiteSpace(response.SerializedData) ? new PlayerDataDTO[] { } :
                                                                            JsonSerializer.Deserialize<PlayerDataDTO[]>(response.SerializedData);

            //Cache the id for the last queue item. We can use this to accept up to this id included.
            if (data.Any())
                _lastPlayeDataQueueItemId = response.LastQueueItemId;

            return data;

        }

        /// <summary>
        /// Polls the client queue asynchronously for new participations for the specified session.
        /// </summary>
        /// <param name="sessionGuid">Required. The guid of the session to poll participations for.</param>
        /// <param name="all">Poll all participations for the session, included ones polled before.</param>
        /// <returns></returns>
        public async Task<ParticipationDTO[]> PollForParticipationsAsync(string sessionGuid, bool all = false)
        {
            LogMethodEntry(nameof(PollForParticipationsAsync), (nameof(sessionGuid), sessionGuid), (nameof(all), all));

            var command = all ? ScoringProgramDataConnectorCommands.PollQueueForAllParticipations :
                                ScoringProgramDataConnectorCommands.PollQueueForNewParticipations;
            var response = await SendDataAsync(sessionGuid, command, serializedData: "");
            if (response.DataType != DataConnectorResponseData.Participations)
            {
                LogError(new Exception($"ERROR: {response.ErrorType},'{JsonSerializer.Deserialize<string>(response.SerializedData)}'"));
                return new ParticipationDTO[] { };
            }
            var data = string.IsNullOrWhiteSpace(response.SerializedData) ? new ParticipationDTO[] { } :
                                                                            JsonSerializer.Deserialize<ParticipationDTO[]>(response.SerializedData);

            //Cache the id for the last queue item. We can use this to accept up to this id included.
            if (data.Any())
                _lastParticipantQueueItemId = response.LastQueueItemId;

            return data;

        }
        /// <summary>
        /// Polls the client queue synchronously for new participations for the specified session.
        /// </summary>
        /// <param name="sessionGuid">Required. The guid of the session to poll participations for.</param>
        /// <param name="all">Poll all participations for the session, included ones polled before.</param>
        /// <returns></returns>
        public ParticipationDTO[] PollForParticipations(string sessionGuid, bool all = false)
        {
            LogMethodEntry(nameof(PollForParticipations), (nameof(sessionGuid), sessionGuid), (nameof(all), all));

            var command = all ? ScoringProgramDataConnectorCommands.PollQueueForAllParticipations :
                                ScoringProgramDataConnectorCommands.PollQueueForNewParticipations;

            var response = SendData(sessionGuid, command, serializedData: "");
            if (response.DataType != DataConnectorResponseData.Participations)
            {
                LogError(new Exception($"ERROR: {response.ErrorType},'{JsonSerializer.Deserialize<string>(response.SerializedData)}'"));
                return new ParticipationDTO[] { };
            }
            var data = string.IsNullOrWhiteSpace(response.SerializedData) ? new ParticipationDTO[] { } :
                                                                            JsonSerializer.Deserialize<ParticipationDTO[]>(response.SerializedData);

            //Cache the id for the last queue item. We can use this to accept up to this id included.
            if (data.Any())
                _lastParticipantQueueItemId = response.LastQueueItemId;

            return data;

        }

        /// <summary>
        /// Polls the Data Connector asynchronously for new handrecords for the session,
        /// </summary>
        /// <param name="sessionGuid">Required. The guid of the session to poll handrecords for.</param>
        /// <param name="all">Poll all handrecords for the session, included ones polled before.</param>
        /// <returns></returns>
        public async Task<HandrecordDTO[]> PollForHandrecordsAsync(string sessionGuid, bool all = false)
        {
            LogMethodEntry(nameof(PollForHandrecordsAsync), (nameof(sessionGuid), sessionGuid), (nameof(all), all));

            var command = all ? ScoringProgramDataConnectorCommands.PollQueueForAllHandrecords :
                                ScoringProgramDataConnectorCommands.PollQueueForNewHandrecords;

            var response = await SendDataAsync(sessionGuid, command, serializedData: "");
            if (response.DataType != DataConnectorResponseData.Handrecords)
            {
                LogError(new Exception($"ERROR: {response.ErrorType},'{JsonSerializer.Deserialize<string>(response.SerializedData)}'"));
                return new HandrecordDTO[] { };
            }
            var data = string.IsNullOrWhiteSpace(response.SerializedData) ? new HandrecordDTO[] { } :
                                                                            JsonSerializer.Deserialize<HandrecordDTO[]>(response.SerializedData);

            //Cache the id for the last queue item. We can use this to accept up to this id included.
            if (data.Any())
                _lastHandrecordQueueItemId = response.LastQueueItemId;

            return data;
        }

        /// <summary>
        /// Polls the Data Connector synchronously for new handrecords for the session,
        /// </summary>
        /// <param name="sessionGuid">Required. The guid of the session to poll handrecords for.</param>
        /// <param name="all">Poll all handrecords for the session, included ones polled before.</param>
        /// <returns></returns>
        public HandrecordDTO[] PollForHandrecords(string sessionGuid, bool all = false)
        {
            LogMethodEntry(nameof(PollForHandrecords), (nameof(sessionGuid), sessionGuid), (nameof(all), all));

            var command = all ? ScoringProgramDataConnectorCommands.PollQueueForAllHandrecords :
                                ScoringProgramDataConnectorCommands.PollQueueForNewHandrecords;
            var response = SendData(sessionGuid, command, serializedData: "");
            if (response.DataType != DataConnectorResponseData.Handrecords)
            {
                LogError(new Exception($"ERROR: {response.ErrorType},'{JsonSerializer.Deserialize<string>(response.SerializedData)}'"));
                return new HandrecordDTO[] { };
            }
            var data = string.IsNullOrWhiteSpace(response.SerializedData) ? new HandrecordDTO[] { } :
                                                                            JsonSerializer.Deserialize<HandrecordDTO[]>(response.SerializedData);

            //Cache the id for the last queue item. We can use this to accept up to this id included.
            if (data.Any())
                _lastHandrecordQueueItemId = response.LastQueueItemId;

            return data;
        }

        /// <summary>
        /// Signals to the Data Connector synchronously that a specific type of queuedata up to the specified id does not need to be sent again.
        /// </summary>
        /// <param name="sessionGuid">The guid of the session to accept the specific eventqueue data for.</param>
        /// <param name="dataType">The type of eventqueue data to accept (results, handrecords, participations, etc.).</param>
        /// <returns></returns>
        /// <remarks>
        /// The Data Connector requires an id of the last downloaded eventqueue item of the given type. These id's are stored
        /// as fields in this class. You can also choose to make the last eventqueue id (for the given type!) a parameter of this method.
        /// </remarks>
        public ScoringProgramResponse AcceptQueueData(string sessionGuid, DataConnectorResponseData dataType)
        {
            LogMethodEntry(nameof(AcceptQueueData), (nameof(sessionGuid), sessionGuid), (nameof(dataType), dataType));

            (ScoringProgramDataConnectorCommands command, int lastQueueItemId) parameters = DetermineParametersForQueuAcceptance(dataType);
            if (parameters.command == ScoringProgramDataConnectorCommands.None)
            {
                return new ScoringProgramResponse
                {
                    RequestCommand = ScoringProgramDataConnectorCommands.None,
                    DataType = DataConnectorResponseData.Error,
                    ErrorType = ErrorType.Validation,
                    SerializedData = JsonSerializer.Serialize($"Invalid datatype '{dataType}'")
                };
            }
            var serializedLastQueueItemId = JsonSerializer.Serialize(parameters.lastQueueItemId);

            return SendData(sessionGuid, parameters.command, serializedLastQueueItemId);

        }

        /// <summary>
        /// Signals to the Data Connector asynchronously that a specific type of queuedata up to the specified id does not need to be sent again.
        /// </summary>
        /// <param name="sessionGuid">The guid of the session to accept the specific eventqueue data for.</param>
        /// <param name="dataType">The type of eventqueue data to accept (results, handrecords, participations, etc.).</param>
        /// <returns></returns>
        /// <remarks>
        /// The Data Connector requires an id of the last downloaded eventqueue item of the given type. These id's are stored
        /// as fields in this class. You can also choose to make the last eventqueue id (for the given type!) a parameter of this method.
        /// </remarks>
        public async Task<ScoringProgramResponse> AcceptQueueDataAsync(string sessionGuid, DataConnectorResponseData dataType)
        {
            LogMethodEntry(nameof(AcceptQueueDataAsync), (nameof(sessionGuid), sessionGuid), (nameof(dataType), dataType));

            (ScoringProgramDataConnectorCommands command, int lastQueueItemId) parameters = DetermineParametersForQueuAcceptance(dataType);
            if (parameters.command == ScoringProgramDataConnectorCommands.None)
            {
                return new ScoringProgramResponse
                {
                    RequestCommand = ScoringProgramDataConnectorCommands.None,
                    DataType = DataConnectorResponseData.Error,
                    ErrorType = ErrorType.Validation,
                    SerializedData = JsonSerializer.Serialize($"Invalid datatype '{dataType}'")
                };
            }
            var serializedLastQueueItemId = JsonSerializer.Serialize(parameters.lastQueueItemId);

            return await SendDataAsync(sessionGuid, parameters.command, serializedLastQueueItemId);

        }

        /// <summary>
        /// Determines which command to sent to the Data Connector and which last queue item id to use.
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        private (ScoringProgramDataConnectorCommands command, int lastQueueItemId) DetermineParametersForQueuAcceptance(DataConnectorResponseData dataType)
        {
            int lastQueueItemId;
            ScoringProgramDataConnectorCommands command;

            switch (dataType)
            {
                case DataConnectorResponseData.Results:
                    {
                        lastQueueItemId = _lastResultQueueItemId;
                        command = ScoringProgramDataConnectorCommands.AcceptResultQueueItems; break;
                    }
                case DataConnectorResponseData.Participations:
                    {
                        lastQueueItemId = _lastParticipantQueueItemId;
                        command = ScoringProgramDataConnectorCommands.AcceptParticipantQueueItems; break;
                    }
                case DataConnectorResponseData.PlayerData:
                    {
                        lastQueueItemId = _lastPlayeDataQueueItemId;
                        command = ScoringProgramDataConnectorCommands.AcceptPlayerDataQueueItems; break;
                    }
                case DataConnectorResponseData.Handrecords:
                    {
                        lastQueueItemId = _lastHandrecordQueueItemId;
                        command = ScoringProgramDataConnectorCommands.AcceptHandrecordQueueItems; break;
                    }
                default:
                    {
                        lastQueueItemId = -1;
                        command = ScoringProgramDataConnectorCommands.None;
                        break;
                    }
            }
            return (command, lastQueueItemId);
        }

        /// <summary>
        /// Returns the movement asynchronously for the given section as it is in use by BCS.Mind that the Data Connector itself creates updates to 
        /// enact movement changes.
        /// </summary>
        /// <param name="sessionGuid">The guid of the session to get the movements from.</param>
        /// <param name="sectionLetters">The letters of the section to get the movements from.</param>
        /// <returns></returns>
        public async Task<SectionDTO> GetMovementAsync(string sessionGuid, string sectionLetters)
        {
            LogMethodEntry(nameof(GetMovementAsync), (nameof(sessionGuid), sessionGuid), (nameof(sectionLetters), sectionLetters));

            try
            {
                var sectionDto = new SectionDTO
                {
                    SessionGuid = sessionGuid,
                    Letters = sectionLetters
                };
                var serializedData = JsonSerializer.Serialize(sectionDto);
                var response = await SendDataAsync(sessionGuid, ScoringProgramDataConnectorCommands.GetMovement, serializedData);
                if (response.RequestCommand != ScoringProgramDataConnectorCommands.GetMovement)
                {
                    LogError(new Exception($"The {ScoringProgramDataConnectorCommands.GetMovement} command returned an unexpected result: " +
                        $"'{response.RequestCommand}'"));
                    return null;
                }
                if (response.DataType != DataConnectorResponseData.Movement)
                {
                    LogError(new Exception($"The {ScoringProgramDataConnectorCommands.GetMovement} failed: " +
                                 $"{response.DataType}-{response.ErrorType}: '{JsonSerializer.Deserialize<string>(response.SerializedData)}'"));
                    return null;
                }
                var section = JsonSerializer.Deserialize<SectionDTO>(response.SerializedData);
                return section;
            }
            catch (Exception ex)
            {
                LogError(ex);
                return null;
            }
        }

        /// <summary>
        /// Returns the movement for the given section synchronously as it is in use by BCS.Mind that the Data Connector itself creates updates to 
        /// enact movement changes.
        /// </summary>
        /// <param name="sessionGuid">The guid of the session to get the movement from</param>
        /// <param name="sectionLetters">The letters of the section to get the movement from</param>
        /// <returns></returns>
        public SectionDTO GetMovement(string sessionGuid, string sectionLetters)
        {
            LogMethodEntry(nameof(GetMovement), (nameof(sessionGuid), sessionGuid), (nameof(sectionLetters), sectionLetters));

            try
            {
                var sectionDto = new SectionDTO
                {
                    SessionGuid = sessionGuid,
                    Letters = sectionLetters
                };
                var serializedData = JsonSerializer.Serialize(sectionDto);
                var response = SendData(sessionGuid, ScoringProgramDataConnectorCommands.GetMovement, serializedData);
                if (response.RequestCommand != ScoringProgramDataConnectorCommands.GetMovement)
                {
                    LogError(new Exception($"The {ScoringProgramDataConnectorCommands.GetMovement} command returned an unexpected result: " +
                        $"'{response.RequestCommand}'"));
                    return null;
                }
                if (response.DataType != DataConnectorResponseData.Movement)
                {
                    LogError(new Exception($"The {ScoringProgramDataConnectorCommands.GetMovement} failed: " +
                                 $"{response.DataType}-{response.ErrorType}: '{JsonSerializer.Deserialize<string>(response.SerializedData)}'"));
                    return null;
                }
                var section = JsonSerializer.Deserialize<SectionDTO>(response.SerializedData);
                return section;
            }
            catch (Exception ex)
            {
                LogError(ex);
                return null;
            }
        }

        /// <summary>
        /// Returns the movements asynchronously for the given session as they are in use by BCS.Mind that the Data Connector itself creates updates to 
        /// enact movement changes.
        /// </summary>
        /// <param name="sessionGuid">The guid of the session to get all movements from</param>
        /// <returns></returns>
        public async Task<SectionDTO[]> GetAllMovementsAsync(string sessionGuid)
        {
            LogMethodEntry(nameof(GetAllMovementsAsync), (nameof(sessionGuid), sessionGuid));

            var emptyResponse = new SectionDTO[] { };
            try
            {
                var response = await SendDataAsync(sessionGuid, ScoringProgramDataConnectorCommands.GetAllMovements, serializedData: "");
                if (response.RequestCommand != ScoringProgramDataConnectorCommands.GetAllMovements)
                {
                    LogError(new Exception($"The {ScoringProgramDataConnectorCommands.GetAllMovements} command returned an unexpected result: " +
                        $"'{response.RequestCommand}'"));

                    return emptyResponse;
                }
                if (response.DataType != DataConnectorResponseData.Sessions)
                {
                    LogError(new Exception($"The {ScoringProgramDataConnectorCommands.GetAllMovements} failed: " +
                                 $"{response.DataType}-{response.ErrorType}: '{JsonSerializer.Deserialize<string>(response.SerializedData)}'"));

                    return emptyResponse;
                }
                var sections = JsonSerializer.Deserialize<SectionDTO[]>(response.SerializedData);
                return sections;
            }
            catch (Exception ex)
            {
                LogError(ex);
                return emptyResponse;
            }
        }

        /// <summary>
        /// Returns the movements synchronously for the given session as they are in use by BCS.Mind that the Data Connector itself creates updates to 
        /// enact movement changes.
        /// </summary>
        /// <param name="sessionGuid">The guid of the session to get all movements from</param>
        /// <returns></returns>
        public SectionDTO[] GetAllMovements(string sessionGuid)
        {
            LogMethodEntry(nameof(GetAllMovements), (nameof(sessionGuid), sessionGuid));

            var emptyResponse = new SectionDTO[] { };
            try
            {
                var response = SendData(sessionGuid, ScoringProgramDataConnectorCommands.GetAllMovements, serializedData: "");
                if (response.RequestCommand != ScoringProgramDataConnectorCommands.GetAllMovements)
                {
                    LogError(new Exception($"The {ScoringProgramDataConnectorCommands.GetAllMovements} command returned an unexpected result: " +
                        $"'{response.RequestCommand}'"));

                    return emptyResponse;
                }
                if (response.DataType != DataConnectorResponseData.Sessions)
                {
                    LogError(new Exception($"The {ScoringProgramDataConnectorCommands.GetAllMovements} failed: " +
                                 $"{response.DataType}-{response.ErrorType}: '{JsonSerializer.Deserialize<string>(response.SerializedData)}'"));

                    return emptyResponse;
                }
                var sections = JsonSerializer.Deserialize<SectionDTO[]>(response.SerializedData);
                return sections;
            }
            catch (Exception ex)
            {
                LogError(ex);
                return emptyResponse;
            }
        }

        /// <summary>
        /// Instructs BCS asynchronously to update the scoringgroups. This can be done to change the scoring method or to rearrange the sections that are attached to them.
        /// </summary>
        /// <param name="scoringGroups">The scoringgroups to update</param>
        /// <returns></returns>
        public async Task<ScoringProgramResponse> UpdateScoringGroupsAsync(IEnumerable<ScoringGroupDTO> scoringGroups)
        {
            LogMethodEntry($"{nameof(UpdateScoringGroupsAsync)}");

            foreach (var groupedScoringGroup in scoringGroups.GroupBy(sg => sg.SessionGuid))
            {
                var serializedData = JsonSerializer.Serialize(groupedScoringGroup);
                var response = await SendDataAsync(groupedScoringGroup.Key, ScoringProgramDataConnectorCommands.UpdateScoringGroups, serializedData);
                if (response.DataType != DataConnectorResponseData.OK)
                    return response;
            }
            return new ScoringProgramResponse
            {
                RequestCommand = ScoringProgramDataConnectorCommands.UpdateScoringGroups,
                DataType = DataConnectorResponseData.OK,
                ErrorType = ErrorType.None,
                SerializedData = JsonSerializer.Serialize($"{scoringGroups.Count()} scoringgroups uploaded.")
            };
        }

        /// <summary>
        /// Instructs BCS synchronously to update the scoringgroups. This can be done to change the scoring method or to rearrange the sections that are attached to them.
        /// </summary>
        /// <param name="scoringGroups">the scoringgroups to update</param>
        /// <returns></returns>
        public ScoringProgramResponse UpdateScoringGroups(IEnumerable<ScoringGroupDTO> scoringGroups)
        {
            LogMethodEntry(nameof(UpdateScoringGroups));

            foreach (var groupedScoringGroup in scoringGroups.GroupBy(sg => sg.SessionGuid))
            {
                var serializedData = JsonSerializer.Serialize(groupedScoringGroup);
                var response = SendData(groupedScoringGroup.Key, ScoringProgramDataConnectorCommands.UpdateScoringGroups, serializedData);
                if (response.DataType != DataConnectorResponseData.OK)
                    return response;
            }
            return new ScoringProgramResponse
            {
                RequestCommand = ScoringProgramDataConnectorCommands.UpdateScoringGroups,
                DataType = DataConnectorResponseData.OK,
                ErrorType = ErrorType.None,
                SerializedData = JsonSerializer.Serialize($"{scoringGroups.Count()} scoringgroups uploaded.")
            };
        }

        /// <summary>
        /// Instructs BCS asynchronously to update the movement for the given section. Be sure to include the movement for the section after its change.
        /// BCS will figure out how to adjust its data to enact the change.
        /// Mind that board results will be erased for each table starting from the lowest numbered round with different round data.
        /// </summary>
        /// <param name="updatedSection">The section with its tables and rounds as they are after the update.</param>
        /// <returns></returns>
        public async Task<ScoringProgramResponse> UpdateMovementAsync(SectionUpdateDTO updatedSection)
        {
            LogMethodEntry(nameof(UpdateMovementAsync));

            var serializedData = JsonSerializer.Serialize(updatedSection);
            return await SendDataAsync(updatedSection.SessionGuid, ScoringProgramDataConnectorCommands.UpdateMovement, serializedData);
        }

        /// <summary>
        /// Instructs BCS synchronously to update the movement for the given section. Be sure to include the movement for the section after its change.
        /// BCS will figure out how to adjust its data to enact the change.
        /// Mind that board results will be erased for each table starting from the lowest numbered round with different round data.
        /// </summary>
        /// <param name="updatedSection">The section with its tables and rounds as they are after the update.</param>
        /// <returns></returns>
        public ScoringProgramResponse UpdateMovement(SectionUpdateDTO updatedSection)
        {
            LogMethodEntry(nameof(UpdateMovement));

            var serializedData = JsonSerializer.Serialize(updatedSection);
            return SendData(updatedSection.SessionGuid, ScoringProgramDataConnectorCommands.UpdateMovement, serializedData);
        }

        /// <summary>
        /// Adds the given session to a known event asynchronously.
        /// This event must have been sent to the Data Connector previously using an <see cref="InitDTO">InitDTO</see>.
        /// </summary>
        /// <param name="addedSession"></param>
        /// <returns></returns>
        public async Task<ScoringProgramResponse> AddSessionAsync(SessionDTO addedSession)
        {
            LogMethodEntry(nameof(AddSessionAsync));

            var serializedData = JsonSerializer.Serialize(addedSession);
            return await SendDataAsync(addedSession.EventGuid, ScoringProgramDataConnectorCommands.AddSession, serializedData);
        }

        /// <summary>
        /// Adds the given session to a known event synchronously.
        /// This event must have been sent to the Data Connector previously using an <see cref="InitDTO">InitDTO</see>.
        /// </summary>
        /// <param name="addedSession"></param>
        /// <returns></returns>
        public ScoringProgramResponse AddSession(SessionDTO addedSession)
        {
            LogMethodEntry(nameof(AddSession));

            var serializedData = JsonSerializer.Serialize(addedSession);
            return SendData(addedSession.SessionGuid, ScoringProgramDataConnectorCommands.AddSession, serializedData);
        }

        /// <summary>
        /// Issue a management command to BCS asynchronously. This command can either be a query for information on the location of its scoring file, which sessions
        /// it is currently administering or which sessions are known to it, or it can be an instruction to shut down.
        /// </summary>
        /// <param name="managementDTO">The carrier of the managment command.</param>
        /// <returns></returns>
        public async Task<ScoringProgramResponse> IssueManagementCommandAsync(BCSManagementRequestDTO managementDTO)
        {
            LogMethodEntry(nameof(IssueManagementCommandAsync));

            if (!managementDTO.Validate())
            {
                var validationErrors = JsonSerializer.Serialize(managementDTO.ValidationMessages);
                return new ScoringProgramResponse
                {
                    RequestCommand = ScoringProgramDataConnectorCommands.ManageBCS,
                    DataType = DataConnectorResponseData.Error,
                    ErrorType = ErrorType.Validation,
                    SerializedData = validationErrors
                };
            }
            var serializedData = JsonSerializer.Serialize(managementDTO);
            var result = await SendDataAsync(sessionGuid: "", ScoringProgramDataConnectorCommands.ManageBCS, serializedData);
            return result;
        }

        /// <summary>
        /// Issue a management command to BCS synchronously. This command can either be a query for information on the location of its scoring file, which sessions
        /// it is currently administering or which sessions are know to it, or it can be an instruction to shut down.
        /// </summary>
        /// <param name="managementDTO">The carrier of the management command</param>
        /// <returns></returns>
        public ScoringProgramResponse IssueManagementCommand(BCSManagementRequestDTO managementDTO)
        {
            LogMethodEntry(nameof(IssueManagementCommand));

            if (!managementDTO.Validate())
            {
                var validationErrors = JsonSerializer.Serialize(managementDTO.ValidationMessages);
                return new ScoringProgramResponse
                {
                    RequestCommand = ScoringProgramDataConnectorCommands.ManageBCS,
                    DataType = DataConnectorResponseData.Error,
                    ErrorType = ErrorType.Validation,
                    SerializedData = validationErrors
                };
            }
            var serializedData = JsonSerializer.Serialize(managementDTO);
            var result = SendData(sessionGuid: "", ScoringProgramDataConnectorCommands.ManageBCS, serializedData);
            return result;
        }

        /// <summary>
        /// Does an asynchronous check on if the session with the given guid exists.
        /// </summary>
        /// <param name="sessionGuid"></param>
        /// <returns></returns>
        public async Task<bool> DoesSessionExistAsync(string sessionGuid)
        {
            try
            {
                var dto = new BCSManagementRequestDTO
                {
                    Command = BCSManagementRequestDTO.GetAllSessionsInformation
                };
                ScoringProgramResponse result = await IssueManagementCommandAsync(dto);
                if (result.DataType == DataConnectorResponseData.AllSessionsInfo)
                {
                    BCSManagementResponseDTO info = JsonSerializer.Deserialize<BCSManagementResponseDTO>(result.SerializedData);
                    var sessionInfos = info.SessionInformation ?? new[] { new SessionInfoDTO() };
                    return sessionInfos.Any(i => i.SessionGuid == sessionGuid);
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                DataConnectorClientLogger.LogError(ex, DataConnectorLoggingSource.ScoringProgramClient);
                return false;
            }
        }

        /// <summary>
        /// Does an synchronous check on if the session with the given guid exists.
        /// </summary>
        /// <param name="sessionGuid"></param>
        /// <returns></returns>
        public bool DoesSessionExist(string sessionGuid)
        {
            try
            {
                var dto = new BCSManagementRequestDTO
                {
                    Command = BCSManagementRequestDTO.GetAllSessionsInformation
                };
                ScoringProgramResponse result = IssueManagementCommand(dto);
                if (result.DataType == DataConnectorResponseData.AllSessionsInfo)
                {
                    BCSManagementResponseDTO info = JsonSerializer.Deserialize<BCSManagementResponseDTO>(result.SerializedData);
                    var sessionInfos = info.SessionInformation ?? new[] { new SessionInfoDTO() };
                    return sessionInfos.Any(i => i.SessionGuid == sessionGuid);
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                DataConnectorClientLogger.LogError(ex, DataConnectorLoggingSource.ScoringProgramClient);
                return false;
            }
        }

        /// <summary>
        /// Instructs the DataConnector to asynchronously clear all incoming and outgoing queuedata and to remove al validation data as well.
        /// Use with caution!
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ClearDataAsync()
        {
            ScoringProgramResponse result = await SendDataAsync(sessionGuid: string.Empty,
                                                                command: ScoringProgramDataConnectorCommands.ClearData,
                                                                serializedData: string.Empty);
            return result.DataType == DataConnectorResponseData.OK;
        }

        /// <summary>
        /// Instructs the DataConnector to synchronously clear all incoming and outgoing queuedata and to remove al validation data as well.
        /// Use with caution!
        /// </summary>
        /// <returns></returns>
        public bool ClearData()
        {
            ScoringProgramResponse result= SendData(sessionGuid: string.Empty,
                                                    command: ScoringProgramDataConnectorCommands.ClearData,
                                                    serializedData: string.Empty);
            return result.DataType == DataConnectorResponseData.OK;
        }


        /// <summary>
        /// Checks if the connection has been established after a Connect command.
        /// </summary>
        /// <param name="command">The command that will be executed by the calling code it the connection is alive.
        ///                       Is passed only for logging purposes.</param>
        /// <returns>A Clientresponse when there is an error, null otherwise.</returns>
        /// <remarks>
        /// Alternatively the Ping command can be used to check the connection before sending a request.
        /// </remarks>
        private ScoringProgramResponse CheckConnection(ScoringProgramDataConnectorCommands command)
        {
            if (DataConnectorStream == null)
                return new ScoringProgramResponse
                {
                    RequestCommand = command,
                    DataType = DataConnectorResponseData.Error,
                    ErrorType = ErrorType.NoConnection,
                    SerializedData = JsonSerializer.Serialize($"{nameof(DataConnectorStream)}  is null.")
                };

            if (DataConnectorReader == null)
                return new ScoringProgramResponse
                {
                    RequestCommand = command,
                    DataType = DataConnectorResponseData.Error,
                    ErrorType = ErrorType.NoConnection,
                    SerializedData = JsonSerializer.Serialize($"{nameof(DataConnectorReader)}  is null.")
                };

            if (DataConnectorWriter == null)
                return new ScoringProgramResponse
                {
                    RequestCommand = command,
                    DataType = DataConnectorResponseData.Error,
                    ErrorType = ErrorType.NoConnection,
                    SerializedData = JsonSerializer.Serialize($"{nameof(DataConnectorWriter)}   is null.")
                };

            return null;
        }

        /// <summary>
        /// Disposes the class.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _instance = null;
        }
    }
}
