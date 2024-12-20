using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace BridgeSystems.Bridgemate.DataConnector.ScoringProgramClient
{
    /// <summary>
    /// Logs all actions of the DataConnector in a structured way.
    /// </summary>
    public class DataConnectorLogger<TCommand> where TCommand:Enum
    {
        /// <summary>
        /// The source of the log message
        /// </summary>
        [Flags]
        public enum Source
        {
            /// <summary>
            /// Not specified
            /// </summary>
            NotSet=0,

            /// <summary>
            /// Server side action
            /// </summary>
            Server=1,

            /// <summary>
            /// Client side action
            /// </summary>
            Client=2,

            /// <summary>
            /// Action by Bridgemate Control Software
            /// </summary>
            BCS=4,

            /// <summary>
            /// Action by a scoring program that communicates with BCS through the dataconnector.
            /// </summary>
            ScoringProgram=8,

            /// <summary>
            /// BCS-Server
            /// </summary>
            BCSServer=5,

            /// <summary>
            /// BCS-Client
            /// </summary>
            BCSClient=6,

            /// <summary>
            /// ScoringProgram-Server
            /// </summary>
            ScoringProgramServer=9,

            /// <summary>
            /// ScoringProgram-Client
            /// </summary>
            ScoringProgramClient=10
        }

        /// <summary>
        /// Contains the structured information
        /// </summary>
        public struct DataConnectorLogRecord
        {
            /// <summary>
            /// Instantiates the logrecord
            /// </summary>
            /// <param name="level"></param>
            /// <param name="source"></param>
            /// <param name="command"></param>
            /// <param name="jsonData"></param>
            /// <param name="remark"></param>
            /// <param name="exception"></param>
            public DataConnectorLogRecord(LogLevel level,Source source,TCommand command,string jsonData, string remark="",Exception exception=null)
            {
                LogLevel = level;
                Source = source;
                Command = command;
                JsonData = jsonData;
                Remark = remark;
                Exception= exception;
                }
            /// <summary>
            /// The level of the logrecord.
            /// </summary>
            public LogLevel LogLevel { get; set; }  

            /// <summary>
            /// The source of the logrecord.
            /// </summary>
            public Source Source { get; set; }
            
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

        private readonly LogLevel _jsonDataLogLevel;
        private readonly Logger _nlLogLogger;

        /// <summary>
        /// Initializes the logger.
        /// </summary>
        /// <param name="jsonDataLogLevel">Specifies the lowest level of logging to include the json data as well.</param>
        public DataConnectorLogger(LogLevel jsonDataLogLevel,string name)
        {
            _jsonDataLogLevel = jsonDataLogLevel;
            _nlLogLogger=LogManager.GetLogger(name);
        }

        /// <summary>
        /// Log a structured message on a DataConnector action.
        /// </summary>
        /// <param name="record"></param>
        public void Log(DataConnectorLogRecord record) 
        {
            var message = $"{record.Source,-20} {record.Command,-20} {record.Remark}";
            _nlLogLogger.Log(record.LogLevel, message);
            if (!string.IsNullOrEmpty(record.JsonData) && _jsonDataLogLevel<=record.LogLevel)
                _nlLogLogger.Log(record.LogLevel, record.JsonData);
            if(record.Exception != null)
                _nlLogLogger.Log(LogLevel.Error, record.Exception);
        }

    }
}
