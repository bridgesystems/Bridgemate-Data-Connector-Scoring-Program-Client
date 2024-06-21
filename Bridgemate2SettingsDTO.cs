using System.Collections.Generic;
using System.Linq;

namespace BridgeSystems.Bridgemate.DataConnectorClasses.SharedDTO
{
 
    #region Bridgemate 2 settings
    /// <summary>
    /// How to represent the contract denomination
    /// </summary>
    public enum Bm2ContractRepresentationOption
    {
        /// <summary>
        /// Use the symbols for clubs, diamonds, hearts and spades
        /// </summary>
        UseSymbols = 1,

        /// <summary>
        /// Use letters for the denominations
        /// </summary>
        UseLetter = 0
    }

    /// <summary>
    /// Show the pointscore sign (plus or minus) from the perspective of declarer or North-South. 
    /// </summary>
    public enum Bm2PointScorePerspectiveOption
    {
        /// <summary>
        /// Show the pointscore sign (plus or minus) from the perspective of North-South. 
        /// </summary>
        FromNorthSouth = 0,

        /// <summary>
        /// Show the pointscore sign (plus or minus) from the perspective of declarer. 
        /// </summary>
        FromDeclarer = 1
    }

    /// <summary>
    /// How to enter the resulting tricks of a contract
    /// </summary>
    public enum BM2ResultEntryMethod
    {
        /// <summary>
        /// The number of tricks above or below the level of the contract.
        /// </summary>
        UpDownTricks = 0,

        /// <summary>
        /// The total number of tricks.
        /// </summary>
        NumberOfTricks,

        /// <summary>
        /// American style.
        /// </summary>
        AmericanStyle
    }


    /// <summary>
    /// How the results on a board should be formatted.
    /// </summary>
    public enum Bm2ResultsOverviewOption
    {
        /// <summary>
        /// Group the results by pointscore and declarer:
        /// Max 6 lines, 1 column.
        /// </summary>
        FrequencyList6lines1column = 0,

        /// <summary>
        /// Group the results by pointscore and declarer:
        /// Max 6 lines of 2 columns.
        /// </summary>
        FrequencyList6lines2columns = 1,

        /// <summary>
        /// Group the results by pointscore and declarer:
        /// Max 4 lines, 1 column.
        /// </summary>
        FrequencyList4lines1column = 2,

        /// <summary>
        /// Show the result for each couple of competing pairs:
        /// Max 6 lines, 1 column.
        /// </summary>
        Traveler6lines1column = 3,

        /// <summary>
        /// Show the result for each couple of competing pairs:
        /// Max 6 lines, 2 columns.
        /// </summary>
        Traveler6lines2columns = 4,

        /// <summary>
        /// Show the result for each couple of competing pairs:
        /// Max 4 lines, 1 column.
        /// </summary>
        Traveler4lines1column = 5
    }

    /// <summary>
    /// Specifies if the pairnumber should be part of entering the contract.
    /// </summary>
    public enum Bm2PairNumberEntryOption
    {
        /// <summary>
        /// The pairnumber cannot be entered.
        /// </summary>
        NoEntry = 0,

        /// <summary>
        /// The pairnumber entry is optional.
        /// </summary>
        Optional = 1,

        /// <summary>
        /// The pairnumber entry is required.
        /// </summary>
        Required = 2
    }

    /// <summary>
    /// Specifies for which organization an entered playernumber should be validated.
    /// </summary>
    public enum Bm2NumberValidationOption
    {
        /// <summary>
        /// No validation.
        /// </summary>
        NoValidation = 0,

        /// <summary>
        /// Validate for the ACBL (USA).
        /// </summary>
        ACBL = 1,

        /// <summary>
        /// Validate for the FFB (France).
        /// </summary>
        FFB = 2,

        /// <summary>
        /// Validate for the ABF (Australia).
        /// </summary>
        ABF = 3,

        /// <summary>
        /// Validate for the JCBL (Japanese).
        /// </summary>
        JCBL = 4
    }

    /// <summary>
    /// Specifies which source data should be consulted to look up player names for player numbers.
    /// </summary>
    public enum Bm2NameSource
    {
        /// <summary>
        /// The PlayerNames table of the .bws scoring file.
        /// </summary>
        PlayerNamesTable = 0,

        /// <summary>
        /// A separate MS-Access table.
        /// </summary>
        BMPlayerDB = 1,

        /// <summary>
        /// No External source.
        /// </summary>
        NoExternalSource = 2,

        /// <summary>
        /// First check the PlayerNames table then the external MS-Access file.
        /// </summary>
        PlayerNamesTableThenBMPlayerDB = 3
    }

    /// <summary>
    /// Specifies whether and when to show the names of the competing pairs.
    /// </summary>
    public enum Bm2ShowPlayerNamesOption
    {
        /// <summary>
        /// Do not show.
        /// </summary>
        DoNotShow = 0,

        /// <summary>
        /// Show in all rounds.
        /// </summary>
        ShowInAllRounds = 1,

        /// <summary>
        /// Show in first round only.
        /// </summary>
        ShowInFirstRoundOnly = 2
    }

    /// <summary>
    /// Specifies whether and when to show the ranking.
    /// </summary>
    public enum Bm2ShowRankingOption
    {
        /// <summary>
        /// Do not show.
        /// </summary>
        DoNotShow = 0,

        /// <summary>
        /// Show the ranking after each round.
        /// </summary>
        ShowAllRounds = 1,

        /// <summary>
        /// Show the ranking after the last round only.
        /// </summary>
        ShowAfterLastRound = 2
    }

    /// <summary>
    /// Specifies whether and how to show the summary points after the last round.
    /// </summary>
    public enum Bm2SummaryPointOption
    {
        /// <summary>
        /// Do not show.
        /// </summary>
        NoSummary = -1,

        /// <summary>
        /// Show the scores as matchpoints.
        /// </summary>
        Matchpoints = 0,

        /// <summary>
        /// Show the scores a percentages.
        /// </summary>
        Percentage = 1
    }

    /// <summary>
    /// Specifies whether and when the players can enter the handrecord for a board.
    /// </summary>
    public enum Bm2EnterHandrecordWhenOption
    {
        /// <summary>
        /// The handrecord can be entered at the end of each round.
        /// </summary>
        AtEndOfRound = 0,

        /// <summary>
        /// The handrecord can be entered after the play of each board.
        /// </summary>
        AtEndOfBoard = 1,

        /// <summary>
        /// The handrecord cannot be entered.
        /// </summary>
        DoNotEnter = 2
    }

    /// <summary>
    /// Specifies whether and how scores obtained in the round can be viewed again.
    /// </summary>
    public enum Bm2ScoreRecapOption
    {
        /// <summary>
        /// The scores for a round cannot be viewed again.
        /// </summary>
        NoScoreRecap = 0,

        /// <summary>
        /// The scores for a round can be viewed again using a button on the Bridgemate.
        /// </summary>
        UseScoreRecap,

        /// <summary>
        /// The scores for a round will be shown automatically at the end of the round.
        /// </summary>
        AutomaticScoreRecap
    }

    /// <summary>
    /// Specifes if the boardnumbers should be filled in automatically.
    /// </summary>
    public enum Bm2AutoBoardNumberOption
    {
        /// <summary>
        /// The boardnumber is not filled in automatically.
        /// </summary>
        NoAutoBoardNumber = 0,

        /// <summary>
        /// The boardnumber is filled in automatically startin with the lowest boardnumber for the round.
        /// </summary>
        AutoBoardNumber,

        /// <summary>
        /// The first boardnumber must be entered by hand, the numbers for the following boards will be filled in automatically.
        /// </summary>
        FirstBoardManually
    }

    #endregion

    /// <summary>
    /// Contains all settings for the Bridgemate 2 for a section.
    /// Mind that all values must be supplied.
    /// </summary>
    public class Bridgemate2SettingsDTO : BridgemateSettingsDTO
    {
        public Bridgemate2SettingsDTO()
        {
        }

        /// <summary>
        /// Show results obtained on other rounds and on other tables.
        /// </summary>
        public bool ShowResults
        {
            get; set;
        }

        /// <summary>
        /// Include the own result in the results overview.
        /// </summary>
        public bool ShowOwnResult
        {
            get; set;
        }

        /// <summary>
        /// Ask after showing the results if the overview should be repeated.
        /// </summary>
        public bool RepeatResults
        {
            get; set;
        }

        /// <summary>
        /// Show percentage rather than matchpoints.
        /// </summary>
        public bool ShowPercentage
        {
            get; set;
        }

        /// <summary>
        /// Currently not supported. Use the ScoringGroupDTO to group sections.
        /// </summary>
        public bool GroupSections
        {
            get; set;
        }

        /// <summary>
        /// Show the point score of a result from the declarer's perspective or from that of North-South.
        /// </summary>
        public Bm2PointScorePerspectiveOption ScorePoints
        {
            get; set;
        }

        /// <summary>
        /// Defines the way resulting tricks are enterd after the board has been played.
        /// </summary>
        public BM2ResultEntryMethod EnterResultsMethod
        {
            get; set;
        }

        /// <summary>
        /// Determines if the pairnumbers are shown at the top of the Bridgemate.
        /// </summary>
        public bool ShowPairNumbers
        {
            get; set;
        }

        /// <summary>
        /// Currently not supported.
        /// </summary>
        public bool IntermediateResults
        {
            get; set;
        }

        /// <summary>
        /// The time in seconds before the Bridgemate will power off.
        /// </summary>
        public int AutopoweroffTime
        {
            get; set;
        }

        /// <summary>
        /// The time in seconds that the message that the opponents must confirm the entered result is shown.
        /// </summary>
        public int VerificationTime
        {
            get; set;
        }

        /// <summary>
        /// Show the denomination of the contract as letters or as suit symbols.
        /// </summary>
        public Bm2ContractRepresentationOption ShowContract
        {
            get; set;
        }

        /// <summary>
        /// Defines if the leadcard should be entered.
        /// </summary>
        public bool LeadCard
        {
            get; set;
        }

        /// <summary>
        /// If ShowResults is "True" determines the maximum number of results that will be shown.
        /// </summary>
        public int MaximumResults
        {
            get; set;
        }

        /// <summary>
        /// Defines if the players can make themselves know by entering their player number on the Bridgemate.
        /// </summary>
        public bool MemberNumbers
        {
            get; set;
        }

        /// <summary>
        /// If "True" all players must have been identified by their player number.
        /// </summary>
        public bool MemberNumbersNoBlankEntry
        {
            get; set;
        }

        /// <summary>
        /// Defines if the Bridgemate should check the order of play.
        /// </summary>
        public bool BoardOrderVerification
        {
            get; set;
        }

        /// <summary>
        /// Currently not supported.
        /// </summary>
        public bool HandRecordValidation
        {
            get; set;
        }

        /// <summary>
        /// Defines if BCS should automatically shut down when all results are in.
        /// </summary>
        public bool AutoShutDownBPC
        {
            get; set;
        }

        /// <summary>
        /// The pincode that the TD must enter before accessing the TD menu on the Bridgemate.
        /// Must be four digits.
        /// </summary>
        public string BM2PINcode
        {
            get; set;
        }

        /// <summary>
        /// Defines if the TD must confirm No Play on the Bridgemate.
        /// </summary>
        public bool BM2ConfirmNP
        {
            get; set;
        }

        /// <summary>
        /// Defines if the Bridgemate should show how many boards in the round are left to play.
        /// </summary>
        public bool BM2RemainingBoards
        {
            get; set;
        }

        /// <summary>
        /// Defines if the Bridgemate should show the seatings for the next round.
        /// </summary>
        public bool BM2NextSeatings
        {
            get; set;
        }
      
        /// <summary>
        /// Defines if the Bridgemate can show the scores obtained during the round when a player presses the appropriate button on the Bridgemate.
        /// </summary>
        public bool BM2ScoreRecap
        {
            get; set;
        }

        /// <summary>
        /// Defines if the Bridgemate should show the score recap wihtout promptimg after the last board of the round has been played..
        /// </summary>
        public bool BM2AutoShowScoreRecap
        {
            get; set;
        }

        /// <summary>
        /// Defines if the players can correct scores themselves. If not the TD must be called to the table to do this.
        /// </summary>
        public bool BM2ScoreCorrection
        {
            get; set;
        }

        /// <summary>
        /// Defines if the Brigemate should enter the board number automatically..
        /// </summary>
        public bool BM2AutoBoardNumber
        {
            get; set;
        }

        /// <summary>
        /// If BM2AutoBoardNumber is "True" defines if the boardnumber of the first board should be entered manually. The subsequent boards will be filled in automatically.
        /// </summary>
        public bool BM2FirstBoardManually
        {
            get; set;
        }

        /// <summary>
        /// Determines if the entry of the leadcard should be checked against the handrecord.
        /// </summary>
        public bool BM2ValidateLeadCard
        {
            get; set;
        }

        /// <summary>
        /// Determines how the results overview will be formatted on the display.
        /// </summary>
        public Bm2ResultsOverviewOption BM2ResultsOverview
        {
            get; set;
        }

        /// <summary>
        /// Determines if the Bridgemate shows the names of the players and if so at the start of each round or in the first round only.
        /// </summary>
        public Bm2ShowPlayerNamesOption BM2ShowPlayerNames
        {
            get; set;
        }

        /// <summary>
        /// Determines if the Bridgemate should show the ranking and if so after play of the last board of a round or after play of the last board of the session..
        /// </summary>
        public Bm2ShowRankingOption BM2Ranking
        {
            get; set;
        }

        /// <summary>
        /// Enable/disable the summary after end of session. (Enabled requires BM2Ranking=1 or 2
        /// </summary>
        public bool BM2GameSummary
        {
            get; set;
        }

        /// <summary>
        /// Defines if matchpoints or the percentage should be shown in the game summary.
        /// </summary>
        public Bm2SummaryPointOption BM2SummaryPoints
        {
            get; set;
        }

        /// <summary>
        /// Defines if the players can or must enter the pair number when entering the declaring side for a contract.
        /// Enforcing this helps in detecting switched seatings.
        /// </summary>
        public Bm2PairNumberEntryOption BM2PairNumberEntry
        {
            get; set;
        }

        /// <summary>
        /// Determines if the "Reset" key is available to the players, rather than to the TD only.
        /// </summary>
        public bool BM2ResetFunctionKey
        {
            get; set;
        }

        /// <summary>
        /// Currently not supported.
        /// </summary>
        public bool BM2RecordBidding
        {
            get; set;
        }

        /// <summary>
        /// Currently not supported.
        /// </summary>
        public bool BM2RecordPlay
        {
            get; set;
        }

        /// <summary>
        /// Currently not supported.
        /// </summary>
        public bool BM2ValidateRecording
        {
            get; set;
        }

        /// <summary>
        /// Currently not suppoorted.
        /// </summary>
        public bool BM2ShowHands
        {
            get; set;
        }

        /// <summary>
        /// Currently not supported
        /// </summary>
        public Bm2NumberValidationOption NumberValidation
        {
            get; set;
        }

        /// <summary>
        /// Defines if the players must enter their player number each round.
        /// </summary>
        public bool BM2NumberEntryEachRound
        {
            get; set;
        }

        /// <summary>
        /// Currently not supported.
        /// </summary>
        public bool BM2NumberEntryPreloadValues
        {
            get; set;
        }

        /// <summary>
        /// Currently not supported.
        /// </summary>
        public Bm2NameSource BM2NameSource
        {
            get; set;
        }

        /// <summary>
        /// Determines if the players can see the handrecord of the board they just played.
        /// </summary>
        public bool BM2ViewHandRecord
        {
            get; set;
        }

        /// <summary>
        /// Determines if the players can enter the handrecord for boards they played if not present.
        /// </summary>
        public bool BM2EnterHandRecord
        {
            get; set;
        }

        /// <summary>
        /// If BM2EnterHandRecord is "True" determines if the players can enter the handrecord after play of the board or after playing all boards for the round.
        /// </summary>
        public Bm2EnterHandrecordWhenOption BM2EnterHandRecordWhen
        {
            get; set;
        }

        /// <summary>
        /// Currently not supported.
        /// </summary>
        public bool BM2TextBasedNumber
        {
            get; set;
        }

        /// <summary>
        /// Defines if the "Call TD" button is visible on the Bridgemate.
        /// </summary>
        public bool BM2TDCall
        {
            get; set;
        }

        /// <summary>
        /// Validates the DTO. Produces validation messages if there are problems. 
        /// </summary>
        /// <returns>True if there are no validation errors.</returns>
        public override bool Validate()
        {
            base.Validate();
            var validationMessages=new List<string>(ValidationMessages);
            if (string.IsNullOrWhiteSpace(BM2PINcode) || BM2PINcode.Length != 4 || !int.TryParse(BM2PINcode, out _))
                validationMessages.Add($"Invalid {nameof(BM2PINcode)} ('{BM2PINcode}'). The pincode must be four digits.");

            ValidationMessages=validationMessages.ToArray();
            return !ValidationMessages.Any();
        }
    }
}

