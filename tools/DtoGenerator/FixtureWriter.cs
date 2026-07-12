using System.Reflection;
using System.Text;
using System.Text.Json;
using BridgeSystems.Bridgemate.DataConnector.ScoringProgramClient;
using BridgeSystems.Bridgemate.DataConnectorClasses.SharedDTO;

namespace DtoGenerator
{
    /// <summary>
    /// Writes golden request/response JSON files with the exact bytes the .NET client produces
    /// (default System.Text.Json options, nested SerializedData). The PHP and Java test suites
    /// assert structural equality against these files, which makes them the cross-language
    /// compatibility contract.
    /// </summary>
    internal sealed class FixtureWriter
    {
        private const string SampleGuid = "A1B2C3D4E5F60718293A4B5C6D7E8F90";
        private const string SampleClubId = "CLUB001";
        private const string SampleLicenceKey = "LICENCE-KEY-123";
        private const string SampleTicks = "638712345678901234";

        public void Write(string outDir)
        {
            var requestsDir = Path.Combine(outDir, "requests");
            var responsesDir = Path.Combine(outDir, "responses");
            Directory.CreateDirectory(requestsDir);
            Directory.CreateDirectory(responsesDir);

            WriteRequest(requestsDir, ScoringProgramDataConnectorCommands.Ping, "", JsonSerializer.Serialize(SampleTicks));
            //InitializeEvent and ContinueEvent are sent with an empty envelope SessionGuid, exactly like the .NET client.
            WriteRequest(requestsDir, ScoringProgramDataConnectorCommands.InitializeEvent, "", Serialize(Fill<InitDTO>()));
            WriteRequest(requestsDir, ScoringProgramDataConnectorCommands.ContinueEvent, "", Serialize(Fill<ContinueDTO>()));
            WriteRequest(requestsDir, ScoringProgramDataConnectorCommands.UpdateMovement, SampleGuid, Serialize(Fill<SectionUpdateDTO>()));
            WriteRequest(requestsDir, ScoringProgramDataConnectorCommands.UpdateScoringGroups, SampleGuid, Serialize(FillArray<ScoringGroupDTO>(2)));
            WriteRequest(requestsDir, ScoringProgramDataConnectorCommands.PutResults, SampleGuid, Serialize(FillArray<ResultDTO>(2)));
            WriteRequest(requestsDir, ScoringProgramDataConnectorCommands.PutPlayerData, SampleGuid, Serialize(FillArray<PlayerDataDTO>(2)));
            WriteRequest(requestsDir, ScoringProgramDataConnectorCommands.PutParticipations, SampleGuid, Serialize(FillArray<ParticipationDTO>(2)));
            WriteRequest(requestsDir, ScoringProgramDataConnectorCommands.PutHandrecords, SampleGuid, Serialize(FillArray<HandrecordDTO>(2)));
            WriteRequest(requestsDir, ScoringProgramDataConnectorCommands.PutTdCalls, SampleGuid, Serialize(FillArray<TdCallDTO>(2)));
            WriteRequest(requestsDir, ScoringProgramDataConnectorCommands.PutBridgemate2Settings, SampleGuid, Serialize(FillArray<Bridgemate2SettingsDTO>(2)));
            WriteRequest(requestsDir, ScoringProgramDataConnectorCommands.PutBridgemate3Settings, SampleGuid, Serialize(FillArray<Bridgemate3SettingsDTO>(2)));

            foreach (var poll in new[]
                     {
                         ScoringProgramDataConnectorCommands.PollQueueForNewResults,
                         ScoringProgramDataConnectorCommands.PollQueueForAllResults,
                         ScoringProgramDataConnectorCommands.PollQueueForNewPlayerData,
                         ScoringProgramDataConnectorCommands.PollQueueForAllPlayerData,
                         ScoringProgramDataConnectorCommands.PollQueueForNewParticipations,
                         ScoringProgramDataConnectorCommands.PollQueueForAllParticipations,
                         ScoringProgramDataConnectorCommands.PollQueueForNewHandrecords,
                         ScoringProgramDataConnectorCommands.PollQueueForAllHandrecords,
                         ScoringProgramDataConnectorCommands.PollQueueForNewTdCalls,
                         ScoringProgramDataConnectorCommands.PollQueueForAllTdCalls
                     })
            {
                WriteRequest(requestsDir, poll, SampleGuid, "");
            }

            foreach (var accept in new[]
                     {
                         ScoringProgramDataConnectorCommands.AcceptResultQueueItems,
                         ScoringProgramDataConnectorCommands.AcceptPlayerDataQueueItems,
                         ScoringProgramDataConnectorCommands.AcceptParticipantQueueItems,
                         ScoringProgramDataConnectorCommands.AcceptHandrecordQueueItems,
                         ScoringProgramDataConnectorCommands.AcceptTdCallQueueItems
                     })
            {
                WriteRequest(requestsDir, accept, SampleGuid, JsonSerializer.Serialize(42));
            }

            WriteResponse(responsesDir, "Ping", new ScoringProgramResponse
            {
                RequestCommand = ScoringProgramDataConnectorCommands.Ping,
                DataType = DataConnectorResponseData.OK,
                SessionGuid = "",
                SerializedData = JsonSerializer.Serialize(SampleTicks)
            });
            WriteResponse(responsesDir, "InitializeEvent", new ScoringProgramResponse
            {
                RequestCommand = ScoringProgramDataConnectorCommands.InitializeEvent,
                DataType = DataConnectorResponseData.OK,
                SessionGuid = SampleGuid,
                SerializedData = JsonSerializer.Serialize("Initialized")
            });
            WriteResponse(responsesDir, "PollQueueForNewResults", new ScoringProgramResponse
            {
                RequestCommand = ScoringProgramDataConnectorCommands.PollQueueForNewResults,
                DataType = DataConnectorResponseData.Results,
                LastQueueItemId = 42,
                SessionGuid = SampleGuid,
                SerializedData = Serialize(FillArray<ResultDTO>(2))
            });
            WriteResponse(responsesDir, "PollQueueForNewPlayerData", new ScoringProgramResponse
            {
                RequestCommand = ScoringProgramDataConnectorCommands.PollQueueForNewPlayerData,
                DataType = DataConnectorResponseData.PlayerData,
                LastQueueItemId = 42,
                SessionGuid = SampleGuid,
                SerializedData = Serialize(FillArray<PlayerDataDTO>(2))
            });
            WriteResponse(responsesDir, "PollQueueForNewParticipations", new ScoringProgramResponse
            {
                RequestCommand = ScoringProgramDataConnectorCommands.PollQueueForNewParticipations,
                DataType = DataConnectorResponseData.Participations,
                LastQueueItemId = 42,
                SessionGuid = SampleGuid,
                SerializedData = Serialize(FillArray<ParticipationDTO>(2))
            });
            WriteResponse(responsesDir, "PollQueueForNewHandrecords", new ScoringProgramResponse
            {
                RequestCommand = ScoringProgramDataConnectorCommands.PollQueueForNewHandrecords,
                DataType = DataConnectorResponseData.Handrecords,
                LastQueueItemId = 42,
                SessionGuid = SampleGuid,
                SerializedData = Serialize(FillArray<HandrecordDTO>(2))
            });
            WriteResponse(responsesDir, "PollQueueForNewTdCalls", new ScoringProgramResponse
            {
                RequestCommand = ScoringProgramDataConnectorCommands.PollQueueForNewTdCalls,
                DataType = DataConnectorResponseData.TdCalls,
                LastQueueItemId = 42,
                SessionGuid = SampleGuid,
                SerializedData = Serialize(FillArray<TdCallDTO>(2))
            });
            WriteResponse(responsesDir, "AcceptResultQueueItems", new ScoringProgramResponse
            {
                RequestCommand = ScoringProgramDataConnectorCommands.AcceptResultQueueItems,
                DataType = DataConnectorResponseData.OK,
                SessionGuid = SampleGuid,
                SerializedData = JsonSerializer.Serialize("Accepted")
            });
            WriteResponse(responsesDir, "Error", new ScoringProgramResponse
            {
                RequestCommand = ScoringProgramDataConnectorCommands.PutResults,
                DataType = DataConnectorResponseData.Error,
                ErrorType = ErrorType.Validation,
                SessionGuid = SampleGuid,
                SerializedData = JsonSerializer.Serialize("The DTO did not validate.")
            });
        }

        private static void WriteRequest(string dir, ScoringProgramDataConnectorCommands command, string sessionGuid, string serializedData)
        {
            var request = new ScoringProgramRequest
            {
                Command = command,
                ClubId = SampleClubId,
                LicenceKey = SampleLicenceKey,
                SessionGuid = sessionGuid,
                SerializedData = serializedData
            };
            WriteFile(Path.Combine(dir, command + ".json"), JsonSerializer.Serialize(request));
        }

        private static void WriteResponse(string dir, string name, ScoringProgramResponse response)
        {
            WriteFile(Path.Combine(dir, name + ".json"), JsonSerializer.Serialize(response));
        }

        private static void WriteFile(string path, string json)
        {
            File.WriteAllText(path, json, new UTF8Encoding(false));
        }

        private static string Serialize(object value) =>
            JsonSerializer.Serialize(value, value.GetType());

        private static T Fill<T>() where T : new() => (T)FillObject(typeof(T));

        private static T[] FillArray<T>(int count) where T : new()
        {
            var array = new T[count];
            for (var i = 0; i < count; i++)
                array[i] = (T)FillObject(typeof(T));
            return array;
        }

        /// <summary>
        /// Fills every read/write property with a deterministic sample value. The values only need
        /// to exercise the serialization shape; they are not meant to form a playable bridge event.
        /// </summary>
        private static object FillObject(Type type)
        {
            var instance = Activator.CreateInstance(type)!;
            foreach (var prop in TypeGraph.AllPropModels(type))
                prop.Info.SetValue(instance, SampleValue(prop));
            return instance;
        }

        private static object? SampleValue(PropModel prop)
        {
            var name = prop.Name;
            switch (prop.Kind)
            {
                case PropKind.String:
                    if (name.EndsWith("Guid", StringComparison.Ordinal))
                        return SampleGuid;
                    if (name == "ClubId")
                        return SampleClubId;
                    if (name == "LicenceKey")
                        return SampleLicenceKey;
                    if (name is "SectionLetters" or "Letters")
                        return "A";
                    return name + " 1";
                case PropKind.Int:
                    var intValue = name == "Commands" ? 7 : 1;
                    return Convert.ChangeType(intValue, prop.Info.PropertyType);
                case PropKind.Bool:
                    return false;
                case PropKind.StringArray:
                    return null;
                case PropKind.Enum:
                    var members = TypeGraph.EnumMembers(prop.ElementType!);
                    var member = members.FirstOrDefault(m => m.Value != 0);
                    if (member.Name == null)
                        member = members[0];
                    return Enum.Parse(prop.ElementType!, member.Name);
                case PropKind.Dto:
                    return FillObject(prop.ElementType!);
                case PropKind.DtoArray:
                    var count = prop.Name == "Sessions" ? 1 : 2;
                    var array = Array.CreateInstance(prop.ElementType!, count);
                    for (var i = 0; i < count; i++)
                        array.SetValue(FillObject(prop.ElementType!), i);
                    return array;
                default:
                    throw new ArgumentOutOfRangeException(nameof(prop));
            }
        }
    }
}
