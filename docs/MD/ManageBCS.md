# Manage BCS

![Image](<lib/Management.png>)

### Data structure

![Image](<lib/BCSManagementDTOClosed 1.png>)

### Description

&nbsp;

Using the [BCSManagementRequestDTO](<BCSManagementRequestDTO.md>) and the [ManageBCS command](<Overviewofcommunication.md#OverviewOfCommands>) it is possible to query information about BCS' data and to issue a "Shut down now" command. In the last case the ScoringProgramResponse will not contain serialized data. In the other cases the response will carry a serialized [BCSManagementResponseDTO](<BCSManagementResponseDTO.md>).

### Example json code

An exmample request asking for the scoring file location and the currently administered sessions:

{

&nbsp; "SessionGuid":null,

&nbsp; "Command":,33.

&nbsp; "SerializedData":

&nbsp; &nbsp; "{

&nbsp; &nbsp; &nbsp; &nbsp; "Command":6

&nbsp;&nbsp; &nbsp; }"

}

&nbsp;

SerilalizedData property of the scoring program response:

{

&nbsp; "EventGuid": "9B720E0FEBF741E9B3D4D9C4C71CD732",

&nbsp; "SessionInformation": \[

&nbsp; &nbsp; {

&nbsp; &nbsp; &nbsp; "EventGuid": "9B720E0FEBF741E9B3D4D9C4C71CD732",

&nbsp; &nbsp; &nbsp; "SessionGuid": "9B720E0FEBF741E9B3D4D9C4C71CD732",

&nbsp; &nbsp; &nbsp; "SessionName": "Friday evening bridge",

&nbsp; &nbsp; &nbsp; "SessionDateTime": "2022-04-05T00:00:00"

&nbsp; &nbsp; }

&nbsp; \],

&nbsp; "IsRunning": true,

&nbsp; "ScoringFilePath": "C:\\\\Users\\\\aners\\\\AppData\\\\Local\\\\BCS.Net\\\\ScoringFiles\\\\BridgeSystems.Bridgemate.BCSData.db",

}&nbsp;

&nbsp;

An example request asking for the scoring file location and all sessions known to BCS:

{

&nbsp; "SessionGuid":null,

&nbsp; "Command":,33.

&nbsp; "SerializedData":

&nbsp; &nbsp; "{

&nbsp; &nbsp; &nbsp; &nbsp; "Command":8

&nbsp;&nbsp; &nbsp; }"

}

&nbsp;

SerilalizedData property of the scoring program response:

{

&nbsp; "EventGuid": "",

&nbsp; "SessionInformation": \[

&nbsp; &nbsp; {

&nbsp; &nbsp; &nbsp; "EventGuid": "7D20A8B6F2E7431F9D65B3FDDD874040",

&nbsp; &nbsp; &nbsp; "SessionGuid": "7D20A8B6F2E7431F9D65B3FDDD874040",

&nbsp; &nbsp; &nbsp; "SessionName": "Monday bridge",

&nbsp; &nbsp; &nbsp; "SessionDateTime": "2024-06-12T00:00:00"

&nbsp; &nbsp; },

&nbsp; &nbsp; {

&nbsp; &nbsp; &nbsp; "EventGuid": "AC46A9EFF3894F2D9A5434FCC76149F7",

&nbsp; &nbsp; &nbsp; "SessionGuid": "AC46A9EFF3894F2D9A5434FCC76149F7",

&nbsp; &nbsp; &nbsp; "SessionName": "Friday bridge",

&nbsp; &nbsp; &nbsp; "SessionDateTime": "2024-06-11T00:00:00"

&nbsp; &nbsp; },

&nbsp; &nbsp; {

&nbsp; &nbsp; &nbsp; "EventGuid": "ECE27FE8E19D4C72A57C759D460F8EB8",

&nbsp; &nbsp; &nbsp; "SessionGuid": "ECE27FE8E19D4C72A57C759D460F8EB8",

&nbsp; &nbsp; &nbsp; "SessionName": "Thursday bridge",

&nbsp; &nbsp; &nbsp; "SessionDateTime": "2024-06-10T00:00:00"

&nbsp; &nbsp; },

&nbsp; &nbsp; {

&nbsp; &nbsp; &nbsp; "EventGuid": "673B2AB4BA9F45BDB758026011F34B3E",

&nbsp; &nbsp; &nbsp; "SessionGuid": "673B2AB4BA9F45BDB758026011F34B3E",

&nbsp; &nbsp; &nbsp; "SessionName": "Qualification tournament",

&nbsp; &nbsp; &nbsp; "SessionDateTime": "2022-05-24T00:00:00"

&nbsp; &nbsp; }&nbsp;

&nbsp; \],

&nbsp; "IsRunning": true,

&nbsp; "ScoringFilePath": "",

&nbsp; "ValidationMessages": null

}&nbsp;

