
namespace BridgeSystems.Bridgemate.DataConnector.ScoringProgramClient
{

    /// <summary>
    /// Clarifies what type of error occurred, causing the <see cref="ScoringProgramRequest">ScoringProgramRequest</see> to fail.
    /// </summary>
    public enum ErrorType
    {
        /// <summary>
        /// The is no error.
        /// </summary>
        None,

        /// <summary>
        /// The Data Connector was handling an other request.
        /// </summary>
        Busy,
        
        /// <summary>
        /// The sent data was null or did not contain any elements while the request command should come with data.
        /// </summary>
        NoData,

        /// <summary>
        /// Signals that sent data that meant to update exisiting data already existed.
        /// </summary>
        NoUpdates,
        
        /// <summary>
        /// The sent data did not comply to a known round on a table.
        /// </summary>
        Movement,

        /// <summary>
        /// The dto has invalid data.
        /// </summary>
        Validation = 5,

        /// <summary>
        /// No record for the given primary key was found.
        /// </summary>
        EntryUnknown,

        /// <summary>
        /// An error occurred when processing the data.
        /// </summary>
        Exception,

        /// <summary>
        /// The requested operation is currently not supported.
        /// </summary>
        NotImplemented,    

        /// <summary>
        /// A meaningful response to the request was exptected, but an empty response came back.
        /// </summary>
        EmptyResponse,

        /// <summary>
        /// The connection to the Data Connector is broken.
        /// </summary>
        NoConnection = 10,

        /// <summary>
        /// The operation was blocked by a previous long running operation.
        /// </summary>
        TimeOut,

        /// <summary>
        /// The datatype of the dtos did not conform to the request command.
        /// </summary>
        WrongDataType,

        /// <summary>
        /// The response command did not conform to the request command.
        /// </summary>
        UnexpectedCommand,

        /// <summary>
        /// Unknown error.
        /// </summary>
        Unknown,
    }
}