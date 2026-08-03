using System.Collections.ObjectModel;
using BridgeSystems.Bridgemate.DataConnector.ScoringProgramClient;
using BridgeSystems.Bridgemate.DataConnectorClasses.SharedDTO;
using BridgeSystems.Bridgemate.DataConnectorClientEmulator.Movements;
using BridgeSystems.Bridgemate.DataConnectorClientEmulator.Support;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BridgeSystems.Bridgemate.DataConnectorClientEmulator.Viewmodels;

partial class ObservableSession : ObservableObject
{   
    private readonly SessionDTO _session;
    private readonly MainViewmodel _parent;
    public ObservableSession(SessionDTO session, MainViewmodel mainViewmodel)
    {
        _session = session ?? throw new ArgumentNullException(nameof(session));
        _parent = mainViewmodel ?? throw new ArgumentNullException(nameof(mainViewmodel));

        CreatedScoringGroups = new ObservableCollection<ScoringGroupDTO>();

        ScoringGroupNumbers = Enumerable.Range(1, 4).ToList();
        SelectedScoringGroupNumber = ScoringGroupNumbers.First();

        ManuallyCreatedSections = new List<SectionDTO>();
        SectionsCreatedAfterLaunch = new ObservableCollection<SectionDTO>();

        CreatedSectionDescriptions = string.Empty;

        CreatableSections = new List<string> { "A", "B", "C", "D" };
        SelectedCreatableSection = creatableSections.First();

        CreatableNumberOfPairs = Enumerable.Range(4, 30).ToList();
        SelectedCreatableNumberOfPairs = CreatableNumberOfPairs[6];

        NumberOfRounds=Enumerable.Range(1,12).ToList();
        SelectedNumberOfRounds = NumberOfRounds[4];

        NumberOfBoardsPerRound = Enumerable.Range(1, 12).ToList();
        SelectedNumberOfBoardsPerRound = NumberOfBoardsPerRound[1];

        SectionCreationValidationMessages = new ObservableCollection<string>();

        UseEwOffset = true;

        ScoringTypes = ScoringTypeSelection.GetScoringTypesList();
        SelectedScoringType = ScoringTypes.First();

        GameTypes = GameTypeSelection.GetGameTypesList();
        SelectedGameType = GameTypes.First();

        UpdatableSections = new List<string> { "A", "B", "C", "D", "E" };
        SelectedUpdatableSection = UpdatableSections.First();

        AddedSectionDescriptions = string.Empty;
        SessionValidationMessages = new ObservableCollection<string>();
    }

    #region Selectable values for the session properties

    [ObservableProperty]
    private ObservableCollection<ScoringGroupDTO> createdScoringGroups;

    [ObservableProperty]
    private List<int> scoringGroupNumbers;

    [ObservableProperty]
    private int selectedScoringGroupNumber;

    [ObservableProperty]
    private List<ScoringTypeSelection> scoringTypes;

    [ObservableProperty]
    private ScoringTypeSelection selectedScoringType;

    [ObservableProperty]
    private List<SectionDTO> manuallyCreatedSections;

    [ObservableProperty]
    private ObservableCollection<SectionDTO> sectionsCreatedAfterLaunch;

    [ObservableProperty]
    private string createdSectionDescriptions;

    [ObservableProperty]
    private string addedSectionDescriptions;

    [ObservableProperty]
    private List<string> creatableSections;

    [ObservableProperty]
    string selectedCreatableSection;

    [ObservableProperty]
    private List<int> creatableNumberOfPairs;

    [ObservableProperty]
    private int selectedCreatableNumberOfPairs;

    [ObservableProperty]
    private List<int> numberOfBoardsPerRound;

    [ObservableProperty]
    private int selectedNumberOfBoardsPerRound;

    [ObservableProperty]
    private List<int> numberOfRounds;

    [ObservableProperty]
    private int selectedNumberOfRounds;

    [ObservableProperty]
    private List<GameTypeSelection> gameTypes;

    [ObservableProperty]
    private GameTypeSelection selectedGameType;

    [ObservableProperty]
    private ObservableCollection<string> sectionCreationValidationMessages;

    [ObservableProperty]
    private bool hasSectionCreationValidationErrors;

    [ObservableProperty]
    private bool fixedBoards;

    [ObservableProperty]
    private bool useEwOffset;

    [ObservableProperty]
    private int ewOffset;

    partial void OnUseEwOffsetChanged(bool oldValue, bool newValue)
    {
        if (newValue == false)
            EwOffset = 0;
        else
            UseConsecutiveNumbering = false;
    }

    [ObservableProperty]
    private bool useConsecutiveNumbering;

    partial void OnUseConsecutiveNumberingChanged(bool oldValue, bool newValue)
    {
        if (newValue == true)
            UseEwOffset = false;
    }

    [ObservableProperty]
    private List<string> updatableSections;

    [ObservableProperty]
    private string selectedUpdatableSection;

    [ObservableProperty]
    private ObservableCollection<string> sessionValidationMessages;

    [ObservableProperty]
    private bool hasLaunched;

    #endregion

    #region Session properties

    public string EventGuid
    {
        get => _session.EventGuid;
        set
        {
            _session.EventGuid = value;
            OnPropertyChanged(nameof(EventGuid));
        }
    }

    public string SessionGuid
    {
        get => _session.SessionGuid;
        set
        {
            _session.SessionGuid = value;
            OnPropertyChanged(nameof(SessionGuid));
        }
    }

    public string Name
    {
        get => _session.Name;
        set
        {
            _session.Name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    public ScoringGroupDTO[] ScoringGroups
    {
        get => _session.ScoringGroups;
        set
        {
            _session.ScoringGroups = value;
            OnPropertyChanged(nameof(ScoringGroups));
        }
    }

    public SectionDTO? GetSection(string sectionLetters)
    {
        return _session.ScoringGroups.SelectMany(sg => sg.Sections).Where(section => section.Letters == sectionLetters).SingleOrDefault();
    }

    public IEnumerable<SectionDTO> GetSections => ScoringGroups.SelectMany(sg => sg.Sections);

    public bool Validate(bool forAdding) => _session.Validate(forAdding);

    public string[] ValidationMessages => _session.ValidationMessages;

    public SessionDTO Session => _session;

    #endregion

    #region Commands

    public string AddTwoWinnerSectionCommandDescription=>nameof(AddTwoWinnerSectionCommand);

    [RelayCommand]
    private void AddTwoWinnerSection()
    {
        CreateSection(sectionLetters: SelectedCreatableSection,
                      scoringGroupNumber: SelectedScoringGroupNumber,
                      scoringType: SelectedScoringType.ScoringType,
                      gameType: SelectedGameType.GameType,
                      numberOfWinners: 2,
                      numberOfPairs: SelectedCreatableNumberOfPairs,
                      numberOfBoardsPerRound: SelectedNumberOfBoardsPerRound,
                      useFixedBoards: FixedBoards,
                      useConsecutiveNumbering: UseConsecutiveNumbering,
                      ewOffset: EwOffset);
    }

    public  string AddOneWinnerSectionCommandDescription => nameof(AddOneWinnerSectionCommand);

    [RelayCommand]
    private void AddOneWinnerSection()
    {
        CreateSection(sectionLetters: SelectedCreatableSection,
                     scoringGroupNumber: SelectedScoringGroupNumber,
                     scoringType: SelectedScoringType.ScoringType,
                     gameType: SelectedGameType.GameType,
                     numberOfWinners: 1,
                     numberOfPairs: SelectedCreatableNumberOfPairs,
                     numberOfBoardsPerRound: SelectedNumberOfBoardsPerRound,
                     useFixedBoards: FixedBoards,
                     useConsecutiveNumbering: UseConsecutiveNumbering,
                     ewOffset: EwOffset);
    }

    public string ClearSectionsCommandDescription => nameof(ClearSectionsCommand);

    [RelayCommand]
    private void ClearSections()
    {
        foreach (SectionDTO section in ManuallyCreatedSections)
        {
            _parent.SectionProperties.Remove((SessionGuid, section.Letters));
            _parent.ExplicitSectionData.Remove((SessionGuid, section.Letters));
        }

        ManuallyCreatedSections.Clear();
        AddedSectionDescriptions=string.Empty;
        CreatedSectionDescriptions=string.Empty;
        CreatedScoringGroups.Clear();
        ScoringGroups = [];
        OnPropertyChanged();
    }

    /// <summary>
    /// Creates or updates a section. Mind that all sections within a scoring group must have the same scoring method and the
    /// same amount of winners.
    /// </summary>
    /// <param name="numberOfWinners"></param>
    /// <returns></returns>
    internal (bool created, SectionDTO? newSection) CreateSection(string sectionLetters,
                                                                 int scoringGroupNumber,
                                                                 int scoringType,
                                                                 int gameType,
                                                                 int numberOfWinners,
                                                                 int numberOfPairs,
                                                                 int numberOfBoardsPerRound,
                                                                 bool useFixedBoards,
                                                                 bool useConsecutiveNumbering,
                                                                 int ewOffset)
    {

        AddOrUpdateScoringGroup(scoringGroupNumber: scoringGroupNumber, scoringType);

        SectionCreationValidationMessages.Clear();
        HasSectionCreationValidationErrors = false;

        //An individual section sends its seating for every round explicitly (BRID-2037): the pair
        //numbers are the odd player numbers, the owner sits North or East, the partner changes per
        //round. Individuals are always one-winner movements.
        var isIndividual = gameType == SectionDTO.GameType_Individual;
        var numberOfPlayers = numberOfPairs * 2;
        if (isIndividual)
        {
            if (numberOfPlayers < 8 || numberOfPlayers % 4 != 0)
            {
                SectionCreationValidationMessages.Add(
                    $"An individual section needs an even number of pairs (at least four): " +
                    $"{numberOfPairs} pairs = {numberOfPlayers} players, which cannot fill whole tables.");
                HasSectionCreationValidationErrors = true;
                return (false, null);
            }
            numberOfWinners = 1;
        }

        var newSection = new SectionDTO
        {
            SessionGuid = SessionGuid,
            ScoringGroupNumber = scoringGroupNumber,
            Letters = sectionLetters,
            Name = $"Section {sectionLetters}",
            Winners = numberOfWinners,
            GameType = gameType,
            HasExplicitParticipations = isIndividual
        };

        List<Seating> seatings = isIndividual ?
            MovementGenerator.GenerateIndividualSeatings(numberOfPlayers, SelectedNumberOfRounds) :
            MovementGenerator.GenerateMitchellSeatings(numberOfPairs,SelectedNumberOfRounds, useFixedBoards, numberOfWinners, useConsecutiveNumbering,
                                    ewOffset, arrowShiftRound: numberOfWinners == 2 ? 0 : 2);

        var seatingsPerTable = seatings.GroupBy(seating => seating.TableNumber);

        var newTables = new List<TableDTO>();
        foreach (var tableGroup in seatingsPerTable)
        {
            var newTable = new TableDTO
            {
                SessionGuid = SessionGuid,
                SectionLetters = sectionLetters,
                TableNumber = tableGroup.Key
            };

            List<RoundDTO> newRounds = MovementGenerator.CreateRoundDTOs(tableGroup, SessionGuid,
                                                                        sectionLetters, numberOfBoardsPerRound);

            newTable.Rounds = newRounds.ToArray();
            newTables.Add(newTable);
        }

        newSection.Tables = newTables.ToArray();

        if (isIndividual)
        {
            //Store the generated all-round participations and player data with the parent: they are
            //sent with the InitDTO at launch and with every movement update for this section.
            _parent.ExplicitSectionData[(SessionGuid, sectionLetters)] =
                (MovementGenerator.CreateIndividualParticipations(seatings, SessionGuid, sectionLetters, numberOfPlayers),
                 MovementGenerator.CreateIndividualPlayerData(SessionGuid, numberOfPlayers));
        }
        else
        {
            _parent.ExplicitSectionData.Remove((SessionGuid, sectionLetters));
        }

        if (!newSection.Validate())
        {
            foreach (var section in SectionCreationValidationMessages)
            {
                SectionCreationValidationMessages.Add(section);
            }
            HasSectionCreationValidationErrors = true;
            return (false, null);
        }

        var newSectionProperties = (scoringGroupNumber,
                                    scoringType, gameType,
                                    numberOfWinners, numberOfPairs,
                                    numberOfBoardsPerRound,
                                    useFixedBoards, useConsecutiveNumbering,
                                    ewOffset);

        var existingSection = ManuallyCreatedSections.SingleOrDefault(section => section.Letters == sectionLetters);
        if (existingSection != null)
        {
            ManuallyCreatedSections.Remove(existingSection);
            SectionsCreatedAfterLaunch.Remove(existingSection);
            _parent.SectionProperties[(SessionGuid ?? "", sectionLetters)] = newSectionProperties;
        }
        else
        {
            _parent.SectionProperties.Add ((SessionGuid, sectionLetters), newSectionProperties);
        }

        ManuallyCreatedSections.Add(newSection);

        if (HasLaunched)
            SectionsCreatedAfterLaunch.Add(newSection);

        CreatedSectionDescriptions = string.Join(" ,", ManuallyCreatedSections.Select(section => section.Letters));
        AddedSectionDescriptions = string.Join(" ,", SectionsCreatedAfterLaunch.Select(section => section.Letters));

        var scoringGroup = CreatedScoringGroups.Single(sg => sg.ScoringGroupNumber == scoringGroupNumber);
        scoringGroup.Sections = ManuallyCreatedSections.Where(section => section.ScoringGroupNumber == scoringGroupNumber).ToArray();
        _parent.RaiseSectionAdded();
        return (true, newSection);
    }

    public string AddSessionCommandDescription=>nameof(AddSessionCommand);

    /// <summary>
    /// Adds a session to a launched event.
    /// </summary>
    [RelayCommand]
    private void AddSession()
    {
        try
        {
            var dto = new AddSessionDTO
            {
                EventGuid=EventGuid,
                Session = _session
            };
            var response=_parent.Client.AddSession(dto); 
            _parent.AddCommunicationResponse(response);
        }
        catch (Exception ex)
        {
            _parent.CommunicationResults.Add(new CommunicationResult
            {
                RequestDescription = $"{ScoringProgramDataConnectorCommands.AddSession}",
                ErrorType = ErrorType.Exception,
                ResponseMessage = ex.Message
            });
        }
    }

    public string AddNewSectionsCommandDescription => nameof(AddNewSectionsCommand);

    /// <summary>
    /// Adds sections created after the launch of the created event.
    /// </summary>
    [RelayCommand]
    private void AddNewSections()
    {
        if (!SectionsCreatedAfterLaunch.Any()) return;

        foreach (var addedSection in SectionsCreatedAfterLaunch.ToList())
        {
            UpdateSection(addedSection.Letters, _parent.SectionProperties[(SessionGuid, addedSection.Letters)].numberOfPairs);
        }

        SectionsCreatedAfterLaunch.Clear();

    }

    /// <summary>
    /// Updates the section by generating a new movement based on the new number of pairs. The previously selected values for
    /// the movement creation and the selected game type do not change.
    /// </summary>
    /// <param name="newNumberOfPairs"></param>
    public void UpdateSection(string sectionLetters, int newNumberOfPairs)
    {
        try
        {
            var sectionToUpdate = ManuallyCreatedSections.FirstOrDefault(section => section.Letters == SelectedCreatableSection);
            if (sectionToUpdate == null) return;

            var sectionProperties = _parent.SectionProperties[(SessionGuid, sectionLetters)];
            var updatedSection = CreateSection(
                sectionLetters,
                sectionProperties.scoringGroupNumber,
                sectionProperties.scoringType,
                sectionProperties.gameType,
                sectionProperties.winners,
                newNumberOfPairs,
                sectionProperties.numberOfBoardsPerRound,
                sectionProperties.useFixedBoards,
                sectionProperties.useConsecutiveNumbering,
                sectionProperties.ewOffset).newSection;

            if (updatedSection == null) return;

            var updateDTO = new SectionUpdateDTO
            {
                SessionGuid = updatedSection.SessionGuid,
                Letters = updatedSection.Letters,
                ScoringGroupNumber = updatedSection.ScoringGroupNumber,
                ScoringGroupScoringMethod = sectionProperties.scoringType,
                GameType = updatedSection.GameType,
                Winners = updatedSection.Winners,
                Tables = updatedSection.Tables,
                HasExplicitParticipations = updatedSection.HasExplicitParticipations
            };
            if (updatedSection.HasExplicitParticipations &&
                _parent.ExplicitSectionData.TryGetValue((SessionGuid, sectionLetters), out var explicitData))
            {
                //The update carries the complete new seating for all rounds; BCS replaces the
                //stored participations with this set (BRID-2037).
                updateDTO.Participations = explicitData.participations.ToArray();
                updateDTO.AddedPlayers = explicitData.players.ToArray();
            }

            var response = _parent.Client.UpdateMovement(updateDTO);

            _parent.AddCommunicationResponse(response);

            AddedSectionDescriptions=string.Empty;


        }
        catch (Exception ex)
        {
            _parent.CommunicationResults.Add(new CommunicationResult
            {
                ErrorType = ErrorType.Exception,
                RequestDescription = nameof(UpdateSection),
                ResponseMessage = ex.Message
            });
        }

    }

    #endregion

    #region Helper functions

    private void AddOrUpdateScoringGroup(int scoringGroupNumber, int scoringType)
    {
        var updatableScoringGroup = new ScoringGroupDTO();
        var existingScoringGroup = CreatedScoringGroups.SingleOrDefault(sg => sg.ScoringGroupNumber == scoringGroupNumber);
        if (existingScoringGroup != null)
        {
            updatableScoringGroup.SessionGuid = existingScoringGroup.SessionGuid;
            CreatedScoringGroups.Remove(existingScoringGroup);
        }
        else
        {
            updatableScoringGroup.SessionGuid = SessionGuid;
        }
        updatableScoringGroup.ScoringGroupNumber = scoringGroupNumber;
        updatableScoringGroup.ScoringMethod = scoringType;
        CreatedScoringGroups.Add(updatableScoringGroup);
        ScoringGroups = CreatedScoringGroups.ToArray();
    }

    #endregion

}
