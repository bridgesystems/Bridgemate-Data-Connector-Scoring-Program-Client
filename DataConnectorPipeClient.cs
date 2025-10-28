using System;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Threading.Tasks;

namespace BridgeSystems.Bridgemate.DataConnector.ScoringProgramClient
{
    /// <summary>
    /// Handles connection to the dataconnector from scoringprogram clients and the BCS client.
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    public abstract class DataConnectorPipeClientConnectionManager<TCommand> : DataConnectorClientCommandManager<TCommand>, IDisposable where TCommand : Enum
    {

        /// <summary>
        /// All processes below are dispoable. They can and must be disposed when the class is no longer in use. Otherwise the 
        /// communication with the Data Connector will stall.
        /// </summary>
        private NamedPipeClientStream _dataConnectorStream;
        private StreamWriter _dataConnectorWriter;
        private StreamReader _dataConnectorReader;

        /// <summary>
        /// Will be raised if the named pipe client has been disposed (due to an error).
        /// </summary>
        public event EventHandler<EventArgs> PipeClientDisposed;

        /// <summary>
        /// Initializes the class.
        /// </summary>
        protected DataConnectorPipeClientConnectionManager()
        {
            TimeOutInMilliSeconds = 5000;
        }

        /// <summary>
        /// Can be used to see if there already is a connection to the Data Connector. If so, do not try to connect again.
        /// </summary>
        public override bool IsActive => _dataConnectorStream?.IsConnected ?? false;

        /// <summary>
        /// The underlying pipe stream for both the writer and the reader. Can and must be disposed.
        /// </summary>
        public NamedPipeClientStream DataConnectorStream => _dataConnectorStream;

        /// <summary>
        /// The class that writes data over the pipe stream.  Can and must be disposed.
        /// </summary>
        public StreamWriter DataConnectorWriter => _dataConnectorWriter;

        /// <summary>
        /// The class that reads data from the pipe stream.  Can and must be disposed.
        /// </summary>
        public StreamReader DataConnectorReader => _dataConnectorReader;

        /// <summary>
        /// Connects to the specified named pipe asynchronously.
        /// </summary>
        /// <param name="pipeName">The name of the pipe to connect to.</param>
        public async override Task<(DataConnectorResponseData result, string message, ErrorType errorType)>
            ConnectAsync(string pipeName)
        {
            LogMethodEntry(nameof(ConnectAsync), (nameof(pipeName), pipeName));

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
        public override (DataConnectorResponseData result, string message, ErrorType errorType)
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

        /// <summary>
        /// Disposes of the StreamWriter class that sends messages to the DataConnector.
        /// </summary>
        public void CloseWriter()
        {
            _dataConnectorWriter?.Close();
            _dataConnectorWriter = null;
        }

        /// <summary>
        /// Disposes of the StreamReader class that reads messages from the DataConnector.
        /// </summary>
        public void CloseReader()
        {
            _dataConnectorReader?.Close();
            _dataConnectorReader = null;
        }

        /// <summary>
        /// Disposes of the NamedPipeClientStream that channels messages to and from the DataConnector.
        /// </summary>
        public void CloseConnection()
        {
            _dataConnectorStream?.Close();
            _dataConnectorStream = null;
        }

        private bool _disposedValue;
        /// <summary>
        /// Disposes the stream, writer and reader for the DataConnector if disposing is not already in progress.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    CloseConnection();
                }
                _disposedValue = true;
            }
            PipeClientDisposed?.Invoke(this,EventArgs.Empty);
        }
    }
}