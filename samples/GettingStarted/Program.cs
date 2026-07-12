using System.Text.Json;
using BridgeSystems.Bridgemate.DataConnector.ScoringProgramClient;
using BridgeSystems.Bridgemate.DataConnectorClasses.SharedDTO;

namespace BridgeSystems.Bridgemate.DataConnector.ScoringProgramClient.Samples
{
    /// <summary>
    /// Getting-started sample for the Bridgemate Data Connector .NET client, the C# twin of the
    /// samples in the PHP and Java client repositories.
    ///
    /// Interactive:  dotnet run --project samples/GettingStarted
    /// Unattended:   dotnet run --project samples/GettingStarted -- --scenario
    ///
    /// Options: --base-address=http://host:5079  --club-id=...  --licence-key=...  --no-trace
    ///
    /// The "initialize event" action instructs the Data Connector to START Bridgemate Control
    /// Software and create a small test event (1 section, 2 tables, 3 rounds). Watch BCS open,
    /// enter a result there (or on a Bridgemate), then use the poll actions here to receive it.
    ///
    /// Mind: unlike the PHP/Java samples the wire trace here is reconstructed at the application
    /// level (the .NET client builds the http envelope internally), but it carries the same
    /// information: the command, the payload and the full response envelope.
    /// </summary>
    internal static class Program
    {
        private static readonly JsonSerializerOptions Pretty = new() { WriteIndented = true };
        private static readonly string StateFile = ".state.json";

        private static IScoringProgramClient _client = null!;
        private static bool _trace = true;

        private static int Main(string[] args)
        {
            Console.WriteLine("=== Bridgemate Data Connector getting-started sample - C# client ===");
            Console.WriteLine($"Running on .NET {Environment.Version} ({Environment.OSVersion.Platform})");

            var clubId = OptionValue(args, "--club-id") ?? "";
            var licenceKey = OptionValue(args, "--licence-key") ?? "";
            var baseAddress = OptionValue(args, "--base-address");
            _trace = !args.Contains("--no-trace");

            var httpClient = baseAddress != null
                ? ScoringProgramDataConnectorHttpClient.Instance(clubId, licenceKey, baseAddress)
                : ScoringProgramDataConnectorHttpClient.Instance(clubId, licenceKey);
            _client = httpClient;
            Console.WriteLine($"Data Connector address: {httpClient.UrlRoot}");

            if (args.Contains("--scenario"))
            {
                return RunScenario() ? 0 : 1;
            }
            RunMenu();
            return 0;
        }

        //----------------------------------------------------------------------------------------
        // Actions. Each one is a plain use of the client, so this file doubles as sample code.
        //----------------------------------------------------------------------------------------

        private static void ConnectAndPing()
        {
            Report("Connect", _client.Connect());
            Report("Ping", _client.Ping());
        }

        /// <summary>
        /// Creates a fresh event from the template: new event/session guids and today's date.
        /// Commands = 7 tells the Data Connector to start BCS (1), reset (2) and start reading (4).
        /// </summary>
        private static ScoringProgramResponse InitializeEvent()
        {
            var sessionGuid = NewGuid();
            var eventGuid = NewGuid();
            var template = File.ReadAllText(DataFile("init-template.json")).Replace("REPLACED-AT-RUNTIME", sessionGuid);
            var initDto = JsonSerializer.Deserialize<InitDTO>(template)!;
            initDto.EventGuid = eventGuid;
            var now = DateTime.Now;
            var session = initDto.Sessions[0];
            session.EventGuid = eventGuid;
            session.Year = now.Year;
            session.Month = now.Month;
            session.Day = now.Day;
            session.Hour = now.Hour;
            session.Minute = now.Minute;
            initDto.PlayerData = LoadPlayerData(sessionGuid);
            initDto.Participations = LoadParticipations(sessionGuid);

            Trace(ScoringProgramDataConnectorCommands.InitializeEvent, initDto);
            var response = _client.Initialize(initDto);
            SaveState(sessionGuid, eventGuid);
            Console.WriteLine($"Session guid: {sessionGuid} (saved to .state.json)");
            return response;
        }

        private static ScoringProgramResponse ContinueEvent()
        {
            var continueDto = new ContinueDTO
            {
                EventGuid = State().eventGuid,
                //Unlike InitDTO, ContinueDTO must not carry the Reset flag (2): only start BCS (1),
                //start reading (4) and optionally clear data (128), minimize, auto-shutdown or debug logging.
                Commands = 5
            };
            Trace(ScoringProgramDataConnectorCommands.ContinueEvent, continueDto);
            return _client.Continue(continueDto);
        }

        private static ScoringProgramResponse SendPlayerData()
        {
            var sessionGuid = State().sessionGuid;
            var playerData = LoadPlayerData(sessionGuid);
            Trace(ScoringProgramDataConnectorCommands.PutPlayerData, playerData);
            return _client.SendPlayerData(sessionGuid, playerData);
        }

        /// <summary>
        /// Uploads one board result: table 1, round 1, board 1 - 1 Clubs by North, 7 tricks, lead club ace.
        /// </summary>
        private static ScoringProgramResponse SendOneResult()
        {
            var sessionGuid = State().sessionGuid;
            var result = new ResultDTO
            {
                SessionGuid = sessionGuid,
                SectionLetters = "A",
                TableNumber = 1,
                RoundNumber = 1,
                BoardNumber = 1,
                ScoringDirection = ResultDTO.ScoringDirection_NSEW,
                PairNorthSouth = 1,
                PairEastWest = 2,
                DeclaringPair = 1,
                DeclarerDirection = ResultDTO.Direction_North,
                Level = 1,
                Denomination = ResultDTO.Denomination_Clubs,
                Stake = ResultDTO.Stake_Normal,
                TotalTricks = 7,
                LeadCardRank = 14,
                LeadCardSuit = 1
            };
            Trace(ScoringProgramDataConnectorCommands.PutResults, new[] { result });
            return _client.SendResults(sessionGuid, new[] { result });
        }

        private static void PollQueue(DataConnectorResponseData dataType, bool all)
        {
            var sessionGuid = State().sessionGuid;
            object[] items = dataType switch
            {
                DataConnectorResponseData.Results => _client.PollForResults(sessionGuid, all),
                DataConnectorResponseData.PlayerData => _client.PollForPlayerData(sessionGuid, all),
                DataConnectorResponseData.Participations => _client.PollForParticipations(sessionGuid, all),
                DataConnectorResponseData.Handrecords => _client.PollForHandrecords(sessionGuid, all),
                DataConnectorResponseData.TdCalls => _client.PollForTdCalls(sessionGuid, all),
                _ => Array.Empty<object>()
            };
            Console.WriteLine($"Polled {dataType}: {items.Length} item(s)");
            for (var i = 0; i < items.Length; i++)
            {
                Console.WriteLine($"  #{i + 1}");
                Console.WriteLine(Indent(Indent(JsonSerializer.Serialize(items[i], items[i].GetType(), Pretty))));
            }
            if (items.Length > 0)
            {
                Console.WriteLine($"Last queue item id: {LastQueueItemId(dataType)} - use 'accept' so they are not sent again.");
            }
        }

        private static void AcceptQueue(DataConnectorResponseData dataType)
        {
            Report($"Accept {dataType}", _client.AcceptQueueData(State().sessionGuid, dataType));
        }

        //----------------------------------------------------------------------------------------
        // Scenario mode: the whole flow in one run. Returns true when every step succeeded.
        //----------------------------------------------------------------------------------------

        private static bool RunScenario()
        {
            var ok = true;
            ok &= Check("Connect", _client.Connect());
            ok &= Check("Ping", _client.Ping());
            ok &= Check("InitializeEvent", InitializeEvent());
            ok &= Check("PutPlayerData", SendPlayerData());
            ok &= Check("PutResults", SendOneResult());
            PollQueue(DataConnectorResponseData.Results, all: false);
            Console.WriteLine(ok ? "SCENARIO OK" : "SCENARIO FAILED");
            return ok;
        }

        private static bool Check(string step, ScoringProgramResponse response)
        {
            var success = response.DataType != DataConnectorResponseData.Error;
            Console.WriteLine($"{step,-20} {(success ? "OK" : "FAILED: " + response.ErrorType)}");
            return success;
        }

        //----------------------------------------------------------------------------------------
        // Interactive menu
        //----------------------------------------------------------------------------------------

        private static void RunMenu()
        {
            var pollAll = false;
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine(" 1  Connect + ping");
                Console.WriteLine(" 2  Initialize event (starts BCS, creates a fresh test event)");
                Console.WriteLine(" 3  Continue event (re-open the event from .state.json)");
                Console.WriteLine(" 4  Send player data");
                Console.WriteLine(" 5  Send a board result (A1, round 1, board 1)");
                Console.WriteLine(" 6  Poll results            7  Accept results");
                Console.WriteLine(" 8  Poll player data        9  Accept player data");
                Console.WriteLine("10  Poll participations    11  Accept participations");
                Console.WriteLine("12  Poll handrecords       13  Accept handrecords");
                Console.WriteLine("14  Poll TD calls          15  Accept TD calls");
                Console.WriteLine($"16  Toggle 'poll all' (currently: {(pollAll ? "all items" : "new items only")})");
                Console.WriteLine($"17  Toggle trace (currently: {(_trace ? "on" : "off")})");
                Console.WriteLine(" 0  Quit");
                Console.WriteLine();
                Console.Write("Choice: ");
                var choice = Console.ReadLine()?.Trim() ?? "0";
                try
                {
                    switch (choice)
                    {
                        case "1": ConnectAndPing(); break;
                        case "2": Report("InitializeEvent", InitializeEvent()); break;
                        case "3": Report("ContinueEvent", ContinueEvent()); break;
                        case "4": Report("PutPlayerData", SendPlayerData()); break;
                        case "5": Report("PutResults", SendOneResult()); break;
                        case "6": PollQueue(DataConnectorResponseData.Results, pollAll); break;
                        case "7": AcceptQueue(DataConnectorResponseData.Results); break;
                        case "8": PollQueue(DataConnectorResponseData.PlayerData, pollAll); break;
                        case "9": AcceptQueue(DataConnectorResponseData.PlayerData); break;
                        case "10": PollQueue(DataConnectorResponseData.Participations, pollAll); break;
                        case "11": AcceptQueue(DataConnectorResponseData.Participations); break;
                        case "12": PollQueue(DataConnectorResponseData.Handrecords, pollAll); break;
                        case "13": AcceptQueue(DataConnectorResponseData.Handrecords); break;
                        case "14": PollQueue(DataConnectorResponseData.TdCalls, pollAll); break;
                        case "15": AcceptQueue(DataConnectorResponseData.TdCalls); break;
                        case "16": pollAll = !pollAll; break;
                        case "17": _trace = !_trace; break;
                        case "0": return;
                        default: Console.WriteLine("Unknown choice."); break;
                    }
                }
                catch (InvalidOperationException e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                }
            }
        }

        //----------------------------------------------------------------------------------------
        // Helpers
        //----------------------------------------------------------------------------------------

        private static void Report(string action, ScoringProgramResponse response)
        {
            Console.WriteLine($"{action} -> DataType={response.DataType} ErrorType={response.ErrorType}");
            Console.WriteLine(Indent(PrettyData(response.SerializedData)));
        }

        /// <summary>
        /// Prints the command and its payload before sending, mirroring the wire trace of the
        /// PHP/Java samples.
        /// </summary>
        private static void Trace(ScoringProgramDataConnectorCommands command, object payload)
        {
            if (!_trace)
            {
                return;
            }
            Console.WriteLine($">> {command}");
            Console.WriteLine(Indent(JsonSerializer.Serialize(payload, payload.GetType(), Pretty)));
        }

        /// <summary>
        /// Renders the (JSON string) payload of a response in full, pretty-printed.
        /// </summary>
        private static string PrettyData(string? serializedData)
        {
            if (string.IsNullOrWhiteSpace(serializedData))
            {
                return "(no data)";
            }
            try
            {
                var decoded = JsonSerializer.Deserialize<JsonElement>(serializedData);
                if (decoded.ValueKind == JsonValueKind.String)
                {
                    return decoded.GetString()!.Replace("\r\n", "\n").Replace("\r", "\n");
                }
                return JsonSerializer.Serialize(decoded, Pretty);
            }
            catch (JsonException)
            {
                return serializedData;
            }
        }

        private static string Indent(string text) => "  " + text.Replace("\n", "\n  ");

        private static string NewGuid() => Guid.NewGuid().ToString("N").ToUpperInvariant();

        private static int LastQueueItemId(DataConnectorResponseData dataType)
        {
            var manager = (ScoringProgramDataConnectorClientCommandManager)_client;
            return dataType switch
            {
                DataConnectorResponseData.Results => manager.LastResultQueueItemId,
                DataConnectorResponseData.PlayerData => manager.LastPlayerDataQueueItemId,
                DataConnectorResponseData.Participations => manager.LastParticipantQueueItemId,
                DataConnectorResponseData.Handrecords => manager.LastHandrecordQueueItemId,
                DataConnectorResponseData.TdCalls => manager.LastTdCallQueueItemId,
                _ => 0
            };
        }

        private sealed record PlayerSeed(
            string PlayerNumber, string FirstName, string LastName, string CountryCode,
            string SectionLetters, int TableNumber, int Direction);

        private static PlayerSeed[] LoadPlayers() =>
            JsonSerializer.Deserialize<PlayerSeed[]>(File.ReadAllText(DataFile("players.json")))!;

        private static PlayerDataDTO[] LoadPlayerData(string sessionGuid) =>
            LoadPlayers().Select(player => new PlayerDataDTO
            {
                SessionGuid = sessionGuid,
                PlayerNumber = player.PlayerNumber,
                FirstName = player.FirstName,
                LastName = player.LastName,
                CountryCode = player.CountryCode
            }).ToArray();

        /// <summary>
        /// Round-1 seating for the players in players.json.
        /// </summary>
        private static ParticipationDTO[] LoadParticipations(string sessionGuid) =>
            LoadPlayers().Select(player => new ParticipationDTO
            {
                SessionGuid = sessionGuid,
                SectionLetters = player.SectionLetters,
                TableNumber = player.TableNumber,
                Direction = (TableDirection)player.Direction,
                RoundNumber = 1,
                PlayerNumber = player.PlayerNumber
            }).ToArray();

        private static string DataFile(string name)
        {
            var local = Path.Combine("data", name);
            return File.Exists(local) ? local : Path.Combine(AppContext.BaseDirectory, "data", name);
        }

        private static (string sessionGuid, string eventGuid) State()
        {
            if (!File.Exists(StateFile))
            {
                throw new InvalidOperationException("No event yet: run \"Initialize event\" first.");
            }
            var state = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(StateFile))!;
            return (state["sessionGuid"], state["eventGuid"]);
        }

        private static void SaveState(string sessionGuid, string eventGuid)
        {
            var state = new Dictionary<string, string> { ["sessionGuid"] = sessionGuid, ["eventGuid"] = eventGuid };
            File.WriteAllText(StateFile, JsonSerializer.Serialize(state, Pretty));
        }

        private static string? OptionValue(string[] args, string name) =>
            args.FirstOrDefault(arg => arg.StartsWith(name + "=", StringComparison.Ordinal))?[(name.Length + 1)..];
    }
}
