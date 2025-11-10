using BridgeSystems.Bridgemate.DataConnector.ScoringProgramClient;
using System.Collections.Generic;
using System.Linq;

namespace BridgeSystems.Bridgemate.DataConnectorClasses.SharedDTO
{
    /// <summary>
    /// Carries information on a management request for BCS.
    /// Is used as a parameter for the <see cref="ScoringProgramDataConnectorClientCommandManager.IssueManagementCommand(BCSManagementRequestDTO)">ScoringProgramPipeClient.IssueManagementCommand</see> method 
    /// or the <see cref="ScoringProgramDataConnectorClientCommandManager.IssueManagementCommandAsync(BCSManagementRequestDTO)">ScoringProgramPipeClient.IssueManagementCommandAsync</see> method.
    /// </summary>
    public class BCSManagementRequestDTO
    {
        /// <summary>
        /// Instructs the Data Connector to issue a command to BCS to shut down.
        /// </summary>
        public const int ShutDownNow = 1;

        /// <summary>
        /// Asks the Data Connector to return an array of session guid - session name pairs for the sessions that currently are being administered.
        /// </summary>
        public const int GetRunningSessions = 2;

        /// <summary>
        /// Returns the full path to the scoring file (i.e. for back-up purposes). Can be combined with GetRunningSessions and GetDataConnectorFileLocation.
        /// </summary>
        public const int GetScoringFileLocation = 4;

        /// <summary>
        /// Asks the Data Connector to return an array of eventguid-sessionguid-session name pairs for all sessions in the scoring file.
        /// </summary>
        public const int GetAllSessionsInformation = 8;

        /// <summary>
        /// Returns the full path to the data connector messages file (i.e. for back-up purposes). Can be combined with GetRunningSessions and GetScoringFileLocaation.
        /// </summary>
        public const int GetDataConnectorFileLocation = 16;

        /// <summary>
        /// Must be (a combination of) one of the constants above.
        /// Valid are: 1 (ShutDownNow), 2 (GetRunningSessions), 4 (GetScoringFileLocation), 2+4, 8 (GetAllSessionsInformation)
        /// </summary>
        public int Command
        {
            get; set;
        }

        /// <summary>
        /// An array of messages describing problems in data integrity and invalid values.
        /// </summary>
        public string[] ValidationMessages
        {
            get; set;
        }

        /// <summary>
        /// Validates the DTO. Produces validation messages if there are problems. 
        /// </summary>
        /// <returns>True if there are no validation errors.</returns>
        public bool Validate()
        {
            var validationsErrors = new List<string>();
            if (!(Command == ShutDownNow || Command == GetRunningSessions || Command == GetScoringFileLocation ||
                  Command == GetDataConnectorFileLocation || Command == GetAllSessionsInformation ||
                  Command == GetRunningSessions + GetScoringFileLocation ||
                  Command == GetRunningSessions + GetDataConnectorFileLocation ||
                  Command == GetRunningSessions + GetScoringFileLocation+ GetDataConnectorFileLocation ||
                  Command == GetScoringFileLocation + GetDataConnectorFileLocation))
            {
                validationsErrors.Add($"Invalid Command ({Command}). " +
                    $"Valid values are 1, 2, 4, 2+4, 2+16,2+4+16, 4+16 or 8.");
            }
            ValidationMessages = validationsErrors.ToArray();
            return !validationsErrors.Any();
        }
    }
}
