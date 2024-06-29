# Overview of communication

This section decribes how to exchange data with the Bridgemate Data Connector.

![Image](<lib/Data exchange 1.png>)

# General description

## Sending data

The scoring program wants to send a command to the Data Connector with accompanying data. The scoring program constructs a DTO containing the data the Data Connector needs to work with. The type of DTO depends on the requested action as expressed in the command. The DTO is serialized as JSON and added to a ScoringProgramRequest class.This request contains a command that specifies what Data Connector should do. The request must contain the guid of the session that the request belongs to. The request is then serialized a second time and sent to the Data Connector over the communication channel.The Data Connector will always respond. The scoring program needs to wait for this response and consume it. Otherwise communication will stall. Mind that nearly all communications are scoped to a single session as expressed by the SessionGuid property of the request. So, say, a request for new board results must be done for each session that the scoring program currently administers.

## Receiving the response

The Data Connector serializes its response data as JSON. Then it constructs a ScoringProgramResponse with this data. The response contains the original command from the request and the session guid that was sent with it in order to check if the response indeed is the answer to the request that was sent.The response has a DataType property that specifies to what DTO the JSON data can be deserialized. The scoring program waits for the response on the communication channel and deserializes it to a ScoringProgramResponse. Depending on the response's DataType the response's SerializedData can be deserialized from JSON to the appropriate DTO (or to a message).

# Overview of the commands for the ScoringProgramRequest

The tables below give a short description of the available commands for the ScoringProgramRequest and the associated DTO that must be sent with it.

&nbsp;

## Sending data to the Bridgemate Data Connector

| **Command** | **Value** | **Description** | **DTO** | **Remarks** |
| --- | --- | --- | --- | --- |
| Connect | &#49; | Connects to the communication channel | n/a | Does not transfer any data |
| Disconnect | &#51; | Disconnects from the communication channel | n/a | Does not transfer any data.The communication channel will wait for a new connection. |
| Ping | &#52; | Sends a string to the Data Connector and awaits its return | string | Serialize a custom string. On deserialzing the response's SerializedData the same string should return. Can be used to check if the communication channel is alive. |
| InitializeEvent | &#53; | Sends start-up parameters for BCS and inititialization data&nbsp; | [InitDTO](<InitDTO.md>) | The InitDTO contains both start-up parameters for BCS and the data for the sessions like movements, player data and hand records. With this data a new scoring file will be created for internal use by BCS. |
| ContinueEvent | &#51;1 | Sends start-up parameters for BCS without initialization data | n/a | Instructs the Data Connector to start up BCS if it is not running and to continue processing data using an existing scoring file |
| ManageBCS | &#51;4 | Can send a request asking BCS to shut down immediately and can query information on administered events | [BCSManagementRequestDTO](<BCSManagementRequestDTO.md>) | Does not transfer any data. |
| UpdateMovement | &#54; | Instructs BCS to change the movement for the given section, or to delete it. | [SectionDTO](<SectionDTO.md>) | The SectionDTO holds Tables as an array of TableDTOs. Each TableDTO holds Rounds as an array of RoundDTOs. Each RoundDTO contains data on the opponents and the boards played. The SectionDTO holds a IsDeleted property that can be set to 'True' if the section must be deleted. If the section does not yet exist, it will be added. |
| UpdateScoringGroups | &#56; | Instructs BCS to update its scoring groups | [Array of ScoringGroupDTO](<ScoringGroupDTO.md>) | All scoring groups for the session with their sections must be sent as all sections must have a parent scoring group. Mind to use this command before adding a new section using the UpdateMovement command that has a new scoring group. |
| PutResults | &#49;0 | Sends board results to the Data Connector | [Array of ResultDTO](<ResultDTO.md>) | Using this command new or modified board results from the scoring program can be sent to the Bridgemates. |
| PutPlayerData | &#49;1 | Sends player data to the Data Connector | [Array of PlayerDataDTO](<PlayerDataDTO.md>) | Player data contains a person's&nbsp; first name, last name, organization id and playernumber. The player numbers must be known to BCS before players can make themselves be known by entering their number on the Bridgemate.&nbsp; The player data can&nbsp; be included in the InitDTO or sent seperately. The first method is more performant. |
| PutParticipations | &#49;2 | Sends participations to the Data Connector | [Array of ParticipationDTO](<ParticipationDTO.md>) | The participation contains information on the starting position of a player: section, table, round and seat direction. The player can either be identified by a player number, in which case the Data Connector will look up the first and last names or by first name and last name only. The participations can&nbsp; be included in the InitDTO or sent seperately. The first method is more performant.&nbsp; |
| PutHandrecords | &#49;3 | Sends handrecords to the Data Connector | Array of HandrecordDTO | The handrecords must have a valid scoringgroup id. Make sure to add the scoringgroup first using the UpdateScoringGroup command if a new section was added. The handrecords can&nbsp; be included in the InitDTO or sent seperately. The first method is more performant. |
| PutBridgemate2Settings | &#49;4 | Sends the Bridgemate 2 settings | [Array of Bridgemate2SettingsDTO](<Bridgemate2SettingsDTO.md>) | The settings must be sent for each section seprately. The settings can be included in the InitDTO or sent seperately. The first method is more performant. |
| PutBridgemate3Settings | &#49;5 | Sends the Bridgemate 2 settings | [Array of Bridgemate3SettingsDTO](<Bridgemate3SettingsDTO.md>) | The settings must be sent for each section seprately. The settings can be included in the InitDTO or sent seperately. The first method is more performant. |


## Polling data from the Bridgemate Data Connector

| **Command** | **Value** | **Description** | **DTO** | **Remarks** |
| --- | --- | --- | --- | --- |
| PollQueueForNewResults | &#49;9 | Polls for new board results | returns: array of [ResultDTO](<ResultDTO.md>) | Returns the new board results since the last batch was acccepted. |
| PollQueueForNewPlayerData | &#50;0 | Polls for new player data | returns: array of [PlayerDataDTO](<PlayerDataDTO.md>) | Returns the new player data since the last batch was acccepted. |
| PollQueueForNewParticipations | &#50;1 | Polls for new participations | returns: array of [ParticipationDTO](<ParticipationDTO.md>) | Returns the new participations since the last batch was acccepted. |
| PollQueueForNewHandrecords | &#50;2 | Polls for new handrecords | returns: array of HandrecordDTO | Returns the new handrecords since the last batch was acccepted. |
| PollQueueForAllResults | &#50;3 | Requests all board results, irrespective of them having been accepted before. | returns: array of [ResultDTO](<ResultDTO.md>) | Returns all board results that have been created by BCS. |
| PollQueueForAllPlayerData | &#50;4 | Requests all player data, irrespective of them having been accepted before. | returns: array of [PlayerDataDTO](<PlayerDataDTO.md>) | Returns all playerdata that has been created by BCS |
| PollQueueForAllParticipations | &#50;5 | Requests all participations, irrespective of them having been accepted before. | returns: array of [ParticipationDTO](<ParticipationDTO.md>) | Returns all participations that have been created by BCS |
| PollQueueForAllHandrecords | &#50;6 | Requests all handrecords results, irrespective of them having been accepted before. | returns: array of HandrecordDTO | Returns all handrecords that have been created by BCS. |
| AcceptResultQueueItems | &#50;7 | Signals to the Data Connector to not send board results that have been sent before. | The id as serialized integer of the last processed result queue item | Signals to the Data Connector to not send board results that have been sent before. |
| AcceptPlayerDataQueueItems | &#50;8 | Signals to the Data Connector to not send playerdata that has been sent before. | The id as serialized integer of the last processed player data queue item | Signals to the Data Connector to not send playerdata that has been sent before. |
| AcceptParticipationQueueItems | &#50;9 | Signals to the Data Connector to not send participations that have been sent before. | The id as serialized integer of the last participation queue item | Signals to the Data Connector to not send participations that have been sent before. |
| AcceptHandrecordQueueItems | &#51;0 | Signals to the Data Connector to not send handrecords that have been sent before. | The id as serialized integer of the last processed handrecord queue item | Signals to the Data Connector to not send handrecords that have been sent before. |
| GetMovement | &#51;2 | Requests the movement for a specific section | sends: [sectionDTO](<SectionDTO.md>) with the desired SessionGuid and Letters. returns: sectionDTO | The sectionDTO has Tables as an array of TableDTO, eacht table has Rounds as an array of RoundDTO. The RoundDTO contains the opponents and the board numbers that they will play. |
| GetAllMovements | &#51;3 | Requests all movements for a session. | sends: n/a returns: an array of [SectionDTO](<SectionDTO.md>) | Each sectionDTO has Tables as an array of TableDTO, eacht table has Rounds as an array of RoundDTO. The RoundDTO contains the opponents and the board numbers that they will play. |


## Data types

The ScoringProgramResponse has a DataType property which specifies which type of data can be expected to be in its SerializedData property.

| **Value** | **Name** | **Description** |
| --- | --- | --- |
| &#49; | OK | The scoring program request was handled correctly. No serialized data is included in the response |
| &#50; | Not in use |  |
| &#51; | Not in use |  |
| &#52; | Error | There was an error in processing the scoring program request. Examine the ErrotType property and deserialize the serialized data as a a string for detailed information |
| &#53; | InitiData | Not in use for the scoring program client |
| &#54; | ContinueData | Not in use for the scoring program client |
| &#55; | SectionData | Not in use for the scoring program client |
| &#56; | Bridgemate2Settings | Not in use for the scoring program client |
| &#57; | Bridgemate3Settings | Not in use for the scoring program client |
| &#49;0 | PlayerData | The serialized data is an array of [PlayerDataDTO](<PlayerDataDTO.md>) |
| &#49;1 | Participations | The serialized data is an array of [ParticipationDTO](<ParticipationDTO.md>) |
| &#49;2 | Results | The serialized data is an array of [ResultDTO](<ResultDTO.md>) |
| &#49;3 | Handrecords | The serialized data is an array of HandrecordDTO |
| &#49;4 | Movement | The serialized data is a [SectionDTO](<SectionDTO.md>) |
| &#49;5 | Sessions | The serialized data is an array of [SectionDTO](<SectionDTO.md>) |
| &#49;6 | EventInfo | The serialized data is BCSManagementResponseDTO containing an array of SessionInfoDTO |
| &#49;7 | AllSessionsInfo | The serialized data is BCSManagementResponseDTO containing an array of SessionInfoDTO |
| &#49;8 | ScoringFileLocation | The serialized data is BCSManagementResponseDTO |
| &#49;9 | ShutDownRequest | Not in use for the scoring program client |


## Error codes

When the ScoringProgramResponse.DataType property is "Error" (4) then the ScoringProgramResponse.ErrorType property will hold a value further explaining what went wrong. Moreover the SerializedData property can be desierialized to a string to obtain detailed debugging information. This information is not meant to be shown to the end users of the external scoring program.

| **Value** | **Name** | **Description** |
| --- | --- | --- |
| &#48; | None | The ScoringProgramRequest was handled succesfully |
| &#49; | Busy | The ScoringProgramRequest could not be processed because a previous request has not yet been completed. Try again. |
| &#50; | NoData | The ScoringProgramRequest.Command&nbsp; requires data to be sent with it, but there is none. |
| &#51; | NoUpdates | The ScoringProgramRequest.Command&nbsp; included data to be updated, but the said data is already present. |
| &#52; | Movement | The iincluded data did not comply to a known section, table in a section or round on a table. |
| &#53; | Validation | The sent data did not pass validation. Deserialize the SerializedData to a string for details. |
| &#54; | EntryUnknown | The provided data has a (composite) primary key that can not be computed. Deserialize the SerializedData to a string for details. |
| &#55; | Exception | An error occurred while processing the data. Study the DataConnector.log to find details. |
| &#56; | NotImplemented | The requested command is not implemented |
| &#57; | EmptyResponse | The&nbsp; Data Connector did not respond to the request. |
| &#49;0 | NoConnection | The communication with the Data Connector is broken |
| &#49;1 | TimeOut | The request was blocked by a previously sent long running operation on the.Data Connector. |
| &#49;2 | WrongDataType | The datatype of the dtos did not conform to the request command. |
| &#49;3 | UnexpectedCommand | The response command did not conform to the request command. |
| &#49;4 | Unknown | Unknown error. Currently not in use. |


# Code examples

Below a typical piece of code to send a request to the Data Connector and awaiting the response: using the Ping command

&nbsp;&nbsp; &nbsp; /// \<summary\>

&nbsp;&nbsp; &nbsp; /// Communicates to the DataConnector to see if it is responsive.

&nbsp;&nbsp; &nbsp; /// The Ping command returns the exact data that was sent with it.

&nbsp;&nbsp; &nbsp; /// \</summary\>

&nbsp;&nbsp; &nbsp; /// \<returns\>\</returns\>

&nbsp;&nbsp; &nbsp; public async Task\<ScoringProgramResponse\> Ping()

&nbsp;&nbsp; &nbsp; {

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; var requestTicks = DateTime.Now.Ticks.ToString();

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; var serializedTicks=JsonSerializer.Serialize(requestTicks);

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; var response = await SendDataAsync(sessionGuid: string.Empty, ScoringProgramMiddleManCommands.Ping, serializedTicks);

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; if (response.RequestCommand \!= ScoringProgramMiddleManCommands.Ping)

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; {

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; return new ScoringProgramResponse

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; {

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; RequestCommand = ScoringProgramMiddleManCommands.Ping,

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; DataType = MiddleManResponseData.Error,

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; ErrorType = ErrorType.Unknown,

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; SerializedData = JsonSerializer.Serialize($"Invalid command in reponse to {nameof(ScoringProgramMiddleManCommands.Ping)}: " +

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; $"'{response.RequestCommand}'")

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; };

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; }

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; if (response.DataType \!= MiddleManResponseData.OK)

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; return response;

&nbsp;

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; var responseTicks = JsonSerializer.Deserialize\<string\>(response.SerializedData);

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; var error = responseTicks \!= requestTicks;

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; return new ScoringProgramResponse

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; {

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; RequestCommand = ScoringProgramMiddleManCommands.Ping,

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; DataType = requestTicks == responseTicks ? MiddleManResponseData.OK : MiddleManResponseData.Error,

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; ErrorType = error ? ErrorType.Validation : ErrorType.None,

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; SerializedData = response.SerializedData

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; };

&nbsp;&nbsp; &nbsp; }

&nbsp;

&nbsp;&nbsp; /// \<summary\>

&nbsp;&nbsp; /// The code that handles the actual sending of requests and reading their reponses.

&nbsp;&nbsp; /// \</summary\>

&nbsp;&nbsp; /// \<param name="sessionGuid"\>Specifies which session the request targets (if any)\</param\>

&nbsp;&nbsp; /// \<param name="command"\>The command to the middlleman\</param\>

&nbsp;&nbsp; /// \<param name="serializedData"\>The data to send to the middleman as json data. (If any)\</param\>

&nbsp;&nbsp; /// \<returns\>\</returns\>

&nbsp;&nbsp; private async Task\<ScoringProgramResponse\> SendDataAsync(string sessionGuid, ScoringProgramMiddleManCommands command, string serializedData)

&nbsp;&nbsp; {

&nbsp;&nbsp; &nbsp; &nbsp; //Construct the request to the Middleman.

&nbsp;&nbsp; &nbsp; &nbsp; var request = new ScoringProgramRequest

&nbsp;&nbsp; &nbsp; &nbsp; {

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; Command = command,

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; SessionGuid = sessionGuid,

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; SerializedData = serializedData

&nbsp;&nbsp; &nbsp; &nbsp; };

&nbsp;

&nbsp;&nbsp; &nbsp; &nbsp; //Serialize it.

&nbsp;&nbsp; &nbsp; &nbsp; var requestSerialized = JsonSerializer.Serialize(request);

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;

&nbsp;&nbsp; &nbsp; &nbsp; //Do not proceed if sending is already in progress (for an other request). There can be only on request be sent at the same time.

&nbsp;&nbsp; &nbsp; &nbsp; if (\_isSending)

&nbsp;&nbsp; &nbsp; &nbsp; {

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; return new ScoringProgramResponse

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; {

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; RequestCommand = command,

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; SessionGuid = sessionGuid,

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; DataType = MiddleManResponseData.Error,

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; ErrorType = ErrorType.Busy,

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; SerializedData = JsonSerializer.Serialize($"Client is busy, please retry later.")

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; };

&nbsp;&nbsp; &nbsp; &nbsp; }

&nbsp;&nbsp; &nbsp; &nbsp; try

&nbsp;&nbsp; &nbsp; &nbsp; {

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; \_isSending = true;

&nbsp;

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; //Do not continue if the connection has been broken. Call the Connect method again then resend.

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; var errorResponse = CheckConnection(command);

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; if (errorResponse \!= null)

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; {

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; return errorResponse;

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; }

&nbsp;

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; //Reconnect to the Middleman if needed.

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; if (\!MiddleManStream.IsConnected)

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; {

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; await MiddleManStream.ConnectAsync(5000);

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; }

&nbsp;

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; //Send the request to the Middleman. Mind: as it is written now this is a blocking call.

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; //However, in .Net an exception will be thrown if the connection has gone dead for whatever reason.

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; await MiddleManWriter.WriteLineAsync(requestSerialized);

&nbsp;

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; //Wait for the response. This too is a blocking call. But in .Net a broken connection will throw an exception.

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; string response = await MiddleManReader.ReadLineAsync();

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; if (response \!= null)

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; {

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; var responseDeserialized = JsonSerializer.Deserialize\<ScoringProgramResponse\>(response);

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; return responseDeserialized ??

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; new ScoringProgramResponse

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; {

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; RequestCommand = command,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; DataType = MiddleManResponseData.Error,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; SerializedData = JsonSerializer.Serialize("Empty response")

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; };

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; }

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; else

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; {

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; return new ScoringProgramResponse

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; {

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; RequestCommand = command,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; DataType = MiddleManResponseData.Error,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; SerializedData = JsonSerializer.Serialize("Empty response")

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; };

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; }

&nbsp;&nbsp; &nbsp; &nbsp; }

&nbsp;&nbsp; &nbsp; &nbsp; catch (IOException)

&nbsp;&nbsp; &nbsp; &nbsp; {

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; CloseConnection();

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; return

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; new ScoringProgramResponse

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; {

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; RequestCommand = command,

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; DataType = MiddleManResponseData.Error,

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; SerializedData = JsonSerializer.Serialize("Pipe broken")

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; };

&nbsp;&nbsp; &nbsp; &nbsp; }

&nbsp;&nbsp; &nbsp; &nbsp; catch (Exception ex)

&nbsp;&nbsp; &nbsp; &nbsp; {

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; CloseConnection();

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; DebugLogger.Error(ex);

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; ErrorLogger.Error(ex);

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; return

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; new ScoringProgramResponse

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; {

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; RequestCommand = command,

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; DataType = MiddleManResponseData.Error,

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; SerializedData = JsonSerializer.Serialize(ex.Message)

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; };

&nbsp;&nbsp; &nbsp; &nbsp; }

&nbsp;

&nbsp;&nbsp; &nbsp; &nbsp; finally

&nbsp;&nbsp; &nbsp; &nbsp; {

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; //Always signal that the client is free for the next items to send.

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; //Otherwise after an exception further communication will be blocked.

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; \_isSending = false;

&nbsp;&nbsp; &nbsp; &nbsp; }

&nbsp;&nbsp; }

