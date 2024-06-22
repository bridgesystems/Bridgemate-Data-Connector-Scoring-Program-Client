# BCSManagementRequestDTO

![Image](<lib/BCSManamgementRequestDTO.png>)

The BCSManagementDTO can be used to send a shut down request to Bridgemate Control Software. It can also be used to query some data about what events are known and which of them currently is being administered. Last it is possible to ask for the location of the scoring file in order to include this in back-ups.

Depending on the given command the request will return different data.The BCSManagemenRequetDTO is sent with a [ManageBCS command](<Overviewofcommunication.md#OverviewOfCommands>)

&nbsp;

Below are the commands that can be given and their effects:

| **Command** | **Effect** | **Returns** |
| --- | --- | --- |
| &#49;: ShutDownNow | Sends a request to BCS to shut&nbsp; down immediately, Mind that this can cause previous data that was sent not to be processed. | A ScoringProgramResponse with its DataType set to OK or an error code. No serialized data is returned. |
| &#50;: GetRunningSessions | Queries which event is currently being administered. | A ScoringProgramResponse with as DataType EventInfo or an errorcode. The SerializedData property contains a [BCSManagementResponseDTO](<BCSManagementResponseDTO.md>). |
| &#52;: GetScoringFileLocation | Queries which scoring file is currently in use and where it is located. | A ScoringProgramResponse with as DataType ScoringFileLocation or an errorcode. The SerializedData property contains a [BCSManagementResponseDTO](<BCSManagementResponseDTO.md>). |
| &#56;: GetAllSessionsInformation | Queries which events are present in the current scoring file. | A ScoringProgramResponse with as DataType AllSessionsInfo or an errorcode. The SerializedData property contains a [BCSManagementResponseDTO](<BCSManagementResponseDTO.md>). |
| &#54;:&nbsp; 2+4 | Combines the two commands | A ScoringProgramResponse with as DataType EventInfo or an errorcode. The SerializedData property contains a[ BCSManagementResponseDTO](<BCSManagementResponseDTO.md>). |


&nbsp;

