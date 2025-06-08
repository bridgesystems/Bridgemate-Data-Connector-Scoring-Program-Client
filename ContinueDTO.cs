using System.Collections.Generic;
using System.IO;
using System.Linq;
using BridgeSystems.Bridgemate.DataConnector.ScoringProgramClient;

namespace BridgeSystems.Bridgemate.DataConnectorClasses.SharedDTO
{
    /// <summary>
    /// Instructs BCS to continue working with a previously created event using the <see cref="ScoringProgramPipeClient.Continue(ContinueDTO)">ScoringProgramPipeClient.Continue</see> method 
    /// or the <see cref="ScoringProgramPipeClient.ContinueAsync(ContinueDTO)">ScoringProgramPipeClient.ContinueAsync</see> method.
    /// </summary>
    public class ContinueDTO
    {
        //The command values are the same as that for the InitDTO. Below are the values that are valid for the ContinueDTO.
        //public const int Command_StartBCS=1; //Required.
        //public const int Command_StartReading = 4; //Optional
        //public const int Command_StartMinimized=16//Optional
        //public const int Command_AutoShutDown=32//Optional
        //public const int Command_ClearData = 128; //Optional

        /// <summary>
        /// Tells BCS which event to continue.
        /// When there is only one session the event guid will be its session guid unless it was set to a different value in the original InitDTO.
        /// </summary>
        public string EventGuid
        {
            get; set;
        }

        /// <summary>
        /// 1: Just start BCS and let it reconnect to the last used scoring file if it contains a session with
        ///    the correct guid.
        /// 5: Start BCS, let it reconnect and start reading.
        /// add 128: clear all previous data from the Data Connector.
        /// </summary>
        public int Commands
        {
            get; set;
        }

        /// <summary>
        /// Optional. Specifies a different directory for the BCS scoring file. Only use in advanced scenarios.
        /// If used make sure that the directory exists.
        /// </summary>
        public string AlternativeDataFolder
        {
            get; set;
        }

        /// <summary>
        /// An array of messages describing problems in data integrity and invalid values.
        /// </summary>
        public string[] ValidationMessages;

        /// <summary>
        /// Validates the DTO. Produces validation messages if there are problems. 
        /// </summary>
        /// <returns>True if there are no validation errors.</returns>
        public bool Validate()
        {
            var validationMessages = new List<string>();
            var mask = 255 & ~InitDTO.StartBCS & ~InitDTO.Command_StartReading & ~InitDTO.Command_ClearData 
                           & ~InitDTO.Command_Minimize & ~InitDTO.Command_AutoShutDownBPC & ~InitDTO.Command_LogLevel_Debug;
            if ((Commands & mask) != 0)
            {
                validationMessages.Add($"Invalid value for {nameof(Commands)} ({Commands}). " +
                    $"Valid values are a sum of 0 and/or 1 and/or 4 and/or 128.");
            }
            if (!string.IsNullOrWhiteSpace(AlternativeDataFolder))
            {
                if (!Directory.Exists(AlternativeDataFolder))
                {
                    validationMessages.Add($"The specified alternative data folder ('{AlternativeDataFolder}' does not exist.)");
                }
            }
            ValidationMessages = validationMessages.ToArray();
            return !validationMessages.Any();
        }


    }
}
