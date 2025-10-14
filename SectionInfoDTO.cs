namespace BridgeSystems.Bridgemate.DataConnectorClasses.SharedDTO
{
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
        /// The number of the scoring group for the section.
        /// </summary>
        public int ScoringGroupNumber { get; set; }

        /// <summary>
        /// The number of tables in the section
        /// </summary>
        public int NumberOfTables { get; set; }
    }
}
