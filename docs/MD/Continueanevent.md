# Continue an event

### Procedure

![Image](<lib/Continue.png>)

### Data structure

![Image](<lib/ContinueDTO.png>)

### Description

To continue a previously initialized event with its sessions use the [ContinueEvent](<Overviewofcommunication.md#OverviewOfCommands>) command and a [ContinueDTO](<ContinueDTO.md>). The SessionGuid must not be set in the [ScoringProgramRequest](<Overviewofcommunication.md#Diagram>). This DTO must contain a valid EventGuid.

Furthermore, the ContinueDTO contains a [Commands](<InitDTO.md#Commands>) property that specifies which actions the Bridgemate Control Software must undertake&nbsp; when it has been launched.

### Example json code

The code below shows json data that needs to be sent to continue a previously initialized event.

{

&nbsp; "Command":30,

&nbsp; "SessionGuid":"",

&nbsp; "SeriallizedData":

&nbsp; {

&nbsp; &nbsp; "EventGuid": "6D115AF1ABEB4462A299B1FE86274949",

&nbsp; &nbsp; "Commands": 5,

&nbsp; &nbsp; "AlternativeDataFolder": null

&nbsp; }

}&nbsp;

&nbsp;

&nbsp;

