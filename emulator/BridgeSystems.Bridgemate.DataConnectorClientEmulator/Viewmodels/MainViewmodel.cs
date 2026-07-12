using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using BridgeSystems.Bridgemate.DataConnector.ScoringProgramClient;
using BridgeSystems.Bridgemate.DataConnectorClasses.SharedDTO;
using BridgeSystems.Bridgemate.DataConnectorClientEmulator.Support;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using NLog;
using static BridgeSystems.Bridgemate.DataConnectorClientEmulator.Support.ExternalPlayerDataTextFileCommunicator;

namespace BridgeSystems.Bridgemate.DataConnectorClientEmulator.Viewmodels;
partial class MainViewmodel : ObservableObject
{
    //Please mind that all commands use the synchronous implmentation. Each command has an asynchronous twin, ending in 'Async.'
    //Make sure however to never to call a new command before the previous one has completed.

    #region fields

    private readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private readonly Dictionary<(string, string), (int scoringGroupNumber,
                                         int scoringType, int gameType,
                                         int winners, int numberOfPairs,
                                         int numberOfBoardsPerRound,
                                         bool useFixedBoards, bool useConsecutiveNumbering,
                                         int ewOffset)>
        _sectionProperties;

    #endregion

    #region Constructor

    public MainViewmodel()
    {
        _sectionProperties = new Dictionary<(string, string), (int scoringGroupNumber,
                                                     int scoringType, int gameType,
                                                     int winners, int numberOfPairs,
                                                     int numberOfBoardsPerRound,
                                                     bool useFixedBoards, bool usConsecutiveNumbering,
                                                     int ewOffset)>();
        CommunicationResults = new ObservableCollection<CommunicationResult>();

        validationMessage = string.Empty;

        Directions = DirectionSelection.GetDirectionsList();
        SelectedDirection = Directions.First();

        //* result creation

        results = new List<ResultDTO>();

        ScoringSides = ScoringSideSelection.GetScoringSidesList();
        SelectedScoringSide = ScoringSides.First();

        ContractSections = new List<string> { "A", "B", "C", "D" };
        selectedContractSection = ContractSections.First();

        ContractTables = Enumerable.Range(1, 20).ToList();
        selectedContractTable = ContractTables.First();

        ContractRounds = Enumerable.Range(1, 12).ToList();
        SelectedContractRound = ContractRounds.First();

        ContractBoards = Enumerable.Range(1, 30).ToList();
        SelectedContractBoard = ContractBoards.First();

        NsPairNumbers = Enumerable.Range(0, 41).ToList();
        SelectedNsPairNumber = NsPairNumbers.First();

        EwPairNumbers = Enumerable.Range(0, 121).ToList();
        SelectedEwPairNumber = EwPairNumbers.First();

        DeclaringPairNumbers = Enumerable.Range(0, 121).ToList();
        SelectedDeclaringPairNumber = DeclaringPairNumbers.First();

        ContractDirectionSelections = DirectionSelection.GetDirectionsList();
        SelectedContractDirectionSelection = contractDirectionSelections.First();

        ContractLevels = Enumerable.Range(1, 7).ToList();
        SelectedContractLevel = ContractLevels.First();

        Denominations = DenominationSelection.GetDenominationSelectionsList();
        SelectedDenomination = Denominations.First();

        Stakes = StakeSelection.GetStakeSelectionsList();
        SelectedStake = Stakes.First();

        TotalTricksMade = Enumerable.Range(0, 14).ToList();
        SelectedTotalTricksMade = TotalTricksMade.First();

        SpecialResultValues = SpecialResultSelection.GetSpecialResultSelectionsList();
        SelectedSpecialResultValue = SpecialResultValues.First();

        //* end result creation

        Sections = new List<string> { "A", "B", "C", "D" };
        SelectedSection = Sections.First();

        TableNumbers = Enumerable.Range(1, 10).ToList();
        SelectedTableNumber = TableNumbers.First();

        ValidationMessages = new ObservableCollection<string>();

        UpdatableSections = new List<string> { "A", "B", "C", "D", "E" };
        SelectedUpdatableSection = UpdatableSections.First();

        ObservableEvent.Guid = CreateGuid();
        CurrentEvent = new ObservableEvent(new ObservableSession(CreateNewSession(), this));

        SwissPairs = new SwissPairsViewmodel(this);
    }

    #endregion
    private IScoringProgramClient? _client;

    internal Dictionary<(string, string), (int scoringGroupNumber,
                                 int scoringType, int gameType,
                                 int winners, int numberOfPairs,
                                 int numberOfBoardsPerRound,
                                 bool useFixedBoards, bool useConsecutiveNumbering,
                                 int ewOffset)> SectionProperties => _sectionProperties;


    internal IScoringProgramClient Client => _client;

    public SwissPairsViewmodel SwissPairs { get; }

    ObservableSession CurrentSession => CurrentEvent.CurrentSession;
    string CurrentSessionGuid => CurrentSession.SessionGuid;
    string CurrentSessionName => CurrentSession.Name;

    #region Bound properties

    [ObservableProperty]
    private bool isConnected;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private bool eventHasLaunched;

    [ObservableProperty]
    ObservableEvent currentEvent;

    [ObservableProperty]
    private ObservableCollection<CommunicationResult> communicationResults;

    [ObservableProperty]
    private string? newPlayerFirstName;

    [ObservableProperty]
    private string? newPlayerLastName;

    [ObservableProperty]
    private string? newPlayerNumber;

    [ObservableProperty]
    private List<DirectionSelection> directions;

    [ObservableProperty]
    private DirectionSelection? selectedDirection;

    [ObservableProperty]
    private List<string> sections;

    [ObservableProperty]
    private string? selectedSection;

    [ObservableProperty]
    private List<int> tableNumbers;

    [ObservableProperty]
    private int selectedTableNumber;

    #region result construction

    private readonly List<ResultDTO> results;

    [ObservableProperty]
    private int numberOfResults;

    [ObservableProperty]
    private List<ScoringSideSelection> scoringSides;

    [ObservableProperty]
    private ScoringSideSelection selectedScoringSide;

    [ObservableProperty]
    private List<string> contractSections;

    [ObservableProperty]
    private string selectedContractSection;

    [ObservableProperty]
    private List<int> contractTables;

    [ObservableProperty]
    private int selectedContractTable;

    [ObservableProperty]
    private List<int> contractRounds;

    [ObservableProperty]
    private int selectedContractRound;

    [ObservableProperty]
    private List<int> contractBoards;

    [ObservableProperty]
    private int selectedContractBoard;

    [ObservableProperty]
    private List<int> nsPairNumbers;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSwitched))]
    private int selectedNsPairNumber;

    [ObservableProperty]
    private List<int> ewPairNumbers;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSwitched))]
    private int selectedEwPairNumber;

    [ObservableProperty]
    private List<int> declaringPairNumbers;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSwitched))]
    private int selectedDeclaringPairNumber;

    [ObservableProperty]
    private List<DirectionSelection> contractDirectionSelections;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSwitched))]
    private DirectionSelection selectedContractDirectionSelection;

    [ObservableProperty]
    private List<int> contractLevels;

    [ObservableProperty]
    private int selectedContractLevel;

    [ObservableProperty]
    private List<DenominationSelection> denominations;

    [ObservableProperty]
    private DenominationSelection selectedDenomination;

    [ObservableProperty]
    private List<StakeSelection> stakes;

    [ObservableProperty]
    private StakeSelection selectedStake;

    [ObservableProperty]
    private List<int> totalTricksMade;

    [ObservableProperty]
    private int selectedTotalTricksMade;

    public bool IsSwitched => DetemineIsSwitched();

    [ObservableProperty]
    private string validationMessage;

    [ObservableProperty]
    private bool enterSpecialResult;

    [ObservableProperty]
    private List<SpecialResultSelection> specialResultValues;

    [ObservableProperty]
    private SpecialResultSelection selectedSpecialResultValue;

    [ObservableProperty]
    private bool addNewPlayer;

    #endregion

    #endregion

    #region Commands

    #region Connect tab

    /// <summary>
    /// When false (the default) the emulator communicates over named pipes. When true it communicates over http,
    /// which also works from another computer on the local network: set <see cref="HttpBaseAddress"/> to the
    /// address of the computer the data connector runs on.
    /// </summary>
    [ObservableProperty]
    private bool useHttp;

    /// <summary>
    /// The base address of the data connector host, e.g. "http://192.168.1.50:5079".
    /// When left empty the client targets the data connector on this computer, discovering its published port
    /// from the registry (default 5079).
    /// </summary>
    [ObservableProperty]
    private string? httpBaseAddress;

    [ObservableProperty]
    private string? httpClubId;

    [ObservableProperty]
    private string? httpLicenceKey;

    [ObservableProperty]
    private string? configStatusMessage;

    [ObservableProperty]
    private BCSManagementResponseDTO? statusInfo;

    [ObservableProperty]
    private BCSManagementResponseDTO? allSessionsInfo;

    public string ConnectCommandDescription => nameof(ConnectCommand);

    /// <summary>
    /// Connects to the data connector/
    /// </summary>
    [RelayCommand]
    private void Connect()
    {
        try
        {
            ConfigStatusMessage = string.Empty;
            this.CreateClient();
            var response = _client.Connect();
            IsConnected = response.RequestCommand == ScoringProgramDataConnectorCommands.Connect && response.DataType == DataConnectorResponseData.OK;
            ConfigStatusMessage = $"Connection {(IsConnected ? "succeeded" : "failed")}.";
        }
        catch (Exception ex)
        {
            ConfigStatusMessage = ex.Message;
            Logger.Error(ex);
            IsConnected = false;
        }
    }

    public string DisconnectCommandDescription => nameof(DisconnectCommand);

    /// <summary>
    /// Disconnects from the data connector.
    /// </summary>
    [RelayCommand]
    private void Disconnect()
    {
        try
        {
            ConfigStatusMessage = string.Empty;
            ScoringProgramResponse response = _client.Disconnect();
            if (response.RequestCommand == ScoringProgramDataConnectorCommands.Disconnect && response.DataType == DataConnectorResponseData.OK)
            {
                ConfigStatusMessage = "Disconnected";
                IsConnected = false;
            }
        }
        catch (Exception ex)
        {
            ConfigStatusMessage = ex.Message;
            Logger.Error(ex);
        }
    }

    public string PingCommandDescription => nameof(PingCommand);

    /// <summary>
    /// Sends a Ping (heartbeat) command to the data connector to check if it is responsive.
    /// </summary>
    [RelayCommand]
    private void Ping()
    {
        try
        {
            ConfigStatusMessage = string.Empty;
            IsBusy = true;
            ScoringProgramResponse response = _client.Ping();
            IsConnected = response.RequestCommand == ScoringProgramDataConnectorCommands.Ping && response.DataType == DataConnectorResponseData.OK;
            ConfigStatusMessage = response.ErrorType == ErrorType.None ? "Ping succeeded." : "Ping failed.";
        }
        catch (Exception ex)
        {
            ConfigStatusMessage = ex.Message;
            Logger.Error(ex);
            IsConnected = false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    public string ShutdownBCSCommandDescription => nameof(ShutdownBCSCommand);

    /// <summary>
    /// Sends a command to shut down BCS (BCS will respond after it has completed its current cycle of processing data).
    /// </summary>
    [RelayCommand]
    private void ShutdownBCS()
    {
        var dto = new BCSManagementRequestDTO
        {
            Command = BCSManagementRequestDTO.ShutDownNow
        };
        ScoringProgramResponse result = _client.IssueManagementCommand(dto);
        ConfigStatusMessage = result.SerializedData;
    }

    public string ClearDataCommandDescription => nameof(ClearDataCommand);

    /// <summary>
    /// Clears all incoming and outgoing data from the data connector.
    /// </summary>
    [RelayCommand]
    private void ClearData()
    {
        var result = _client.ClearData();
        ConfigStatusMessage = $"Clear data {(result ? "succeeded" : "failed")}.";
    }

    public string GetStatusInfoCommandDescription => nameof(GetStatusInfoCommand);

    /// <summary>
    /// Returns information on BCS: if it is running and which event it is running.
    /// </summary>
    [RelayCommand]
    private void GetStatusInfo()
    {
        try
        {
            ConfigStatusMessage = string.Empty;
            var dto = new BCSManagementRequestDTO
            {
                Command = BCSManagementRequestDTO.GetRunningSessions +
                          BCSManagementRequestDTO.GetDataConnectorFileLocation +
                          BCSManagementRequestDTO.GetScoringFileLocation
            };

            ScoringProgramResponse result = _client.IssueManagementCommand(dto);
            if (result.DataType == DataConnectorResponseData.EventInfo)
            {
                BCSManagementResponseDTO info = JsonSerializer.Deserialize<BCSManagementResponseDTO>(result.SerializedData);
                StatusInfo = info;
            }
            else
            {
                ConfigStatusMessage = $"{result.DataType} {result.ErrorType} {result.SerializedData}";
            }
        }
        catch (Exception ex)
        {
            ConfigStatusMessage = ex.Message;
        }
    }

    public string GetAllSessionsInfoCommandDescription => nameof(GetAllSessionsInfoCommand);

    /// <summary>
    /// Returns a list of all sessions known to BCS.
    /// </summary>
    [RelayCommand]
    private void GetAllSessionsInfo()
    {
        ConfigStatusMessage = string.Empty;
        //await GetMovements();
        var dto = new BCSManagementRequestDTO
        {
            Command = BCSManagementRequestDTO.GetAllSessionsInformation
        };
        ScoringProgramResponse result = _client.IssueManagementCommand(dto);
        if (result.DataType == DataConnectorResponseData.AllSessionsInfo)
        {
            BCSManagementResponseDTO info = JsonSerializer.Deserialize<BCSManagementResponseDTO>(result.SerializedData);
            AllSessionsInfo = info;
        }
        else
        {
            ConfigStatusMessage = $"{result.DataType} {result.ErrorType} {result.SerializedData}";
        }
    }

    public string LaunchVisibleDataConnectorCommandDescription=> nameof(LaunchVisibleDataConnectorCommand);

    [RelayCommand]
    private void LaunchVisibleDataConnector()
    {
        try
        {
            RegistryKey? key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Bridge Systems BV\BCS.Net\InfoForExternalProgram");
            var dataConnectorRootDirectory = Path.GetDirectoryName(key?.GetValue("ExePath")?.ToString());

            if (dataConnectorRootDirectory == null || !Directory.Exists(dataConnectorRootDirectory))
            {
                ConfigStatusMessage = "The data connector executable could not be found.";
                return;
            }

            var dataconnectorPath = Path.Combine(dataConnectorRootDirectory, "BDC", "BridgeSystems.Bridgemate.DataConnectorService.exe");
            var process = new Process();
            var startInfo = new ProcessStartInfo
            {
                Arguments = "-c",
                FileName = dataconnectorPath
            };
            process.StartInfo = startInfo;
            var success = process.Start();
            ConfigStatusMessage = $"Restart of the dataconnector {(success ? "succeeded" : "failed")}.";

        }
        catch (Exception ex)
        {
            ConfigStatusMessage = ex.Message;
        }
    }

    #endregion

    #region Basic event procedures

    public string LaunchCommandDescription => nameof(LaunchCommand);

    /// <summary>
    /// Launches a prefonfigured event by loading a initDto as json.
    /// </summary>
    [RelayCommand]
    private void Launch()
    {
        try
        {
            CurrentEvent.EventGuid = string.Empty;
            EventHasLaunched = false;
            var json = File.ReadAllText(@"DTOs\MinimalInitDto.json");
            var dto = JsonSerializer.Deserialize<InitDTO>(json);
            CurrentEvent.EventGuid = dto.EventGuid;
            CurrentEvent.Sessions.Clear();
            CurrentEvent.Sessions.Add(new ObservableSession(dto.Sessions.Single(), this));
            CurrentEvent.CurrentSession = CurrentEvent.Sessions.Single();
            ScoringProgramResponse response = _client.Initialize(dto);
            AddCommunicationResponse(response);
            if (response.DataType == DataConnectorResponseData.OK)
                EventHasLaunched = true;
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            CommunicationResults.Add(new CommunicationResult
            {
                RequestDescription = $"{ScoringProgramDataConnectorCommands.InitializeEvent}",
                ResponseMessage = $"Request failed:{Environment.NewLine}{ex.Message}"
            });
        }
    }

    public string ContinueCommandDescription => nameof(ContinueCommand);

    /// <summary>
    /// Restarts BCS, instructing it to continue processing data for the event. The current data will not be changed.
    /// </summary>
    [RelayCommand]
    private void Continue()
    {
        var dto = new ContinueDTO
        {
            Commands = InitDTO.StartBCS + InitDTO.Command_StartReading,
            EventGuid = CurrentEvent.EventGuid
        };
        ScoringProgramResponse result = _client.Continue(dto);
        AddCommunicationResponse(result);
        if (result.DataType == DataConnectorResponseData.OK)
        {
            EventHasLaunched = true;
        }
    }


    public string SendResultCommandDescription => nameof(SendResultCommand);

    /// <summary>
    /// Sends a preconstructed result to the data connector.
    /// </summary>
    [RelayCommand]
    private void SendResult()
    {
        try
        {
            var json = File.ReadAllText(@"DTOs\ResultDto.json");
            var dtos = JsonSerializer.Deserialize<ResultDTO[]>(json);
            ScoringProgramResponse response = _client.SendResults(CurrentSessionGuid, dtos);
            AddCommunicationResponse(response);
        }
        catch (Exception ex)
        {
            CommunicationResults.Add(new CommunicationResult
            {
                RequestDescription = $"{ScoringProgramDataConnectorCommands.PutResults}",
                ResponseMessage = ex.Message
            });
            Logger.Error(ex);
        }
    }

    public string UpdateNameCommandDescription => nameof(UpdateNameCommand);

    /// <summary>
    /// Sends full player data or a participation specifying the seating of a player in a round.
    /// </summary>
    [RelayCommand]
    private void UpdateName()
    {
        ScoringProgramDataConnectorCommands currentCommand = ScoringProgramDataConnectorCommands.None;
        try
        {
            currentCommand = ScoringProgramDataConnectorCommands.PutPlayerData;
            var playerData = new PlayerDataDTO
            {
                SessionGuid = CurrentSessionGuid,
                PlayerNumber = NewPlayerNumber,
                FirstName = NewPlayerFirstName,
                LastName = NewPlayerLastName
            };
            ScoringProgramResponse result = _client.SendPlayerData(CurrentSessionGuid, new[] { playerData });
            AddCommunicationResponse(result);

            currentCommand = ScoringProgramDataConnectorCommands.PutParticipations;
            var participation = new ParticipationDTO
            {
                SessionGuid = CurrentSessionGuid,
                PlayerNumber = NewPlayerNumber,
                SectionLetters = "A",
                TableNumber = SelectedTableNumber,
                RoundNumber = 1,
                Direction = SelectedDirection?.Direction ?? TableDirection.None
            };

            result = _client.SendParticipations(CurrentSessionGuid, new[] { participation });
            AddCommunicationResponse(result);
        }
        catch (Exception ex)
        {
            CommunicationResults.Add(new CommunicationResult
            {
                RequestDescription = $"{currentCommand}",
                ResponseMessage = ex.Message
            });
            Logger.Error(ex);
        }
    }

    #endregion

    #region Polling

    /// <summary>
    /// If true polling will ignore previously accepted data and return all data of the requested type.
    /// </summary>
    [ObservableProperty]
    private bool pollAll;

    public string PollPlayerDataCommandDescription => nameof(PollPlayerDataCommand);

    /// <summary>
    /// Polls for new player data since the last accept.
    /// </summary>
    [RelayCommand]
    private void PollPlayerData()
    {
        PlayerDataDTO[] data = _client.PollForPlayerData(CurrentSessionGuid, PollAll);
        AddPollResponse(data);
    }

    public string PollParticipationsCommandDescription => nameof(PollParticipationsCommand);

    /// <summary>
    /// Polls for new participations since the last accept.
    /// </summary>
    [RelayCommand]
    private void PollParticipations()
    {
        ParticipationDTO[] data = _client.PollForParticipations(CurrentSessionGuid, PollAll);
        AddPollResponse(data);
    }

    public string PollResultsCommandDescription => nameof(PollResultsCommand);

    /// <summary>
    /// Polls for new results since the last accept.
    /// </summary>
    [RelayCommand]
    private void PollResults()
    {
        ResultDTO[] data = _client.PollForResults(CurrentSessionGuid, PollAll);
        AddPollResponse(data);
    }

    public string PollHandrecordsCommandDescription => nameof(PollHandrecordsCommand);

    /// <summary>
    /// Polls for new handrecords since the last accept.
    /// </summary>
    [RelayCommand]
    private void PollHandrecords()
    {
        HandrecordDTO[] data = _client.PollForHandrecords(CurrentSessionGuid, PollAll);
        AddPollResponse(data);
    }

    public string AcceptPlayerDataCommandDescription => nameof(AcceptPlayerDataCommand);

    /// <summary>
    /// Accepts the last set of retrieved player data.
    /// </summary>
    [RelayCommand]
    private void AcceptPlayerData()
    {
        try
        {
            ScoringProgramResponse result = _client.AcceptQueueData(CurrentSessionGuid, DataConnectorResponseData.PlayerData);
            AddCommunicationResponse(result);
        }
        catch (Exception ex)
        {
            CommunicationResults.Add(new CommunicationResult
            {
                RequestDescription = $"{ScoringProgramDataConnectorCommands.AcceptPlayerDataQueueItems}",
                ResponseMessage = ex.Message
            });
            Logger.Error(ex);
        }
    }

    public string AcceptParticipationsCommandDescription => nameof(AcceptParticipationsCommand);    

    /// <summary>
    /// Accepts the last set of retrieved pArticipations.
    /// </summary>
    [RelayCommand]
    private void AcceptParticipations()
    {
        try
        {
            ScoringProgramResponse result = _client.AcceptQueueData(CurrentSessionGuid, DataConnectorResponseData.Participations);
            AddCommunicationResponse(result);
        }
        catch (Exception ex)
        {
            CommunicationResults.Add(new CommunicationResult
            {
                RequestDescription = $"{ScoringProgramDataConnectorCommands.AcceptParticipantQueueItems}",
                ResponseMessage = ex.Message
            });
            Logger.Error(ex);
        }
    }

    public string AcceptResultsCommandDescription => nameof(AcceptResultsCommand);

    /// <summary>
    /// Accepts the last set of retrieved results.
    /// </summary>
    [RelayCommand]
    private void AcceptResults()
    {
        try
        {
            ScoringProgramResponse result = _client.AcceptQueueData(CurrentSessionGuid, DataConnectorResponseData.Results);
            AddCommunicationResponse(result);
        }
        catch (Exception ex)
        {
            CommunicationResults.Add(new CommunicationResult
            {
                RequestDescription = $"{ScoringProgramDataConnectorCommands.AcceptResultQueueItems}",
                ResponseMessage = ex.Message
            });
            Logger.Error(ex);
        }
    }

    public string AcceptHandrecordsCommandDescription => nameof(AcceptHandrecordsCommand);

    /// <summary>
    /// Accepts the last set of retrieved handrecords.
    /// </summary>
    [RelayCommand]
    private void AcceptHandrecords()
    {
        try
        {
            ScoringProgramResponse result = _client.AcceptQueueData(CurrentSessionGuid, DataConnectorResponseData.Handrecords);
            AddCommunicationResponse(result);
        }
        catch (Exception ex)
        {
            CommunicationResults.Add(new CommunicationResult
            {
                RequestDescription = $"{ScoringProgramDataConnectorCommands.AcceptHandrecordQueueItems}",
                ResponseMessage = ex.Message
            });
            Logger.Error(ex);
        }
    }

    public string PollTdCallsCommandDescription => nameof(PollTdCallsCommand);

    /// <summary>
    /// Polls asynchronously for new td call status updates since the last accept.
    /// </summary>
    [RelayCommand]
    private async Task PollTdCalls()
    {
        try
        {
            TdCallDTO[] data = await _client.PollForTdCallsAsync(CurrentSessionGuid, PollAll);
            AddPollResponse(data);
        }
        catch (Exception ex)
        {
            CommunicationResults.Add(new CommunicationResult
            {
                ErrorType = ErrorType.Exception,
                RequestDescription = PollAll ? ScoringProgramDataConnectorCommands.PollQueueForAllTdCalls.ToString()
                                              : ScoringProgramDataConnectorCommands.PollQueueForNewTdCalls.ToString(),
                ResponseMessage = ex.Message
            });
            Logger.Error(ex);
        }
    }

    public string AcceptTdCallsCommandDescription => nameof(AcceptTdCallsCommand);

    /// <summary>
    /// Accepts the last set of retrieved td call status updates.
    /// </summary>
    [RelayCommand]
    private async Task AcceptTdCalls()
    {
        try
        {
            ScoringProgramResponse result = await _client.AcceptQueueDataAsync(CurrentSessionGuid, DataConnectorResponseData.TdCalls);
            AddCommunicationResponse(result);
        }
        catch (Exception ex)
        {
            CommunicationResults.Add(new CommunicationResult
            {
                ErrorType = ErrorType.Exception,
                RequestDescription = $"{ScoringProgramDataConnectorCommands.AcceptTdCallQueueItems}",
                ResponseMessage = ex.Message
            });
            Logger.Error(ex);
        }
    }

    #endregion

    #region Advanced operations

    public string LaunchAdvancedCommandDescription => nameof(LaunchAdvancedCommand);

    /// <summary>
    /// Launches an event with two sections.
    /// </summary>
    [RelayCommand]
    private void LaunchAdvanced()
    {
        try
        {
            CurrentEvent.EventGuid = string.Empty;
            CurrentEvent.Sessions.Clear();
            EventHasLaunched = false;
            var json = File.ReadAllText(@"DTOs\FullInitDto.json");
            var dto = JsonSerializer.Deserialize<InitDTO>(json);
            CurrentEvent.EventGuid = dto.EventGuid;
            CurrentEvent.Sessions.Add(new ObservableSession(dto.Sessions.Single(), this));
            CurrentEvent.CurrentSession = CurrentEvent.Sessions.Single();
            ScoringProgramResponse response = _client.Initialize(dto);
            AddCommunicationResponse(response);
            if (response.DataType == DataConnectorResponseData.OK)
            {
                EventHasLaunched = true;
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            CommunicationResults.Add(new CommunicationResult
            {
                ErrorType = ErrorType.Exception,
                RequestDescription = $"{ScoringProgramDataConnectorCommands.InitializeEvent}",
                ResponseMessage = $"Request failed:{Environment.NewLine}{ex.Message}"
            });
        }
    }

    public string LaunchSelectedInitDtoCommandDescription => nameof(LaunchSelectedInitDtoCommand);

    /// <summary>
    /// Launches an event from a custon initDTO json file.
    /// </summary>
    [RelayCommand]
    private void LaunchSelectedInitDto()
    {
        try
        {
            var json = LoadJsonFile();
            if (string.IsNullOrEmpty(json)) return;

            var initDto = JsonSerializer.Deserialize<InitDTO>(json);
            if (initDto == null) return;

            if (!initDto.Validate())
            {
                var message = string.Join(Environment.NewLine, initDto.ValidationMessages);
                CommunicationResults.Add(new CommunicationResult
                {
                    ErrorType = ErrorType.Validation,
                    RequestDescription = "Invalid InitDto.",
                    ResponseMessage = message

                });
                return;
            }

            CurrentEvent.EventGuid = string.Empty;
            CurrentEvent.Sessions.Clear();
            ScoringProgramResponse response = _client.Initialize(initDto);

            AddCommunicationResponse(response);
            if (response.ErrorType == ErrorType.None)
            {
                CurrentEvent.EventGuid = initDto.EventGuid;
                foreach (var session in initDto.Sessions)
                    CurrentEvent.Sessions.Add(new ObservableSession(session, this));
                CurrentEvent.CurrentSession = CurrentEvent.Sessions.First();

                foreach (var session in CurrentEvent.Sessions)
                    session.HasLaunched = true;

                EventHasLaunched = true;
            }
        }
        catch (Exception ex)
        {
            CommunicationResults.Add(new CommunicationResult
            {
                ErrorType = ErrorType.Exception,
                RequestDescription = "Failed attempt to load json file.",
                ResponseMessage = ex.Message
            });
        }
    }

    public string AddResultCommandDescription=>nameof(AddResultCommand);

    /// <summary>
    /// Add a ResultDTO to the results collection if it validates.
    /// </summary>
    [RelayCommand]
    private void AddResult()
    {
        ValidationMessage = string.Empty;

        ResultDTO newResult;

        if (EnterSpecialResult)
        {
            //Pass, No play, or artificial two sided result.
            ResultDTO result = new ResultDTO
            {
                SessionGuid = CurrentSessionGuid,
                ScoringDirection = SelectedScoringSide.ScoringSide,
                SectionLetters = SelectedContractSection,
                TableNumber = SelectedContractTable,
                RoundNumber = SelectedContractRound,
                BoardNumber = SelectedContractBoard,
                PairNorthSouth = SelectedNsPairNumber,
                PairEastWest = SelectedEwPairNumber,
                DeclaringPair = SelectedDeclaringPairNumber,
                DeclarerDirection = (int)SelectedContractDirectionSelection.Direction,
                Level = SelectedSpecialResultValue.SpecialResultValue,
                Denomination = 0,
                Stake = 0,
                TotalTricks = 0
            };

            newResult = result;
        }
        else
        {
            //Natural result

            ResultDTO result = new ResultDTO
            {
                SessionGuid = CurrentSessionGuid,
                ScoringDirection = SelectedScoringSide.ScoringSide,
                SectionLetters = SelectedContractSection,
                TableNumber = SelectedContractTable,
                RoundNumber = SelectedContractRound,
                BoardNumber = SelectedContractBoard,
                PairNorthSouth = SelectedNsPairNumber,
                PairEastWest = SelectedEwPairNumber,
                DeclaringPair = SelectedDeclaringPairNumber,
                DeclarerDirection = (int)SelectedContractDirectionSelection.Direction,
                Level = SelectedContractLevel,
                Denomination = SelectedDenomination.Denomination,
                Stake = SelectedStake.Stake,
                TotalTricks = SelectedTotalTricksMade
            };

            newResult = result;
        }

        var validated = newResult.Validate();

        if (validated)
        {
            results.Add(newResult);
            NumberOfResults = results.Count;
        }
        else
        {
            ValidationMessage = string.Join("; ", newResult.ValidationMessages);
        }
    }

    public string DeleteResultCommandDescription => nameof(DeleteResultCommand);

    /// <summary>
    /// Deletes the result on the specified table in the specified round. The command is sent immediately to the data connector.
    /// </summary>
    [RelayCommand]
    private void DeleteResult()
    {
        try
        {
            ResultDTO result = new ResultDTO
            {
                SessionGuid = CurrentSessionGuid,
                ScoringDirection = SelectedScoringSide.ScoringSide,
                SectionLetters = SelectedContractSection,
                TableNumber = SelectedContractTable,
                RoundNumber = SelectedContractRound,
                BoardNumber = SelectedContractBoard,
                IsDeleted = true
            };

            ScoringProgramResponse response = _client.SendResults(CurrentSessionGuid, new[] { result });

            AddCommunicationResponse(response);
        }
        catch (Exception ex)
        {
            CommunicationResults.Add(new CommunicationResult
            {
                RequestDescription = $"{ScoringProgramDataConnectorCommands.PutResults}",
                ErrorType = ErrorType.Exception,
                ResponseMessage = ex.Message
            });
        }
    }

    public string SendResultsAdvancedCommandDescription => nameof(SendResultsAdvancedCommand);

    /// <summary>
    /// Sends the ResultDTOs in the result collection to the data connector.
    /// After this the results are cleared in the UI.
    /// </summary>
    [RelayCommand]
    private void SendResultsAdvanced()
    {
        try
        {
            ScoringProgramResponse response = _client.SendResults(CurrentSessionGuid, results.ToArray());
            AddCommunicationResponse(response);
            results.Clear();
            NumberOfResults = 0;
        }
        catch (Exception ex)
        {
            CommunicationResults.Add(new CommunicationResult
            {
                ErrorType = ErrorType.Exception,
                RequestDescription = $"{ScoringProgramDataConnectorCommands.PutResults}",
                ResponseMessage = ex.Message
            });
            Logger.Error(ex);
        }
    }

    public string UpdateNameAdvancedCommandDescription => nameof(UpdateNameAdvancedCommand);

    /// <summary>
    /// Adds or updates player data or specifies where a player is seated in a round.
    /// </summary>
    [RelayCommand]
    private void UpdateNameAdvanced()
    {
        ScoringProgramDataConnectorCommands currentCommand = ScoringProgramDataConnectorCommands.None;
        try
        {
            ScoringProgramResponse result;

            if (AddNewPlayer)
            {
                currentCommand = ScoringProgramDataConnectorCommands.PutPlayerData;
                var playerData = new PlayerDataDTO
                {
                    SessionGuid = CurrentSessionGuid,
                    PlayerNumber = NewPlayerNumber,
                    FirstName = NewPlayerFirstName,
                    LastName = NewPlayerLastName
                };

                result = _client.SendPlayerData(CurrentSessionGuid, new[] { playerData });
                AddCommunicationResponse(result);
            }

            currentCommand = ScoringProgramDataConnectorCommands.PutParticipations;
            var participation = new ParticipationDTO
            {
                SessionGuid = CurrentSessionGuid,
                PlayerNumber = NewPlayerNumber,
                SectionLetters = SelectedSection ?? "A",
                TableNumber = SelectedTableNumber,
                RoundNumber = 1,
                Direction = SelectedDirection?.Direction ?? TableDirection.None
            };

            result = _client.SendParticipations(CurrentSessionGuid, new[] { participation });
            AddCommunicationResponse(result);
        }
        catch (Exception ex)
        {
            CommunicationResults.Add(new CommunicationResult
            {
                ErrorType = ErrorType.Exception,
                RequestDescription = $"{currentCommand}",
                ResponseMessage = ex.Message
            });
            Logger.Error(ex);
        }
    }


    #region Td calls

    [ObservableProperty]
    private string? tdCallSection = "A";

    [ObservableProperty]
    private int tdCallTableNumber = 1;

    [ObservableProperty]
    private int tdCallRoundNumber = 1;

    [ObservableProperty]
    private int tdCallStatus = TdCallDTO.TdCallStatus_Launched;

    [ObservableProperty]
    private int tdCallHour = DateTime.Now.Hour;

    [ObservableProperty]
    private int tdCallMinute = DateTime.Now.Minute;

    [ObservableProperty]
    private string tdCallValidationMessage = string.Empty;

    public IReadOnlyList<int> TdCallStatusValues { get; } = new[]
    {
        TdCallDTO.TdCallStatus_Launched,
        TdCallDTO.TdCallStatus_InProgress,
        TdCallDTO.TdCallStatus_Resolved,
        TdCallDTO.TdCallStatus_Deferred
    };

    public string SendTdCallCommandDescription => nameof(SendTdCallCommand);

    /// <summary>
    /// Sends a single td call status update to the data connector for the current session.
    /// </summary>
    [RelayCommand]
    private async Task SendTdCall()
    {
        TdCallValidationMessage = string.Empty;
        try
        {
            var dto = new TdCallDTO
            {
                SessionGuid = CurrentSessionGuid,
                SectionLetters = TdCallSection ?? string.Empty,
                TableNumber = TdCallTableNumber,
                RoundNumber = TdCallRoundNumber,
                Hour = TdCallHour,
                Minute = TdCallMinute,
                Status = TdCallStatus
            };

            if (!dto.Validate())
            {
                TdCallValidationMessage = string.Join("; ", dto.ValidationMessages);
                return;
            }

            ScoringProgramResponse response = await _client.SendTdCallsAsync(CurrentSessionGuid, new[] { dto });
            AddCommunicationResponse(response);
        }
        catch (Exception ex)
        {
            CommunicationResults.Add(new CommunicationResult
            {
                ErrorType = ErrorType.Exception,
                RequestDescription = $"{ScoringProgramDataConnectorCommands.PutTdCalls}",
                ResponseMessage = ex.Message
            });
            Logger.Error(ex);
        }
    }

    #endregion

    #endregion

    #region Manual event creation

    [ObservableProperty]
    private ObservableCollection<string> validationMessages;

    [ObservableProperty]
    private bool hasValidationMessages;

    [ObservableProperty]
    private List<string> updatableSections;

    [ObservableProperty]
    private string selectedUpdatableSection;

    public string AllSectionDesriptions => string.Join(", ",
        CurrentEvent.Sessions
                    .SelectMany(session => session.GetSections.Select(section => section.Letters)));

    public string AddSessionCommandDescription => nameof(AddSessionCommand);

    /// <summary>
    /// Adds a session to the event.
    /// </summary>
    [RelayCommand]
    private void AddSession()
    {
        SessionDTO newSession = CreateNewSession();
        CurrentEvent.Sessions.Add(new ObservableSession(newSession, this));
        CurrentEvent.CurrentSession = CurrentEvent.Sessions.Last();
        OnPropertyChanged(nameof(AllSectionDesriptions));
    }

    public string LaunchCreatedEventCommandDescription => nameof(LaunchCreatedEventCommand);

    /// <summary>
    /// Launches the event with the created sections. Will erase all previous data of the event if this exists.
    /// </summary>
    [RelayCommand]
    private void LaunchCreatedEvent()
    {
        try
        {
            ValidationMessages.Clear();
            HasValidationMessages = false;
           
            var initDto = new InitDTO
            {
                Commands = InitDTO.StartBCS + InitDTO.Command_Reset + InitDTO.Command_StartReading,
                EventGuid = CurrentEvent.EventGuid,
                Sessions = CurrentEvent.Sessions.Select(session => session.Session).ToArray()
            };

            if (!initDto.Validate())
            {
                foreach (var validationMesage in initDto.ValidationMessages)
                {
                    ValidationMessages.Add(validationMesage);
                }
                HasValidationMessages |= true;
                return;
            }

            var response = _client.Initialize(initDto);

            AddCommunicationResponse(response);
            if (response.ErrorType == ErrorType.None)
            {
                EventHasLaunched = true;
                foreach (var session in CurrentEvent.Sessions)
                    session.HasLaunched = true;
            }
        }
        catch (Exception ex)
        {
            CommunicationResults.Add(new CommunicationResult
            {
                ErrorType = ErrorType.Exception,
                RequestDescription = $"{nameof(LaunchCreatedEvent)}",
                ResponseMessage = ex.Message
            });
        }
    }



    public string AddTwoPairsCommandDescription => nameof(AddTwoPairsCommand);

    /// <summary>
    /// Adds two pairs (one table) to an existing section.
    /// </summary>
    [RelayCommand]
    private void AddTwoPairs()
    {
        if (!_sectionProperties.TryGetValue((CurrentSessionGuid ?? "", SelectedUpdatableSection), out var sectionProperties)) return;
        var numberOfPairs = sectionProperties.numberOfPairs;

        UpdateSection(SelectedUpdatableSection, numberOfPairs + 2);
    }

    public string RemoveTwoPairsCommandDescription => nameof(RemoveTwoPairsCommand);

    /// <summary>
    /// Removes two pairs from an existing section.
    /// </summary>
    [RelayCommand]
    private void RemoveTwoPairs()
    {
        if (!_sectionProperties.TryGetValue((CurrentSessionGuid ?? "", SelectedUpdatableSection), out var sectionProperties)) return;

        var numberOfPairs = sectionProperties.numberOfPairs;
        if (numberOfPairs < 6) return;

        UpdateSection(SelectedUpdatableSection, numberOfPairs - 2);
    }

    /// <summary>
    /// Updates the section by generating a new movement based on the new number of pairs. The previously selected values for
    /// the movement creation and the selected game type do not change.
    /// </summary>
    /// <param name="newNumberOfPairs"></param>
    private void UpdateSection(string sectionLetters, int newNumberOfPairs)
    {
        CurrentSession.UpdateSection(sectionLetters, newNumberOfPairs);
        return;

        try
        {
            var sectionToUpdate = CurrentSession.GetSection(sectionLetters);
            if (sectionToUpdate == null) return;

            var sectionProperties = _sectionProperties[(CurrentSessionGuid ?? "", sectionLetters)];
            var updatedSection = CurrentSession.CreateSection(
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
                Tables = updatedSection.Tables
            };

            var response = _client.UpdateMovement(updateDTO);

            AddCommunicationResponse(response);


        }
        catch (Exception ex)
        {
            CommunicationResults.Add(new CommunicationResult
            {
                ErrorType = ErrorType.Exception,
                RequestDescription = nameof(AddTwoPairs),
                ResponseMessage = ex.Message
            });
        }

    }

    public string DeleteSectionCommandDescription => nameof(DeleteSectionCommand);

    /// <summary>
    /// Deletes the selected section. The data is sent to the data connector immediately.
    /// </summary>
    [RelayCommand]
    private void DeleteSection()
    {
        var existingSection = CurrentSession.GetSection(SelectedUpdatableSection);
        if (existingSection == null) return;

        var updateDto = new SectionUpdateDTO
        {
            SessionGuid = existingSection.SessionGuid,
            Letters = existingSection.Letters,
            IsDeleted = true
        };

        var response = _client.UpdateMovement(updateDto);
        AddCommunicationResponse(response);

    }

    public string ImportPlayerDataCommandDescription => nameof(ImportPlayerDataCommandDescription);

    /// <summary>
    /// Imports player data from a csv file
    /// </summary>
    [RelayCommand]
    private void ImportPlayerData()
    {
        try
        {
            var file = SelectPlayerCsvFile();
            if (string.IsNullOrWhiteSpace(file)) return;

            List<ImportedPlayer> players = ProcessExternalPlayerDataFile(file);
            
            if(!players.Any()) return;

            var dtos = players.Select(player => new PlayerDataDTO
            {
                SessionGuid=CurrentSessionGuid,
                PlayerNumber=player.PlayerNumberId,
                FirstName = player.FirstName,
                LastName = player.LastName,
            }).ToArray();

            ScoringProgramResponse response = _client.SendPlayerData(CurrentSessionGuid, dtos);
            AddCommunicationResponse(response);
        }
        catch (Exception ex)
        {
            CommunicationResults.Add(new CommunicationResult
            {
                RequestDescription = nameof(ImportPlayerData),
                ErrorType = ErrorType.Exception,
                ResponseMessage = ex.Message
            });
        }
    }

    private List<ImportedPlayer>  ProcessExternalPlayerDataFile(string fullPath)
    {
        //var noPlayers = new List<PlayerModel>();
        if (!File.Exists(fullPath))
        {
            return new List<ImportedPlayer>();
        }

        try
        {

            IExternalPlayerDataCommunication? reader = null;

            reader = new ExternalPlayerDataMultiCsvFormatsCommunicator(() => fullPath);

            List<ImportedPlayer> players = reader.GetPlayers();
            if (reader.ImportErrors.Any())
            {
                CommunicationResults.Add(new CommunicationResult
                {
                    RequestDescription= nameof(ImportPlayerData),
                    ErrorType = ErrorType.Validation,
                    ResponseMessage=string.Join(Environment.NewLine, reader.ImportErrors)
                });
               
            }
            if (players.Count > 10000)
            {
                
            }

            return players;
        }
        catch (Exception ex)
        {
            return new List<ImportedPlayer>();
        }
        finally
        {
           
        }

    }


    #endregion

    #endregion

    #region Helper functions

    private void CreateClient()
    {
        if (UseHttp)
        {
            //Credential validation is advisory only for now, but send the club credentials anyway.
            var httpClient = ScoringProgramDataConnectorHttpClient.Instance(clubId: HttpClubId, licenceKey: HttpLicenceKey);
            //An empty base address targets the data connector on this computer; its published port is
            //discovered from the registry (default 5079).
            httpClient.BaseAddress = HttpBaseAddress;
            //Like the pipe client, start the local data connector on Connect if it is not running.
            //This has no effect when the base address points to another computer.
            httpClient.EnsureLocalDataConnectorIsRunning = true;
            _client = httpClient;
        }
        else
        {
            _client = ScoringProgramDataConnectorPipeClient.Instance();
        }
    }

    private string CreateGuid()
    {
        var guid = Guid.NewGuid().ToString("N").ToUpper();
        return guid;
    }

    internal void AddCommunicationResponse(ScoringProgramResponse response)
    {
        try
        {
            var requestMessage = $"{response.RequestCommand}; Response type: {response.DataType};  Response error: {response.ErrorType}";
            string responseMessage;
            if (response.RequestCommand == ScoringProgramDataConnectorCommands.UpdateMovement &&
                response.DataType != DataConnectorResponseData.Warning && response.DataType != DataConnectorResponseData.Error)
            {
                try
                {
                    responseMessage = JsonSerializer.Deserialize<SectionUpdateDTO>(response.SerializedData).ToString();
                }
                catch
                {
                    responseMessage = JsonSerializer.Deserialize<string>(response.SerializedData);
                }
            }
            else
                responseMessage = JsonSerializer.Deserialize<string>(response.SerializedData);

            CommunicationResults.Add(new CommunicationResult
            {
                ErrorType = response.ErrorType,
                RequestDescription = requestMessage,
                ResponseMessage = responseMessage
            });
        }
        catch
        {
            CommunicationResults.Add(new CommunicationResult
            {
                ErrorType = response.ErrorType,
                RequestDescription = response.RequestCommand.ToString(),
                ResponseMessage = response.SerializedData
            });

        }
    }

    private void AddPollResponse(IEnumerable<PlayerDataDTO> playerdata)
    {
        if (!playerdata.Any())
        {
            CommunicationResults.Add(new CommunicationResult
            {
                ErrorType = ErrorType.NoData,
                RequestDescription = PollAll ? ScoringProgramDataConnectorCommands.PollQueueForAllPlayerData.ToString() :
                                              ScoringProgramDataConnectorCommands.PollQueueForNewPlayerData.ToString(),
                ResponseMessage = "No data"
            });
        }
        foreach (var playerdataitem in playerdata)
        {
            CommunicationResults.Add(new CommunicationResult
            {
                ErrorType = ErrorType.None,
                RequestDescription = PollAll ? ScoringProgramDataConnectorCommands.PollQueueForAllPlayerData.ToString() :
                                            ScoringProgramDataConnectorCommands.PollQueueForNewPlayerData.ToString(),
                ResponseMessage = CreatePlayerDataDescription(playerdataitem)
            });
        }
    }

    private void AddPollResponse(IEnumerable<ParticipationDTO> participations)
    {
        if (!participations.Any())
        {
            CommunicationResults.Add(new CommunicationResult
            {
                ErrorType = ErrorType.NoData,
                RequestDescription = PollAll ? ScoringProgramDataConnectorCommands.PollQueueForAllParticipations.ToString() :
                                              ScoringProgramDataConnectorCommands.PollQueueForNewParticipations.ToString(),
                ResponseMessage = "No data"
            });
        }
        foreach (var participation in participations)
        {
            CommunicationResults.Add(new CommunicationResult
            {
                ErrorType = ErrorType.None,
                RequestDescription = PollAll ? ScoringProgramDataConnectorCommands.PollQueueForAllParticipations.ToString() :
                                            ScoringProgramDataConnectorCommands.PollQueueForNewParticipations.ToString(),
                ResponseMessage = CreateParticipationDescription(participation)
            });
        }
    }

    private void AddPollResponse(IEnumerable<ResultDTO> results)
    {
        if (!results.Any())
        {
            CommunicationResults.Add(new CommunicationResult
            {
                ErrorType = ErrorType.NoData,
                RequestDescription = PollAll ? ScoringProgramDataConnectorCommands.PollQueueForAllResults.ToString() :
                                              ScoringProgramDataConnectorCommands.PollQueueForNewResults.ToString(),
                ResponseMessage = "No data"
            });
        }
        foreach (var result in results)
        {
            var contract = CreateContractDescription(result);
            CommunicationResults.Add(new CommunicationResult
            {
                ErrorType = ErrorType.None,
                RequestDescription = PollAll ? ScoringProgramDataConnectorCommands.PollQueueForAllResults.ToString() :
                                               ScoringProgramDataConnectorCommands.PollQueueForNewResults.ToString(),
                ResponseMessage = contract
            });
        }
    }

    private void AddPollResponse(IEnumerable<TdCallDTO> tdCalls)
    {
        if (!tdCalls.Any())
        {
            CommunicationResults.Add(new CommunicationResult
            {
                ErrorType = ErrorType.NoData,
                RequestDescription = PollAll ? ScoringProgramDataConnectorCommands.PollQueueForAllTdCalls.ToString() :
                                              ScoringProgramDataConnectorCommands.PollQueueForNewTdCalls.ToString(),
                ResponseMessage = "No data"
            });
        }
        foreach (var tdCall in tdCalls)
        {
            CommunicationResults.Add(new CommunicationResult
            {
                ErrorType = ErrorType.None,
                RequestDescription = PollAll ? ScoringProgramDataConnectorCommands.PollQueueForAllTdCalls.ToString() :
                                               ScoringProgramDataConnectorCommands.PollQueueForNewTdCalls.ToString(),
                ResponseMessage = CreateTdCallDescription(tdCall)
            });
        }
    }

    private string CreateTdCallDescription(TdCallDTO tdCall)
    {
        var status = tdCall.Status switch
        {
            TdCallDTO.TdCallStatus_Launched => "Launched",
            TdCallDTO.TdCallStatus_InProgress => "In progress",
            TdCallDTO.TdCallStatus_Resolved => "Resolved",
            TdCallDTO.TdCallStatus_Deferred => "Deferred",
            _ => tdCall.Status.ToString()
        };
        return $"Table: {tdCall.SectionLetters}{tdCall.TableNumber} Round: {tdCall.RoundNumber} " +
               $"Time: {tdCall.Hour:00}:{tdCall.Minute:00} Status: {status}";
    }

    private void AddPollResponse(IEnumerable<HandrecordDTO> handrecords)
    {
        if (!handrecords.Any())
        {
            CommunicationResults.Add(new CommunicationResult
            {
                ErrorType = ErrorType.NoData,
                RequestDescription = PollAll ? ScoringProgramDataConnectorCommands.PollQueueForAllHandrecords.ToString() :
                                              ScoringProgramDataConnectorCommands.PollQueueForNewHandrecords.ToString(),
                ResponseMessage = "No data"
            });
        }
        foreach (var handrecord in handrecords)
        {
            CommunicationResults.Add(new CommunicationResult
            {
                ErrorType = ErrorType.None,
                RequestDescription = PollAll ? ScoringProgramDataConnectorCommands.PollQueueForAllHandrecords.ToString() :
                                               ScoringProgramDataConnectorCommands.PollQueueForNewHandrecords.ToString(),
                ResponseMessage = CreateHandrecordDescription(handrecord)
            });
        }
    }

    private bool DetemineIsSwitched()
    {
        if (SelectedDeclaringPairNumber == 0) return false;

        if (SelectedContractDirectionSelection.Direction == TableDirection.North || selectedContractDirectionSelection.Direction == TableDirection.South)
        {
            if (SelectedDeclaringPairNumber == SelectedNsPairNumber)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        if (SelectedContractDirectionSelection.Direction == TableDirection.East || SelectedContractDirectionSelection.Direction == TableDirection.West)
        {
            if (SelectedDeclaringPairNumber == SelectedEwPairNumber)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        return false;
    }

    private string CreatePlayerDataDescription(PlayerDataDTO playerData)
    {
        return $"{playerData.FirstName} {playerData.LastName} ({playerData.PlayerNumber})";
    }

    private string CreateParticipationDescription(ParticipationDTO participation)
    {
        var direction = participation.Direction switch
        {
            TableDirection.North => "North",
            TableDirection.East => "East",
            TableDirection.South => "South",
            TableDirection.West => "West",
            _ => ""
        };

        return $"Table: {participation.SectionLetters}{participation.TableNumber}-{direction}] Round: {participation.RoundNumber}{Environment.NewLine}" +
               $"Number: {participation.PlayerNumber}'; Name: '{participation.FirstName}' '{participation.LastName}''";

    }

    private string CreateContractDescription(ResultDTO result)
    {
        var isNatural = result.Level > 0;
        if (isNatural)
        {
            var direction = result.DeclarerDirection switch
            {
                1 => "North",
                2 => "East",
                3 => "South",
                4 => "West",
                _ => ""
            };

            var denom = result.Denomination switch
            {
                1 => "Clubs",
                2 => "Diamonds",
                3 => "Hearts",
                4 => "Spades",
                5 => "NT",
                _ => ""
            };

            var stake = result.Stake switch
            {
                1 => "x",
                2 => "xx",
                _ => ""
            };

            var leadCard = result.LeadCardRank switch
            {
                var crd when crd >= 2 && crd <= 10 => crd.ToString(),
                11 => "J",
                12 => "Q",
                13 => "K",
                14 => "A",
                _ => ""
            };

            var leadSuit = result.LeadCardSuit switch
            {
                1 => "C",
                2 => "D",
                3 => "H",
                4 => "S",
                _ => ""
            };

            var switched = result.IsSwitched() ? " (switched)" : "";

            return $"Table: {result.SectionLetters}{result.TableNumber} Round: {result.RoundNumber}; NS: {result.PairNorthSouth} - EW: {result.PairEastWest}{Environment.NewLine}" +
                   $"By: {result.DeclaringPair} {direction}{switched}: Result: {result.Level} {denom} {stake}: {leadCard}{leadSuit} ({result.TotalTricks}) ";
        }
        else
        {
            return result.Level switch
            {
                ResultDTO.ContractLevel_Pass => "Pass",
                ResultDTO.ContractLevel_NoPLay => "No Play",
                ResultDTO.AvgMinMin => "A-/A-",
                ResultDTO.AvgMinAvg => "A-/A",
                ResultDTO.AvgMinPlus => "A-/A+",
                ResultDTO.AvgAvgMin => "A/A-",
                ResultDTO.AvgAvgAvg => "A/A",
                ResultDTO.AvgAvgPlus => "A/A+",
                ResultDTO.AvgPlusMin => "A+/A-",
                ResultDTO.AvgPlusAvg => "A+/A",
                ResultDTO.AvgPlusPlus => "A+/A+",
                _ => ""
            };
        }
    }

    private string CreateHandrecordDescription(HandrecordDTO handrecord)
    {
        var hand = $"      {handrecord.NorthSpades}{Environment.NewLine}" +
                   $"      {handrecord.NorthHearts}{Environment.NewLine}" +
                   $"      {handrecord.NorthDiamonds}{Environment.NewLine}" +
                   $"      {handrecord.NorthClubs}{Environment.NewLine}" +
                   $"{handrecord.WestSpades,-7}       {handrecord.EastSpades,-7}{Environment.NewLine}" +
                   $"{handrecord.WestHearts,-7}       {handrecord.EastHearts,-7}{Environment.NewLine}" +
                   $"{handrecord.WestDiamonds,-7}       {handrecord.EastDiamonds,-7}{Environment.NewLine}" +
                   $"{handrecord.WestClubs,-7}       {handrecord.EastClubs,-7}{Environment.NewLine}" +
                   $"      {handrecord.SouthSpades}{Environment.NewLine}" +
                   $"      {handrecord.SouthHearts}{Environment.NewLine}" +
                   $"      {handrecord.SouthDiamonds}{Environment.NewLine}" +
                   $"      {handrecord.SouthClubs}{Environment.NewLine}";

        return $"Scoringgroup: {handrecord.ScoringGroupNumber}; Board: {handrecord.BoardNumber}{Environment.NewLine}" +
               $"{hand}";

    }


    #region Manual event creation

    private string LoadJsonFile()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "JSON files (*.json)|*.json",
            Title = "Select a JSON file",
            Multiselect = false
        };

        if (dialog.ShowDialog() == true)
        {
            return File.ReadAllText(dialog.FileName);
        }

        return string.Empty; // user cancelled
    }

    private string SelectPlayerCsvFile()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "csv player files (*.csv)|*.csv",
            Title = "Select a player data file",
            Multiselect = false
        };

        if (dialog.ShowDialog() == true)
        {
            return dialog.FileName;
        }

        return string.Empty; // user cancelled
    }



    /// <summary>
    /// Creates a new session with one scoring group.
    /// </summary>
    private SessionDTO CreateNewSession()
    {
        var sessionGuid = CreateGuid();

        var now = DateTime.Now;

        var newSession = new SessionDTO
        {
            EventGuid = ObservableEvent.Guid,
            SessionGuid = sessionGuid,
            Name = $"Session {now.ToShortTimeString()}",
            Year = now.Year,
            Month = now.Month,
            Day = now.Day,
            Hour = now.Hour,
            Minute = now.Minute,
            ScoringGroups = Array.Empty<ScoringGroupDTO>()
        };

        return newSession;
    }

    #endregion

    public void RaiseSectionAdded()
    {
        OnPropertyChanged(nameof(AllSectionDesriptions));
    }

    #endregion
}
