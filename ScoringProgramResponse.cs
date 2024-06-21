namespace BridgeSystems.Bridgemate.DataConnector.ScoringProgramClient
{
    /// <summary>
    /// This class carries the response data that the Data Connector sends back after sending it a <see cref="ScoringProgramRequest">ScoringProgramRequest</see>.
    /// </summary>
    public class ScoringProgramResponse
    {
        /// <summary>
        /// The command of the request that this is the reponse for.
        /// </summary>
        public ScoringProgramDataConnectorCommands RequestCommand
        {
            get; set;
        }

        /// <summary>
        /// Specifies the datatype to deserialize to after rececption of the response.
        /// Data ConnectorResponseData.Error signals that something went wrong. The serialized data will be a string
        /// describing the error.
        /// </summary>
        public DataConnectorResponseData DataType
        {
            get; set;
        }

        /// <summary>
        /// If the response contains queue data the id of last added queueitem.
        /// </summary>
        public int LastQueueItemId
        {
            get; set;
        }

        /// <summary>
        /// Specifies the type of error that occurred. ErrorType.None if all went well.
        /// </summary>
        public ErrorType ErrorType
        {
            get; set;
        }

        /// <summary>
        /// The guid of the session the response belongs to.
        /// </summary>
        public string SessionGuid
        {
            get; set;
        }

        /// <summary>
        /// The serialized data. The DataType property tells to what class it should be deserialized.
        /// </summary>
        public string SerializedData
        {
            get; set;
        }
    }
}
