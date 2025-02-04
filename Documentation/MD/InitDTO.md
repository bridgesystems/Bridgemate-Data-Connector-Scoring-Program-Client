# InitDTO

![Image](<lib/InitDTO 1.png>)

The InitDTO is used with the [InitializeEvent](<Overviewofcommunication.md#OverviewOfCommands>) Command.

##### Commands property

The command property is the sum of actions that BCS should perform:

| Value | Description | Remarks |
| --- | --- | --- |
| &#49; | Start BCS | Instructs Bridgemate Control Software (BCS) to launch. Always include. |
| &#50; | Reset | Instructs BCS to create a new scoring file based on the provided data, clear the Bridgemate servers and upload new data to them. Always include. If the data in the Bridgemates should not be cleared then use the [ContinueDTO](<ContinueDTO.md>) with the [ContinueEvent](<Overviewofcommunication.md>) command.&nbsp; |
| &#52; | Start reading | Instructs BCS to start reading from the Bridgemates and the Bridgemate Data Connector. Optional |
| &#56; | Show in App | Instructs BCS to upload all session to the App back-end. Alternatively this can be specified on the SessionDTOs seperately |
| &#49;6 | Minimize | BCS will start minimized. |
| &#51;2 | Auto shutdown | Instructs BCS to shut down after the last result has been processed. |
| &#54;4 | Loglevel debug | Lowers the Log level from "Info" to "Debug". |
| &#49;28 | Clear data | Instructs the Bridgemate Data Connector to clear all data. This prevents stale data of the same sessions that are contained in the InitDTO from being processed. On the other hand it will also delete data from other sessions that may await further processing. Use with caution in situations where multiple events may be ongoing. Mind that all data related to the (re)initialized event will be removed in any case. |


&nbsp;

Typically the Commands property will be 1+2+4+128=135. The other values are optional.

##### EventGuid property

This is required when there is more than one session. When there is one session, its SessionGuid property will be reused as the event guid. This property will help discern scoring groups with the same scoring group number between events.

##### Sessions property

At least one is required. See the details on the [SessionDTO ](<SessionDTO.md>)for further details. The SessionDTO must contain the movement data.

##### PlayerData property

Optional. Can be sent seperately using the [PutPlayerData](<Overviewofcommunication.md>) command as well. The first name, last name of each player that could participate in the event, uniquely defined by the combination of the SessionGuid of one of the event's sessions and a PlayerNumber defined by the organization, usually the Bridge league the player is a member of.

##### Participations property

Optional. Can be sent seperately using the [PutParticipations](<Overviewofcommunication.md>) command as well.&nbsp; Starting positions for each player. The combination of SessionGuid and PlayerNumber must be present in the PlayerData.

##### Handrecords property

Optional. Can be sent seperately using the [PutHandrecords](<Overviewofcommunication.md>) command as well.

##### Bridgemate2Settings property

Optional. The settings for the Bridgemate II's. Can be sent seperately using the [PutBridgemate2Settings](<Overviewofcommunication.md#OverviewOfCommands>) command as well.

##### Bridgemate3Settings property

Optional. The settings for the Bridgemate III's.Can be sent seperately using the [PutBridgemate3Settings](<Overviewofcommunication.md#OverviewOfCommands>) command as well.

##### AlternativeDataFolder

Only used in advanced scenario's. Not documented here.

