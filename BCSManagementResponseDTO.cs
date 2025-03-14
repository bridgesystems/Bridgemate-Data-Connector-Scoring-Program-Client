﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BridgeSystems.Bridgemate.DataConnectorClasses.SharedDTO
{
    /// <summary>
    /// Contains information on sessions that BCS knows and/or is currently administering.
    /// Can only occur as a child of a BCSManagementResponseDTO.
    /// </summary>
    public class SessionInfoDTO
    {
        public SessionInfoDTO()
        {

        }
        /// <summary>
        /// The guid of the event that the session is part of. Can be the same as the session guid when there is only one session.
        /// </summary>
        public string EventGuid
        {
            get; set;
        }

        /// <summary>
        /// The guid of the session
        /// </summary>
        public string SessionGuid
        {
            get; set;
        }

        /// <summary>
        /// The name of the session
        /// </summary>
        public string SessionName
        {
            get; set;
        }

        /// <summary>
        /// The date and time that the session was scheduled to start.
        /// </summary>
        public DateTime SessionDateTime { get; set; }

        /// <summary>
        /// Signals if the InitDto of the session has been processed.
        /// </summary>
        public bool HasBeenProcessed { get; set; }
    }

    /// <summary>
    /// Carries response data for a previously sent BCSManagementRequestDTO.
    /// </summary>
    public class BCSManagementResponseDTO
    {

        /// <summary>
        /// The guid of the event currently being administered. Can be empty if no sessions are active.
        /// </summary>
        public string EventGuid
        {
            get; set;
        }

        /// <summary>
        /// Either information on the sessions currently being administered, or information on all session known to BCS.
        /// </summary>
        public SessionInfoDTO[] SessionInformation
        {
            get; set;
        }

        /// <summary>
        /// True if BCS is running.
        /// </summary>
        public bool IsRunning
        {
            get; set;
        }

        /// <summary>
        /// The location of the scoring file. Can be used for back-up purposes.
        /// </summary>
        public string ScoringFilePath
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
            var validationMessages = new List<string>();
            if (EventGuid != null && (EventGuid.Length != 32 || EventGuid.Any(c => !(c >= 'A' && c <= 'F' || c >= '0' && c <= '9'))))
            {
                validationMessages.Add("The event guid must be exactly 32 character long and can only contain capital A to F or digits 0 to 9.");
            }

            if (SessionInformation != null && SessionInformation.Any())
            {
                foreach (SessionInfoDTO sessionInfo in SessionInformation)
                {
                    if (sessionInfo.SessionGuid != null && (sessionInfo.SessionGuid.Length != 32 || sessionInfo.SessionGuid.Any(c => !(c >= 'A' && c <= 'F' || c >= '0' && c <= '9'))))
                    {
                        validationMessages.Add("The session guid must be exactly 32 character long and can only contain capital A to F or digits 0 to 9.");
                    }
                }
            }

            ValidationMessages = validationMessages.ToArray();
            return validationMessages.Any();
        }
    }
}
