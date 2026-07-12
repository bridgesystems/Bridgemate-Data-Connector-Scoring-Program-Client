namespace BridgeSystems.Bridgemate.DataConnector.ScoringProgramClient
{
    /// <summary>
    /// This class carries both a command and its associated data (if any) for the Data Connector.
    /// If the command pertains to a specific session the SessionGuid property has to be set. Validation will check in that case if
    /// the SessionGuid properties of the SerializedData has the same value.
    /// The Data Connector always responds with a <see cref="ScoringProgramResponse">ScoringProgramResponse</see>
    /// </summary>
    /// <remarks>Note that the ScoringProgramRequest itself is sent as JSON data to the Data Connector. So the SerializedData property contains
    /// nested JSON data.</remarks>
    public class ScoringProgramRequest
    {
        public ScoringProgramRequest()
        {
            ClubId=string.Empty;
            LicenceKey=string.Empty;
        }
        /// <summary>
        /// Required. The command that the Data Connector must pass on to BCS.
        /// </summary>
        public ScoringProgramDataConnectorCommands Command
        {
            get; set;
        }

        /// <summary>
        /// Required when using the http interface for the dataconnector
        /// </summary>
        public string ClubId
        { get; set; }

        /// <summary>
        /// Required when using the http interface for the dataconnector
        /// </summary>
        public string LicenceKey
        {
            get; set;
        }

        /// <summary>
        /// Required if the command pertains to a session. Must be empty otherwise.
        /// </summary>
        public string SessionGuid
        {
            get; set;
        }

        /// <summary>
        /// The data associated with the command, if any. Must be serialized as JSON.
        /// </summary>
        public string SerializedData
        {
            get; set;
        }
    }

}
