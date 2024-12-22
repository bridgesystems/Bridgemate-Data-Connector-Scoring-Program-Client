using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Text.Json;

namespace BridgeSystems.Bridgemate.DataConnector.ScoringProgramClient
{

    /// <summary>
    /// Structured logger for the scoring program side of data communicator communication
    /// </summary>
    public class ScoringProgramDataConnectorLogger : DataConnectorLogger<ScoringProgramDataConnectorCommands>
    {
        /// <summary>
        /// Initializes the class.
        /// </summary>
        /// <param name="jsonDataLogLevel"></param>
        /// <param name="name"></param>
        public ScoringProgramDataConnectorLogger(DataConnectorLogLevel jsonDataLogLevel, string name) : base(jsonDataLogLevel, name)
        { }
    }

    /// <summary>
    /// The log level
    /// </summary>
    public enum DataConnectorLogLevel
    {
        /// <summary>
        /// Invalid value
        /// </summary>
        NotSet,

        /// <summary>
        /// Trace
        /// </summary>
        Trace,

        /// <summary>
        /// Debug
        /// </summary>
        Debug,

        /// <summary>
        /// Info
        /// </summary>
        Info,

        /// <summary>
        /// Warn
        /// </summary>
        Warn,

        /// <summary>
        /// Exception
        /// </summary>
        Exception
    }

    /// <summary>
    /// The source of the log message
    /// </summary>
    [Flags]
    public enum DataConnectorLoggingSource
    {
        /// <summary>
        /// Not specified
        /// </summary>
        NotSet = 0,

        /// <summary>
        /// Server side action
        /// </summary>
        Server = 1,

        /// <summary>
        /// Client side action
        /// </summary>
        Client = 2,

        /// <summary>
        /// Action by Bridgemate Control Software
        /// </summary>
        BCS = 4,

        /// <summary>
        /// Action by a scoring program that communicates with BCS through the dataconnector.
        /// </summary>
        ScoringProgram = 8,

        /// <summary>
        /// BCS-Server
        /// </summary>
        BCSServer = 5,

        /// <summary>
        /// BCS-Client
        /// </summary>
        BCSClient = 6,

        /// <summary>
        /// ScoringProgram-Server
        /// </summary>
        ScoringProgramServer = 9,

        /// <summary>
        /// ScoringProgram-Client
        /// </summary>
        ScoringProgramClient = 10
    }

    /// <summary>
    /// Logs all actions of the DataConnector in a structured way.
    /// </summary>
    public class DataConnectorLogger<TCommand> where TCommand : Enum
    {
        /// <summary>
        /// Contains the structured information
        /// </summary>
        public struct DataConnectorLogRecord
        {
            /// <summary>
            /// Returns formatted json from raw json.
            /// </summary>
            /// <param name="rawJson"></param>
            /// <returns></returns>
            public static string HumanReadableJson(string rawJson)
            {
                if (string.IsNullOrWhiteSpace(rawJson))
                    return string.Empty;

                var options = new JsonSerializerOptions()
                {
                    WriteIndented = true
                };

                var jsonElement = JsonSerializer.Deserialize<JsonElement>(rawJson);

                return JsonSerializer.Serialize(jsonElement, options);
            }

            /// <summary>
            /// Instantiates the logrecord
            /// </summary>
            /// <param name="level"></param>
            /// <param name="source"></param>
            /// <param name="command"></param>
            /// <param name="jsonData"></param>
            /// <param name="remark"></param>
            /// <param name="exception"></param>
            public DataConnectorLogRecord(DataConnectorLogLevel level, DataConnectorLoggingSource source, TCommand command, string jsonData, string remark = "", Exception exception = null)
            {
                LogLevel = level;
                Source = source;
                Command = command;
                JsonData = jsonData;
                Remark = remark;
                Exception = exception;
            }
            /// <summary>
            /// The level of the logrecord.
            /// </summary>
            public DataConnectorLogLevel LogLevel { get; set; }

            /// <summary>
            /// The source of the logrecord.
            /// </summary>
            public DataConnectorLoggingSource Source { get; set; }

            /// <summary>
            /// Either the SCoringProgramCommand enum value or the BCSComand enum value.
            /// </summary>
            public TCommand Command { get; set; }

            /// <summary>
            /// The data serialized as json
            /// </summary>
            public string JsonData { get; set; }

            /// <summary>
            /// Extra information
            /// </summary>
            public string Remark { get; set; }

            /// <summary>
            /// A possibel exception that occurred.
            /// </summary>
            public Exception Exception { get; set; }
        }

        private readonly DataConnectorLogLevel _jsonDataLogLevel;


        /// <summary>
        /// Initializes the logger.
        /// </summary>
        /// <param name="jsonDataLogLevel">Specifies the lowest level of logging to include the json data as well.</param>
        /// <param name="name">the name of the logger</param>
        public DataConnectorLogger(DataConnectorLogLevel jsonDataLogLevel, string name)
        {
            _jsonDataLogLevel = jsonDataLogLevel;
            //_nlLogLogger=LogManager.GetLogger(name);
        }

        /// <summary>
        /// LogRecord a structured message on a DataConnector action.
        /// </summary>
        /// <param name="record"></param>
        public void LogRecord(DataConnectorLogRecord record)
        {
            //var message = $"{record.Source,-20} {record.Command,-20} {record.Remark}";
            //_nlLogLogger.Log(ToNLogLogLevel( record.LogLevel), message);
            //if (!string.IsNullOrEmpty(record.JsonData) && _jsonDataLogLevel <= record.LogLevel)
            //    _nlLogLogger.Log(ToNLogLogLevel(record.LogLevel), DataConnectorLogRecord.HumanReadableJson(record.JsonData));
            //if(record.Exception != null)
            //    _nlLogLogger.Log(LogLevel.Error, record.Exception);
        }

        /// <summary>
        /// Create the content for logging
        /// </summary>
        /// <param name="record"></param>
        /// <param name="eventqueItemId"></param>
        /// <returns></returns>
        public (string message, string formattedJson) CreateRequestLogContent(DataConnectorLogRecord record, int? eventqueItemId = null)
        {
            var message = $"REQ: {record.Source,-20} {$"{record.Command}{(eventqueItemId.HasValue?"(id: " + eventqueItemId.Value + ")":"")}",-20} {record.Remark}";
            var formattedJson = string.Empty;
            if (!string.IsNullOrEmpty(record.JsonData) && _jsonDataLogLevel <= record.LogLevel)
                formattedJson = DataConnectorLogRecord.HumanReadableJson(record.JsonData);
            return (message, formattedJson);
        }

        /// <summary>
        /// Create the content for logging
        /// </summary>
        /// <param name="record"></param>
        /// <param name="dataType"></param>
        /// <param name="errorType"></param>
        /// <param name="eventqueItemId"></param>
        /// <returns></returns>
        public (string message, string formattedJson) CreateResponseLogContent(DataConnectorLogRecord record,
            DataConnectorResponseData dataType, ErrorType errorType, int? eventqueItemId = null)
        {
            var message = $"RSP: {record.Source,-20} {$"{record.Command}: {(dataType == DataConnectorResponseData.Error ? $"{dataType}: {errorType}" : $"{dataType}{(eventqueItemId.HasValue ? " (id: " + eventqueItemId.Value + ")" : "")}")}",-20}  " +
                $"{record.Remark}";
            var formattedJson = string.Empty;
            if (!string.IsNullOrEmpty(record.JsonData) && _jsonDataLogLevel <= record.LogLevel)
                formattedJson = DataConnectorLogRecord.HumanReadableJson(record.JsonData);
            return (message, formattedJson);
        }

        /// <summary>
        /// Logs an record for an exception.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="source"></param>
        public void LogError(Exception ex, DataConnectorLoggingSource source)
        {
            var errrorLogRecord = new DataConnectorLogger<TCommand>.DataConnectorLogRecord(
                    DataConnectorLogLevel.Exception, source, default, jsonData: "", exception: ex);
            LogRecord(errrorLogRecord);
        }

        /// <summary>
        /// Logs an record for an exception.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="source"></param>
        public void LogWarning(Exception ex, DataConnectorLoggingSource source)
        {
            var errrorLogRecord = new DataConnectorLogger<TCommand>.DataConnectorLogRecord(
                    DataConnectorLogLevel.Warn, source, default, jsonData: "", exception: ex);
            LogRecord(errrorLogRecord);
        }

        /// <summary>
        /// Logs the entry of a method with its parameters (if any).
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        /// <param name="source"></param>
        public void LogMethodEntry(string methodName, DataConnectorLoggingSource source, params (string parameterName, object parameterValue)[] parameters)
        {
            var parametersString = parameters.Any() ? string.Join(", ", parameters.Select(p => $"{p.parameterName}: {p.parameterValue}")) :
                                   string.Empty;
            var logRecord = new DataConnectorLogger<TCommand>.DataConnectorLogRecord(DataConnectorLogLevel.Debug, source, default,
               jsonData: "",
               remark: $"{methodName}({string.Join(", ", parametersString)})");
            LogRecord(logRecord);
        }

        private LogLevel ToNLogLogLevel(DataConnectorLogLevel dataConnectorLogLevel)
        {
            switch (dataConnectorLogLevel)
            {
                case DataConnectorLogLevel.NotSet:
                    return LogLevel.Info;
                case DataConnectorLogLevel.Trace:
                    return LogLevel.Trace;
                case DataConnectorLogLevel.Debug:
                    return LogLevel.Debug;
                case DataConnectorLogLevel.Info:
                    return LogLevel.Info;
                case DataConnectorLogLevel.Warn:
                    return LogLevel.Warn;
                case DataConnectorLogLevel.Exception:
                    return LogLevel.Error;
                default:
                    return LogLevel.Info;
            }
        }

    }
}
