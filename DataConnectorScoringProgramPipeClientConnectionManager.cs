using NLog;
using System;

namespace BridgeSystems.Bridgemate.DataConnector.ScoringProgramClient
{
    /// <summary>
    /// Handles connections to the dataconnector for scoring programs.
    /// </summary>
    public class DataConnectorScoringProgramPipeClientConnectionManager : DataConnectorPipeClientConnectionManager<ScoringProgramDataConnectorCommands>
    {
        /// <summary>
        /// Returns the logging source.
        /// </summary>
        public override DataConnectorLoggingSource LoggingSource => DataConnectorLoggingSource.ScoringProgramClient;

        /// <summary>
        /// The logger.
        /// </summary>
        public static readonly Logger ScoringProgramClientLogger = LogManager.GetLogger(nameof(ScoringProgramClientLogger));

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="ex"></param>
        protected override void LogError(Exception ex)
        {
            ScoringProgramClientLogger.Error(ex);
        }

        /// <summary>
        /// Logs entry of a method.
        /// </summary>
        /// <param name="entry"></param>
        protected override void LogMethodEntry(string entry)
        {
            ScoringProgramClientLogger.Debug(entry);
        }
    }
}