using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security;
using System.Security.Principal;
using System.Threading.Tasks;
using NLog;

namespace BridgeSystems.Bridgemate.DataConnector.ScoringProgramClient
{

    /// <summary>
    /// The base class for the <see cref="ScoringProgramPipeClient">ScoringProgram pipe client</see> and the BCS pipe client (not in this code base).
    /// Used to connect to and disconnect from the Data Connector.
    /// </summary>
    public abstract class DataConnectorPipeClient<TCommand> : IDisposable where TCommand : Enum
    {
        const int DefaultTimeOutInMilliSeconds = 5000;
        
        /// <summary>
        /// The name of the data connector logger.
        /// </summary>
        //public static string DataConnectorLoggerName => nameof(DataConnectorClientLogger);

        /// <summary>
        /// All processes below are dispoable. They can and must be disposed when the class is no longer in user. Otherwise the 
        /// communication with the Data Connector will stall.
        /// </summary>
        private NamedPipeClientStream _dataConnectorStream;
        private StreamWriter _dataConnectorWriter;
        private StreamReader _dataConnectorReader;

        /// <summary>
        /// NLog implementation of logging.
        /// </summary>
        
        //protected static readonly DataConnectorClientLogger DebugLogger = LogManager.GetLogger(nameof(DebugLogger));
        //protected static readonly DataConnectorClientLogger ErrorLogger = LogManager.GetLogger(nameof(ErrorLogger));
        public DataConnectorLogCreator<TCommand> DataConnectorClientLogger=new DataConnectorLogCreator<TCommand>(jsonDataLogLevel:DataConnectorLogLevel.Debug,nameof(DataConnectorClientLogger));
        
        /// <summary>
        /// Initializes the class.
        /// </summary>
        protected DataConnectorPipeClient()
        {
            TimeOutInMilliSeconds = 5000;
        }

        private int? _timeOutInMilliSeconds;
        private bool disposedValue;

        /// <summary>
        /// The timeout for establishing a connectio with the Data Connector.
        /// If not set the default value DefaultTimeOutInMilliSeconds will be used.
        /// </summary>
        public int TimeOutInMilliSeconds
        {
            get => _timeOutInMilliSeconds ?? DefaultTimeOutInMilliSeconds;
            set => _timeOutInMilliSeconds = value;
        }

        /// <summary>
        /// Can be used to see if there already is a connection to the Data Connector. If so, do not try to connect again.
        /// </summary>
        public bool IsActive => _dataConnectorStream?.IsConnected ?? false;
       
        /// <summary>
        /// The underlying pipe stream for both the writer and the reader. Can and must be disposed.
        /// </summary>
        protected NamedPipeClientStream DataConnectorStream => _dataConnectorStream;

        /// <summary>
        /// The class that writes data over the pipe stream.  Can and must be disposed.
        /// </summary>
        protected StreamWriter DataConnectorWriter => _dataConnectorWriter;

        /// <summary>
        /// The class that reads data from the pipe stream.  Can and must be disposed.
        /// </summary>
        protected StreamReader DataConnectorReader => _dataConnectorReader;

        /// <summary>
        /// The source for the logging: Client or Server, BCS or ScoringProgram.
        /// </summary>
        protected abstract DataConnectorLoggingSource LoggingSource { get; }

        /// <summary>
        /// Connects to the specified named pipe asynchronously.
        /// </summary>
        /// <param name="pipeName">The name of the pipe to connect to.</param>
        protected async Task<(DataConnectorResponseData result, string message, ErrorType errorType)>
            ConnectAsync(string pipeName)
        {
            LogMethodEntry(nameof(ConnectAsync),(nameof(pipeName), pipeName));
                
            try
            {
                if (_dataConnectorStream != null && _dataConnectorStream.IsConnected)
                {
                    return (DataConnectorResponseData.OK, "Client already connected.", ErrorType.None);
                }
                if (_dataConnectorStream == null)
                {
                    _dataConnectorStream = new NamedPipeClientStream(".", pipeName,
                                                          PipeDirection.InOut, PipeOptions.None,
                                                          TokenImpersonationLevel.Impersonation);
                }
                await TryConnectAsync();

                CreateStreamReaderAndWriter();

                return (DataConnectorResponseData.OK, "OK", ErrorType.None);
            }
            catch (Exception ex)
            {
                LogError(ex);

                return (DataConnectorResponseData.Error, ex.Message, ErrorType.NoConnection);
            }
        }

        /// <summary>
        /// Connects to the specified named pipe synchronously.
        /// </summary>
        /// <param name="pipeName">The name of the pipe to connect to.</param>
        protected (DataConnectorResponseData result, string message, ErrorType errorType)
          Connect(string pipeName)
        {
            LogMethodEntry(nameof(Connect), (nameof(pipeName), pipeName));

            try
            {
                if (_dataConnectorStream != null && _dataConnectorStream.IsConnected)
                {
                    return (DataConnectorResponseData.OK, "Client already connected.", ErrorType.None);
                }
                if (_dataConnectorStream == null)
                {
                    _dataConnectorStream = new NamedPipeClientStream(".", pipeName,
                                                          PipeDirection.InOut, PipeOptions.None,
                                                          TokenImpersonationLevel.Impersonation);
                }
                TryConnect();

                CreateStreamReaderAndWriter();

                return (DataConnectorResponseData.OK, "OK", ErrorType.None);
            }
            catch (Exception ex)
            {
                var errrorLogRecord = new DataConnectorLogCreator<TCommand>.DataConnectorLogRecord(
                        DataConnectorLogLevel.Debug, LoggingSource, default, jsonData: "", exception: ex);
                DataConnectorClientLogger.LogRecord(errrorLogRecord);
                return (DataConnectorResponseData.Error, ex.Message, ErrorType.NoConnection);
            }
        }

        /// <summary>
        /// Tries to connect twice synchronously. After that throws a <see cref="TimeoutException">TimeOutException</see>.
        /// </summary>
        private void TryConnect()
        {
            var counter = 0;
            var connected = false;
            do
            {
                try
                {
                    _dataConnectorStream.Connect(TimeOutInMilliSeconds);
                    connected = true;
                }
                catch (TimeoutException ex)
                {
                    counter++;
                    if (counter > 1) throw ex;
                }
            } while (!connected);
        }

        /// <summary>
        /// Tries to connect twice asynchronously. After that throws a <see cref="TimeoutException">TimeOutException</see>.
        /// </summary>
        private async Task TryConnectAsync()
        {
            var counter = 0;
            var connected = false;
            do
            {
                try
                {
                    await _dataConnectorStream.ConnectAsync(TimeOutInMilliSeconds);
                    connected = true;
                }
                catch (TimeoutException ex)
                {
                    counter++;
                    if (counter > 1) throw ex;
                }
            } while (!connected);
        }

        /// <summary>
        /// Creates the StreamReader and StreamReader classes based on the NamedPipeStream.
        /// </summary>
        private void CreateStreamReaderAndWriter()
        {
            _dataConnectorReader = new StreamReader(_dataConnectorStream);
            _dataConnectorWriter = new StreamWriter(_dataConnectorStream);
            _dataConnectorWriter.AutoFlush = true;
        }

        protected void CloseWriter()
        {
            _dataConnectorWriter?.Close();
            _dataConnectorWriter = null;
        }

        protected void CloseReader()
        {
            _dataConnectorReader?.Close();
            _dataConnectorReader = null;
        }

        protected void CloseConnection()
        {
            _dataConnectorStream?.Close();
            _dataConnectorStream = null;
        }

        /// <summary>
        /// Logs an error
        /// </summary>
        /// <param name="ex"></param>
        protected abstract void LogError(Exception ex);
      

        /// <summary>
        /// Logs the entry to a method with its parameters (if any),
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        protected void LogMethodEntry(string methodName, params (string parameterName, object parameterValue)[] parameters)
        {
            var logEntry=DataConnectorClientLogger.CreateMethodEntryLog(methodName,LoggingSource,parameters);
            LogMethodEntry(logEntry);
        }

        /// <summary>
        /// Logs the entry to a method.
        /// </summary>
        /// <param name="entry"></param>
        protected abstract void LogMethodEntry(string entry);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    CloseConnection();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}