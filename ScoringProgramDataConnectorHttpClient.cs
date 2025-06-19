using NLog;
using System;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace BridgeSystems.Bridgemate.DataConnector.ScoringProgramClient
{
    /// <summary>
    /// Exchanges information with the dataconnector using http.
    /// </summary>
    public class ScoringProgramDataConnectorHttpClient : ScoringProgramDataConnectorClientCommandManager, IScoringProgramClient
    {
        public const string ProductionUrlWithouProtocol = "bridgematedataconnector-a6bndyc3gwgmhydq.germanywestcentral-01.azurewebsites.net";
        public const string ProductionUrl= "https://"+ ProductionUrlWithouProtocol;
        public const string LocalHostUrlWithoutProtocol = "localhost:5079";
        public const string LocalHostUrl = "http://" + LocalHostUrlWithoutProtocol;
        
        public static bool UseLocalHost { get; set; }
        
        public static string ApiUrlRoot=>UseLocalHost ? LocalHostUrl : ProductionUrl;
        public static string ApiUrlRootWihtoutProtocol = UseLocalHost ? LocalHostUrlWithoutProtocol : ProductionUrlWithouProtocol;
        
            /// <summary>
        /// The url to call when the scoring program communicates with the data connector.
        /// </summary>
        public const string ApiCall = "dc-scoringprogram";

        /// <summary>
        /// The debug logger
        /// </summary>
        protected static readonly Logger DebugLogger = LogManager.GetLogger(nameof(DebugLogger));
        
        /// <summary>
        /// The error logger
        /// </summary>
        protected static readonly Logger ErrorLogger = LogManager.GetLogger(nameof(ErrorLogger));
        private static readonly Logger Logger = LogManager.GetLogger(nameof(ScoringProgramDataConnectorHttpClient));

        private static ScoringProgramDataConnectorHttpClient _instance;
        public static ScoringProgramDataConnectorHttpClient Instance(string clubId, string licenceKey)
        {
                if(_instance==null)
                    _instance=new ScoringProgramDataConnectorHttpClient(clubId,licenceKey);
                _instance.Credentials=(clubId,licenceKey);
                return _instance;
        }

        private ScoringProgramDataConnectorHttpClient(string clubdId, string licenceKey)
        {
            Credentials = (clubdId, licenceKey);
        }

        /// <summary>
        /// The information needed to get access to the http channel for the data communicator.
        /// </summary>
        public (string clubId, string licenceKey) Credentials { get; set; }


        private bool disposedValue;

        /// <summary>
        /// Always returns true.
        /// </summary>
        public override bool IsActive => true;

        /// <summary>
        /// Not implemented for http.
        /// </summary>
        /// <returns></returns>
        public ScoringProgramResponse Connect()
        {
            return new ScoringProgramResponse
            {
                RequestCommand = ScoringProgramDataConnectorCommands.Connect,
                SessionGuid = "",
                DataType = DataConnectorResponseData.Error,
                ErrorType = ErrorType.NotImplemented,
                SerializedData = JsonSerializer.Serialize("Not implemented for Http client.")
            };
        }

        /// <summary>
        /// Not implemented for http.
        /// </summary>
        /// <returns></returns>
        public async Task<ScoringProgramResponse> ConnectAsync()
        {
            Ping ping = new Ping();
            PingReply reply = await ping.SendPingAsync(ApiUrlRootWihtoutProtocol);

            return new ScoringProgramResponse
            {
                RequestCommand = ScoringProgramDataConnectorCommands.Connect,
                DataType = reply.Status == IPStatus.Success ? DataConnectorResponseData.OK : DataConnectorResponseData.Error,
                ErrorType =reply.Status== IPStatus.Success? ErrorType.None:ErrorType.NoConnection,
                SerializedData = JsonSerializer.Serialize(reply.Status)
            };
        }

        /// <summary>
        /// Not implemented for http.
        /// </summary>
        /// <param name="connectionName"></param>
        /// <returns></returns>
        public override (DataConnectorResponseData result, string message, ErrorType errorType) Connect(string connectionName)
        {
            return (DataConnectorResponseData.None, "Not implemented", ErrorType.NotImplemented);
        }


        /// <summary>
        /// Not implemented for http.
        /// </summary>
        /// <param name="connectionName"></param>
        /// <returns></returns>
        public override async Task<(DataConnectorResponseData result, string message, ErrorType errorType)> ConnectAsync(string connectionName)
        {
            await Task.CompletedTask;
            return (DataConnectorResponseData.None, "Not implemented", ErrorType.NotImplemented);
        }

        /// <summary>
        /// Not implemented for http.
        /// </summary>
        /// <returns></returns>
        public async Task<ScoringProgramResponse> DisconnectAsync()
        {
            await Task.CompletedTask;
            return new ScoringProgramResponse
            {
                RequestCommand = ScoringProgramDataConnectorCommands.Disconnect,
                SessionGuid = "",
                DataType = DataConnectorResponseData.OK,
                ErrorType = ErrorType.NotImplemented,
                SerializedData = JsonSerializer.Serialize("Not implemented for Http client.")
            };
        }

        /// <summary>
        /// Not implemented for http.
        /// </summary>
        /// <returns></returns>
        public ScoringProgramResponse Disconnect()
        {
            return new ScoringProgramResponse
            {
                RequestCommand = ScoringProgramDataConnectorCommands.Disconnect,
                SessionGuid = "",
                DataType = DataConnectorResponseData.OK,
                ErrorType = ErrorType.NotImplemented,
                SerializedData = JsonSerializer.Serialize("Not implemented for Http client.")
            };
        }

        /// <summary>
        /// The code that handles the actual sending of requests and reading their reponses.
        /// </summary>
        /// <param name="sessionGuid">Specifies which session the request targets (if any)</param>
        /// <param name="command">The command to the middlleman</param>
        /// <param name="serializedData">The data to send to the Data Connector as json data. (If any)</param>
        /// <returns></returns>
        protected override async Task<ScoringProgramResponse> SendDataAsync(string sessionGuid,
            ScoringProgramDataConnectorCommands command,
            string serializedData)
        {
            //Construct the request to the Data Connector.
            var request = new ScoringProgramRequest
            {
                Command = command,
                SessionGuid = sessionGuid,
                SerializedData = serializedData,
                ClubId=Credentials.clubId,
                LicenceKey=Credentials.licenceKey
            };

           

            //Do not proceed if sending is already in progress (for an other request). There can be only on request be sent at the same time.
            if (IsSending)
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
                IsSending = true;
                
                //Serialize
                var requestSerialized = JsonSerializer.Serialize(request);
                using (var httpClient = new HttpClient())
                {
                    var retryCounter = 5;
                    while (retryCounter > 0)
                    {
                        //Send the request to the Data Connector and await the response.
                        var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{ApiUrlRoot}/{ApiCall}");
                        var content = new StringContent(requestSerialized, Encoding.UTF8, "application/json");
                        requestMessage.Content = content;
                        HttpResponseMessage httpResponse=null;
                        try
                        {
                            httpResponse = await httpClient.SendAsync(requestMessage);
                        }
                        catch (Exception ex) 
                        {
                            Logger.Error(ex);
                            retryCounter--;
                            await Task.Delay(10000 - retryCounter * 200);
                        }
                        if (httpResponse != null)
                        {
                            if (httpResponse.IsSuccessStatusCode)
                            {
                                var json = await httpResponse.Content.ReadAsStringAsync();
                                retryCounter = 0;
                                var clientResponse = JsonSerializer.Deserialize<ScoringProgramResponse>(json);
                                return clientResponse ??
                                             new ScoringProgramResponse
                                             {
                                                 RequestCommand = command,
                                                 DataType = DataConnectorResponseData.Error,
                                                 SerializedData = JsonSerializer.Serialize("Empty response")
                                             };

                            }
                            else
                            {
                                retryCounter--;
                                var errorMessage = httpResponse.ReasonPhrase;
                                Logger.Error(errorMessage);
                                await Task.Delay(10000 - retryCounter * 200);
                            }
                        }
                    }
                    return new ScoringProgramResponse
                    {
                        RequestCommand = command,
                        DataType = DataConnectorResponseData.Error,
                        ErrorType = ErrorType.NoConnection
                    };
                }
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
                IsSending = false;
            }
        }

        /// <summary>
        /// The code that handles the actual sending of requests and reading their reponses.
        /// </summary>
        /// <param name="sessionGuid">Specifies which session the request targets (if any)</param>
        /// <param name="command">The command to the middlleman</param>
        /// <param name="serializedData">The data to send to the Data Connector as json data. (If any)</param>
        /// <returns></returns>
        protected override  ScoringProgramResponse SendData(string sessionGuid,
            ScoringProgramDataConnectorCommands command,
            string serializedData)
        {
            //Construct the request to the Data Connector.
            var request = new ScoringProgramRequest
            {
                Command = command,
                SessionGuid = sessionGuid,
                SerializedData = serializedData,
                ClubId = Credentials.clubId,
                LicenceKey = Credentials.licenceKey
            };



            //Do not proceed if sending is already in progress (for an other request). There can be only on request be sent at the same time.
            if (IsSending)
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
                IsSending = true;

                //Serialize
                var requestSerialized = JsonSerializer.Serialize(request);
                using (var httpClient = new HttpClient())
                {
                    var retryCounter = 5;
                    while (retryCounter > 0)
                    {
                        //Send the request to the Data Connector and await the response.
                        var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{ApiUrlRoot}/{ApiCall}");
                        var content = new StringContent(requestSerialized, Encoding.UTF8, "application/json");
                        requestMessage.Content = content;
                        HttpResponseMessage httpResponse = null;
                        try
                        {
                            httpResponse = httpClient.SendAsync(requestMessage).ConfigureAwait(false).GetAwaiter().GetResult();
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                            retryCounter--;
                            Thread.Sleep(10000 - retryCounter * 200);
                        }
                        if (httpResponse != null)
                        {
                            if (httpResponse.IsSuccessStatusCode)
                            {
                                var json = httpResponse.Content.ReadAsStringAsync().Result;
                                retryCounter = 0;
                                var clientResponse = JsonSerializer.Deserialize<ScoringProgramResponse>(json);
                                return clientResponse ??
                                             new ScoringProgramResponse
                                             {
                                                 RequestCommand = command,
                                                 DataType = DataConnectorResponseData.Error,
                                                 SerializedData = JsonSerializer.Serialize("Empty response")
                                             };

                            }
                            else
                            {
                                retryCounter--;
                                var errorMessage = httpResponse.ReasonPhrase;
                                Logger.Error(errorMessage);
                                Thread.Sleep(10000 - retryCounter * 200);
                            }
                        }
                    }
                    return new ScoringProgramResponse
                    {
                        RequestCommand = command,
                        DataType = DataConnectorResponseData.Error,
                        ErrorType = ErrorType.NoConnection
                    };
                }
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
                IsSending = false;
            }
        }




        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {

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
