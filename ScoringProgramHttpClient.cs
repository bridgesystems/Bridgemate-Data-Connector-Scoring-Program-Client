using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BridgeSystems.Bridgemate.DataConnectorClasses.SharedDTO;
using NLog;

namespace BridgeSystems.Bridgemate.DataConnector.ScoringProgramClient
{
    /// <summary>
    /// WARN: Currently not supported.
    /// Uses the Http protocol to communicate with the Data Connector.
    /// </summary>
    public class ScoringProgramHttpClient : IDisposable
    {
        private bool disposedValue;
        private readonly HttpClient _httpClient;
        protected static readonly Logger DebugLogger = LogManager.GetLogger(nameof(DebugLogger));
        protected static readonly Logger ErrorLogger = LogManager.GetLogger(nameof(ErrorLogger));
        private static readonly Logger Logger = LogManager.GetLogger(nameof(ScoringProgramHttpClient));
        private int _lastResultQueueItemId;
        private int _lastParticipantQueueItemId;
        private int _lastHandrecordQueueItemId;

        private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions(JsonSerializerOptions.Default)
        {
            WriteIndented = true,
        };

        public ScoringProgramHttpClient()
        {
            throw new NotImplementedException();
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// The id of the last participant queue item downloaded from the Data Connector.
        /// You can use this to internally accept this item (and all before of the same type).
        /// </summary>
        public int LastParticipantQueueItemId => _lastParticipantQueueItemId;

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

        public bool IsActive => true;

        public async Task<ScoringProgramResponse> Connect(string dbName, bool replace)
        {
            await Task.CompletedTask;
            return new ScoringProgramResponse
            {
                RequestCommand = ScoringProgramDataConnectorCommands.Connect,
                SessionGuid = "",
                DataType = DataConnectorResponseData.OK,
                ErrorType = ErrorType.None,
                SerializedData = JsonSerializer.Serialize("Not implemented for Http client.")
            };
        }

        public async Task<ScoringProgramResponse> DisconnectAsync()
        {
            await Task.CompletedTask;
            return new ScoringProgramResponse
            {
                RequestCommand = ScoringProgramDataConnectorCommands.Disconnect,
                SessionGuid = "",
                DataType = DataConnectorResponseData.OK,
                ErrorType = ErrorType.None,
                SerializedData = JsonSerializer.Serialize("Not implemented for Http client.")
            };
        }

        /// <summary>
        /// Communicates to the Data Connector to see if it is responsive.
        /// </summary>
        /// <returns></returns>
        public async Task<ScoringProgramResponse> Ping()
        {
            var requestTicks = DateTime.Now.Ticks.ToString();
            var response = await SendDataAsync(sessionGuid: string.Empty, ScoringProgramDataConnectorCommands.Ping, requestTicks);
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
            return new ScoringProgramResponse
            {
                RequestCommand = ScoringProgramDataConnectorCommands.Ping,
                DataType = requestTicks == responseTicks ? DataConnectorResponseData.OK : DataConnectorResponseData.Error,
                ErrorType = requestTicks == responseTicks ? ErrorType.None : ErrorType.Validation,
                SerializedData = JsonSerializer.Serialize(response.SerializedData)
            };
        }

        /// <summary>
        /// Instructs BCS to create a new scoring file with the provided table domain data (scoring groups, sections, tables, rounds). 
        /// Player names, Results and Handrecords can be included or can be uploaded later. The latter option is considerably less performant.
        /// </summary>
        /// <param name="sessionGraph"></param>
        /// <returns></returns>
        public Task<ScoringProgramResponse> InitiaLizeAsync(InitDTO initDTO)
        {

            var serializedData = JsonSerializer.Serialize(initDTO);
            Logger.Info($"InitDto: {serializedData}");
            return SendDataAsync("", ScoringProgramDataConnectorCommands.InitializeEvent, serializedData);
        }

        /// <summary>
        /// Instructs BCS to continue working a previously created scoring file.
        /// </summary>
        /// <param name="continueDTO">A DTO containing the session guid for the file to be revived.</param>
        /// <returns></returns>
        public Task<ScoringProgramResponse> ContinueAsync(ContinueDTO continueDTO)
        {
            var serializedData = JsonSerializer.Serialize(continueDTO);
            return SendDataAsync("", ScoringProgramDataConnectorCommands.ContinueEvent, serializedData);
        }

        /// <summary>
        /// Sends boardresults to the BCS queue
        /// </summary>
        /// <param name="dtos"></param>
        /// <param name="sessionGuid"></param>
        /// <returns></returns>
        public async Task<ScoringProgramResponse> SendResultsAsync(string sessionGuid, ResultDTO[] dtos)
        {
            var dtosForSession = dtos.Where(dto => dto.SessionGuid == sessionGuid).ToList();
            var serializedData = JsonSerializer.Serialize(dtosForSession);
            Logger.Info($"Results:{serializedData}");
            return await SendDataAsync(sessionGuid, ScoringProgramDataConnectorCommands.PutResults, serializedData);
        }

        /// <summary>
        /// Sends handrecords to the BCS queue.
        /// </summary>
        /// <param name="sessionGuid"></param>
        /// <param name="dtos"></param>
        /// <returns></returns>
        public Task<ScoringProgramResponse> SendHandrecordsAsync(string sessionGuid, HandrecordDTO[] dtos)
        {
            var dtosForSession = dtos.Where(dto => dto.SessionGuid == sessionGuid).ToList();
            var serializedData = JsonSerializer.Serialize(dtosForSession);
            return SendDataAsync(sessionGuid, ScoringProgramDataConnectorCommands.PutHandrecords, serializedData);
        }

        /// <summary>
        /// Sends PlayerData to the BCS queue.
        /// </summary>
        /// <param name="organizationId">The organization that gives out the playernumbers</param>
        /// <param name="playerData">a DTO containing the player data:playernumber, first name, last name, country code</param>
        /// <returns></returns>
        /// <remarks>
        /// Best practice is to send all player data first, even before initializing the session. After the session starts
        /// preferably the data is not changed to prevent unpredictable behaviour. By sending ParticipationDTOs you can specify
        /// which players start on which direction on which table.
        /// If all players are known before the session starts only the participants of the session need to be uploaded.
        /// If players will make themselves known at session start-up by entering their playernumber on the Bridgemate then all 
        /// playernumbers that could be entered should be uploaded.
        /// </remarks>
        public Task<ScoringProgramResponse> SendPlayerDataAsync(string organizationId, PlayerDataDTO[] playerData)
        {
            var dtosForOrganization = playerData.Where(dto => dto.SessionGuid == organizationId).ToList();
            var serializedData = JsonSerializer.Serialize(dtosForOrganization);
            //Mind: this method (ab)uses the sessionGuid parameter to pass on the clubId.
            return SendDataAsync(sessionGuid: organizationId,
                                 command: ScoringProgramDataConnectorCommands.PutPlayerData,
                                 serializedData: serializedData);
        }

        /// <summary>
        /// Sends participations to the BCS queue.
        /// </summary>
        /// <param name="dtos"></param>
        /// <param name="sessionGuid">The guid of the session that all participations must belong to.</param>
        /// <returns></returns>
        /// <remarks>
        /// Best practice is to send PlayerNumbers only. For this to work the PlayerDataDTOs have to be sent beforehand.
        /// </remarks>
        public async Task<ScoringProgramResponse> SendParticipationsAsync(string sessionGuid, ParticipationDTO[] dtos)
        {
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
        /// Polls the client queue for new boardresults.
        /// <param name="all">Poll all results for the session, included ones polled before.</paramref>
        /// </summary>
        /// <returns></returns>
        public async Task<ResultDTO[]> PollForResultsAsync(string sessionGuid, bool all = false)
        {
            var command = all ? ScoringProgramDataConnectorCommands.PollQueueForAllResults : ScoringProgramDataConnectorCommands.PollQueueForNewResults;
            var response = await SendDataAsync(sessionGuid, command, serializedData: "");
            if (response.DataType != DataConnectorResponseData.Results)
            {
                Logger.Error($"ERROR: {response.ErrorType},'{JsonSerializer.Deserialize<string>(response.SerializedData)}'");
                return new ResultDTO[] { };
            }
            var data = string.IsNullOrWhiteSpace(response.SerializedData) ? new ResultDTO[] { } :
                                                                                   JsonSerializer.Deserialize<ResultDTO[]>(response.SerializedData);
            if (data.Any())
                _lastResultQueueItemId = response.LastQueueItemId;

            return data;

        }

        /// <summary>
        /// Polls the client queue for new player data for the specified organization.
        /// </summary>
        /// <param name="all">Poll all participations for the session, included ones polled before.</paramref>
        /// <returns></returns>
        public async Task<ParticipationDTO[]> PollForParticipationsAsync(string sessionGuid, bool all = false)
        {
            var command = all ? ScoringProgramDataConnectorCommands.PollQueueForAllParticipations : ScoringProgramDataConnectorCommands.PollQueueForNewParticipations;
            var response = await SendDataAsync(sessionGuid, command, serializedData: "");
            if (response.DataType != DataConnectorResponseData.Participations)
            {
                Logger.Error($"ERROR: {response.ErrorType},'{JsonSerializer.Deserialize<string>(response.SerializedData)}'");
                return new ParticipationDTO[] { };
            }
            var data = string.IsNullOrWhiteSpace(response.SerializedData) ? new ParticipationDTO[] { } :
                                                                                           JsonSerializer.Deserialize<ParticipationDTO[]>(response.SerializedData);
            if (data.Any())
                _lastParticipantQueueItemId = response.LastQueueItemId;

            return data;

        }

        /// <summary>
        /// Polls the Data Connector for new handrecords for the session,
        /// </summary>
        /// <param name="sessionGuid"></param>
        /// <param name="all">Poll all handrecords for the session, included ones polled before.</paramref>
        /// <returns></returns>
        public async Task<HandrecordDTO[]> PollForHandrecordsAsync(string sessionGuid, bool all = false)
        {
            var command = all ? ScoringProgramDataConnectorCommands.PollQueueForAllHandrecords : ScoringProgramDataConnectorCommands.PollQueueForNewHandrecords;
            var response = await SendDataAsync(sessionGuid, command, serializedData: "");
            if (response.DataType != DataConnectorResponseData.Handrecords)
            {
                Logger.Error($"ERROR: {response.ErrorType},'{JsonSerializer.Deserialize<string>(response.SerializedData)}'");
                return new HandrecordDTO[] { };
            }
            var data = string.IsNullOrWhiteSpace(response.SerializedData) ? new HandrecordDTO[] { } :
                                                                            JsonSerializer.Deserialize<HandrecordDTO[]>(response.SerializedData);
            if (data.Any())
                _lastHandrecordQueueItemId = response.LastQueueItemId;

            return data;
        }

        /// <summary>
        /// Signals to the Data Connector that a specific type of queuedata up to the specified id does not need to be sent again.
        /// </summary>
        /// <param name="sessionGuid"></param>
        /// <param name="dataType"></param>
        /// <param name="upToId"></param>
        /// <returns></returns>
        /// <remarks>
        /// The Data Connector requires an id of the last downloaded eventqueu item of the given type. These id's are stored
        /// as fields in this class. You can also choose to make the last eventqueue id (for the given type!) a parameter of this method.
        /// </remarks>
        public async Task<ScoringProgramResponse> AcceptQueueDataAsync(string sessionGuid, DataConnectorResponseData dataType)
        {
            ScoringProgramDataConnectorCommands command;
            string serializedLastQueueItemId;

            //Determine the command to sent based on the type of queue data that is accepted.
            switch (dataType)
            {
                case DataConnectorResponseData.Results:
                    {
                        serializedLastQueueItemId = JsonSerializer.Serialize(_lastResultQueueItemId);
                        command = ScoringProgramDataConnectorCommands.AcceptResultQueueItems; break;
                    }
                case DataConnectorResponseData.Participations:
                    {
                        serializedLastQueueItemId = JsonSerializer.Serialize(_lastParticipantQueueItemId);
                        command = ScoringProgramDataConnectorCommands.AcceptParticipantQueueItems; break;
                    }
                case DataConnectorResponseData.Handrecords:
                    {
                        serializedLastQueueItemId = JsonSerializer.Serialize(_lastHandrecordQueueItemId);
                        command = ScoringProgramDataConnectorCommands.AcceptHandrecordQueueItems; break;
                    }
                default:
                    return new ScoringProgramResponse
                    {
                        RequestCommand = ScoringProgramDataConnectorCommands.None,
                        DataType = DataConnectorResponseData.Error,
                        ErrorType = ErrorType.Validation,
                        SerializedData = JsonSerializer.Serialize($"Invalid datatype '{dataType}'")
                    };
            }
            return await SendDataAsync(sessionGuid, command, serializedLastQueueItemId);

        }

        /// <summary>
        /// Returns the movement for the given section as it is in use by BCS. This data can be used to create movement updates.
        /// </summary>
        /// <param name="sessionGuid"></param>
        /// <param name="sectionLetters"></param>
        /// <returns></returns>
        public async Task<SectionDTO> GetMovementAsync(string sessionGuid, string sectionLetters)
        {
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
                    Logger.Error($"The {ScoringProgramDataConnectorCommands.GetMovement} command returned an unexpected result: '{response.RequestCommand}'");
                    return null;
                }
                if (response.DataType != DataConnectorResponseData.Movement)
                {
                    Logger.Error($"The {ScoringProgramDataConnectorCommands.GetMovement} failed: " +
                                 $"{response.DataType}-{response.ErrorType}: '{JsonSerializer.Deserialize<string>(response.SerializedData)}'");
                    return null;
                }
                var section = JsonSerializer.Deserialize<SectionDTO>(response.SerializedData);
                return section;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return null;
            }
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

        /// <summary>
        /// The code that handles the actual sending of requests and reading their reponses.
        /// </summary>
        /// <param name="sessionGuid">Specifies which session the request targets (if any)</param>
        /// <param name="command">The command to the middlleman</param>
        /// <param name="serializedData">The data to send to the Data Connector as json data. (If any)</param>
        /// <returns></returns>
        private async Task<ScoringProgramResponse> SendDataAsync(string sessionGuid, ScoringProgramDataConnectorCommands command, string serializedData)
        {
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

                //Send the request to the Data Connector and await the response.
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"http://localhost:5079/middleman");
                var content = new StringContent(requestSerialized, Encoding.UTF8, "application/json");
                requestMessage.Content = content;
                var httpResponse = await _httpClient.SendAsync(requestMessage);
                var json = await httpResponse.Content.ReadAsStringAsync();

                var clientResponse = JsonSerializer.Deserialize<ScoringProgramResponse>(json);
                return clientResponse ??
                             new ScoringProgramResponse
                             {
                                 RequestCommand = command,
                                 DataType = DataConnectorResponseData.Error,
                                 SerializedData = JsonSerializer.Serialize("Empty response")
                             };

            }
            catch (Exception ex)
            {
                DebugLogger.Error(ex);
                ErrorLogger.Error(ex);
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



        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _httpClient.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ScoringProgramHttpClient()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
