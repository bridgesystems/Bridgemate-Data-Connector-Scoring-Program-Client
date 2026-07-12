using System.Collections.ObjectModel;
using BridgeSystems.Bridgemate.DataConnector.ScoringProgramClient;
using BridgeSystems.Bridgemate.DataConnectorClasses.SharedDTO;
using BridgeSystems.Bridgemate.DataConnectorClientEmulator.Movements;
using BridgeSystems.Bridgemate.DataConnectorClientEmulator.Support;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BridgeSystems.Bridgemate.DataConnectorClientEmulator.Viewmodels;

/// <summary>
/// Drives a single-section, single-scoring-group session whose rounds are pushed to the Data
/// Connector one at a time (with KeepBridgematesAlive), the way a real Swiss-match scoring
/// program would, instead of sending the whole movement up front like "Manual event creation" does.
/// The movement itself is a plain barometer Mitchell (every table plays the same board group per
/// round); once its natural round limit is reached, pairings wrap around and repeat.
/// </summary>
partial class SwissPairsViewmodel : ObservableObject
{
    private readonly MainViewmodel _parent;
    private const string SectionLetters = "A";
    private const int ScoringGroupNumber = 1;

    private int _numberOfPairs;
    private string? _sessionGuid;

    public SwissPairsViewmodel(MainViewmodel parent)
    {
        _parent = parent ?? throw new ArgumentNullException(nameof(parent));

        SessionName = "Swiss Pairs Test";
        NumberOfTables = 6;
        NumberOfWinnersOptions = new List<int> { 1, 2 };
        SelectedNumberOfWinners = NumberOfWinnersOptions.First();
        NumberOfBoardsPerRound = 1;
        ValidationMessages = new ObservableCollection<string>();
    }

    #region Groupbox 1: create session

    [ObservableProperty]
    private string sessionName;

    [ObservableProperty]
    private int numberOfTables;

    [ObservableProperty]
    private List<int> numberOfWinnersOptions;

    [ObservableProperty]
    private int selectedNumberOfWinners;

    [ObservableProperty]
    private int numberOfBoardsPerRound;

    [ObservableProperty]
    private bool hasLaunched;

    [ObservableProperty]
    private ObservableCollection<string> validationMessages;

    [ObservableProperty]
    private bool hasValidationMessages;

    [ObservableProperty]
    private int currentRoundNumber;

    [ObservableProperty]
    private int maxRounds;

    public string LaunchCommandDescription => nameof(LaunchCommand);

    [RelayCommand]
    private void Launch()
    {
        try
        {
            ValidationMessages.Clear();
            HasValidationMessages = false;

            _numberOfPairs = NumberOfTables * 2;
            MaxRounds = NumberOfTables % 2 == 0 ? NumberOfTables - 1 : NumberOfTables;

            List<Seating> seatings = GenerateSeatingsUpToRound(1);

            _sessionGuid = Guid.NewGuid().ToString("N").ToUpper();

            var newSection = new SectionDTO
            {
                SessionGuid = _sessionGuid,
                ScoringGroupNumber = ScoringGroupNumber,
                Letters = SectionLetters,
                Name = $"Section {SectionLetters}",
                Winners = SelectedNumberOfWinners,
                GameType = SectionDTO.GameType_Pairs,
                KeepBridgematesAlive = true,
                Tables = BuildTables(seatings)
            };

            if (!newSection.Validate())
            {
                foreach (var message in newSection.ValidationMessages)
                    ValidationMessages.Add(message);
                HasValidationMessages = true;
                return;
            }

            var now = DateTime.Now;
            var newSession = new SessionDTO
            {
                EventGuid = _sessionGuid,
                SessionGuid = _sessionGuid,
                Name = SessionName,
                Year = now.Year,
                Month = now.Month,
                Day = now.Day,
                Hour = now.Hour,
                Minute = now.Minute,
                ScoringGroups = new[]
                {
                    new ScoringGroupDTO
                    {
                        SessionGuid = _sessionGuid,
                        ScoringGroupNumber = ScoringGroupNumber,
                        ScoringMethod = ScoringGroupDTO.ScoringType_Pairs,
                        Sections = new[] { newSection }
                    }
                }
            };

            ObservableEvent.Guid = _sessionGuid;

            var initDto = new InitDTO
            {
                Commands = InitDTO.StartBCS + InitDTO.Command_Reset + InitDTO.Command_StartReading,
                EventGuid = _sessionGuid,
                Sessions = new[] { newSession }
            };

            if (!initDto.Validate())
            {
                foreach (var message in initDto.ValidationMessages)
                    ValidationMessages.Add(message);
                HasValidationMessages = true;
                return;
            }

            var response = _parent.Client.Initialize(initDto);
            _parent.AddCommunicationResponse(response);

            if (response.ErrorType == ErrorType.None)
            {
                _parent.CurrentEvent = new ObservableEvent(new ObservableSession(newSession, _parent));
                _parent.EventHasLaunched = true;
                HasLaunched = true;
                CurrentRoundNumber = 1;
            }
        }
        catch (Exception ex)
        {
            _parent.CommunicationResults.Add(new CommunicationResult
            {
                ErrorType = ErrorType.Exception,
                RequestDescription = nameof(Launch),
                ResponseMessage = ex.Message
            });
        }
    }

    #endregion

    #region Groupbox 2: push rounds

    public string PushNextRoundCommandDescription => nameof(PushNextRoundCommand);

    [RelayCommand]
    private void PushNextRound() => PushRound(keepBridgematesAlive: true);

    public string PushLastRoundCommandDescription => nameof(PushLastRoundCommand);

    [RelayCommand]
    private void PushLastRound() => PushRound(keepBridgematesAlive: false);

    private void PushRound(bool keepBridgematesAlive)
    {
        try
        {
            var targetRound = CurrentRoundNumber + 1;

            //A SectionUpdateDTO must carry the complete, consecutive movement known so far for every
            //table (round 1..targetRound) - not just the newest round - or it fails client-side validation.
            List<Seating> seatings = GenerateSeatingsUpToRound(targetRound);

            var updateDto = new SectionUpdateDTO
            {
                SessionGuid = _sessionGuid,
                Letters = SectionLetters,
                ScoringGroupNumber = ScoringGroupNumber,
                ScoringGroupScoringMethod = ScoringGroupDTO.ScoringType_Pairs,
                GameType = SectionDTO.GameType_Pairs,
                Winners = SelectedNumberOfWinners,
                KeepBridgematesAlive = keepBridgematesAlive,
                Tables = BuildTables(seatings)
            };

            var response = _parent.Client.UpdateMovement(updateDto);
            _parent.AddCommunicationResponse(response);

            if (response.ErrorType == ErrorType.None)
                CurrentRoundNumber = targetRound;
        }
        catch (Exception ex)
        {
            _parent.CommunicationResults.Add(new CommunicationResult
            {
                ErrorType = ErrorType.Exception,
                RequestDescription = nameof(PushRound),
                ResponseMessage = ex.Message
            });
        }
    }

    #endregion

    #region Helper functions

    /// <summary>
    /// Generates the seatings for every round from 1 up to (and including) <paramref name="targetRound"/>.
    /// The Mitchell movement can only produce MaxRounds distinct pairings; beyond that, rounds wrap
    /// around and repeat an earlier round's pairings while still advancing the round/board number.
    /// </summary>
    private List<Seating> GenerateSeatingsUpToRound(int targetRound)
    {
        var arrowShiftRound = SelectedNumberOfWinners == 2 ? 0 : 2;
        var allSeatings = new List<Seating>();

        for (var round = 1; round <= targetRound; round++)
        {
            var effectiveRound = ((round - 1) % MaxRounds) + 1;

            List<Seating> roundSeatings = MovementGenerator.GenerateMitchellSeatings(
                _numberOfPairs, numberOfRounds: effectiveRound, fixedBoards: false, SelectedNumberOfWinners,
                consecutiveNumbering: false, ewOffset: 0, arrowShiftRound: arrowShiftRound)
                .Where(seating => seating.RoundNumber == effectiveRound)
                .ToList();

            foreach (var seating in roundSeatings)
            {
                seating.RoundNumber = round;
                seating.BoardSet = round; //barometer: every table plays the same boards each round.
            }

            allSeatings.AddRange(roundSeatings);
        }

        return allSeatings;
    }

    private TableDTO[] BuildTables(IEnumerable<Seating> seatings)
    {
        return seatings.GroupBy(seating => seating.TableNumber)
                        .Select(tableGroup => new TableDTO
                        {
                            SessionGuid = _sessionGuid,
                            SectionLetters = SectionLetters,
                            TableNumber = tableGroup.Key,
                            Rounds = MovementGenerator.CreateRoundDTOs(tableGroup.OrderBy(s => s.RoundNumber),
                                                                       _sessionGuid, SectionLetters,
                                                                       NumberOfBoardsPerRound).ToArray()
                        })
                        .ToArray();
    }

    #endregion
}
