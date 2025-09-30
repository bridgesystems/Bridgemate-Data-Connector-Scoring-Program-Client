using System;

namespace BridgeSystems.Bridgemate.DataConnectorClasses.SharedDTO
{
    /// <summary>
    /// Contains information on sessions that BCS knows and/or is currently administering.
    /// Can only occur as a child of a BCSManagementResponseDTO.
    /// </summary>
    public class SessionInfoDTO
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SessionInfoDTO"/> class.
        /// </summary>
        public SessionInfoDTO()
        {
            Sections=Array.Empty<SectionInfoDTO>();
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

        /// <summary>
        /// Information on the sections within the session.
        /// </summary>
        public SectionInfoDTO[] Sections { get; set; }
    }

    /// <summary>
    /// Caries information on a section within a session.
    /// </summary>
    public class SectionInfoDTO
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SectionInfoDTO"/> class.
        /// </summary>
        public SectionInfoDTO()
        {
            SectionName = string.Empty;
            SectionLetters= string.Empty;
        }

        /// <summary>
        /// The name of the section
        /// </summary>
        public string SectionName { get; set; }

        /// <summary>
        /// The letters assigned to the section
        /// </summary>
        public string SectionLetters { get; set; }

        /// <summary>
        /// The number of tables in the section
        /// </summary>
        public int NumberOfTables { get; set; }
    }
}
