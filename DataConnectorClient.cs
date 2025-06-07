using System;
using System.Threading.Tasks;

namespace BridgeSystems.Bridgemate.DataConnector.ScoringProgramClient
{
    public abstract class DataConnectorClient<TCommand> where TCommand : Enum
    {
        public static int DefaultTimeOutInMilliSeconds = 5000;
        /// <summary>
        /// NLog implementation of logging.
        /// </summary>
        public DataConnectorLogCreator<TCommand> DataConnectorClientLogger = new DataConnectorLogCreator<TCommand>(jsonDataLogLevel: DataConnectorLogLevel.Debug, nameof(DataConnectorClientLogger));

        public abstract bool IsActive { get; }

        private int? _timeOutInMilliSeconds;
        /// <summary>
        /// The timeout for establishing a connection with the Data Connector.
        /// If not set the default value DefaultTimeOutInMilliSeconds will be used.
        /// </summary>
        public int TimeOutInMilliSeconds
        {
            get => _timeOutInMilliSeconds ?? DefaultTimeOutInMilliSeconds;
            set => _timeOutInMilliSeconds = value;
        }

        /// <summary>
        /// The source for the logging: Client or Server, BCS or ScoringProgram.
        /// </summary>
        protected abstract DataConnectorLoggingSource LoggingSource { get; }

        /// <summary>
        /// Disposes the class.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected abstract void Dispose(bool disposing);    

        protected abstract (DataConnectorResponseData result, string message, ErrorType errorType) Connect(string pipeName);
        protected abstract Task<(DataConnectorResponseData result, string message, ErrorType errorType)> ConnectAsync(string pipeName);

        /// <summary>
        /// Logs an error
        /// </summary>
        /// <param name="ex"></param>
        protected abstract void LogError(Exception ex);

        /// <summary>
        /// Logs the entry to a method.
        /// </summary>
        /// <param name="entry"></param>
        protected abstract void LogMethodEntry(string entry);


        /// <summary>
        /// Logs the entry to a method with its parameters (if any),
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        protected void LogMethodEntry(string methodName, params (string parameterName, object parameterValue)[] parameters)
        {
            var logEntry = DataConnectorClientLogger.CreateMethodEntryLog(methodName, LoggingSource, parameters);
            LogMethodEntry(logEntry);
        }
    }
}