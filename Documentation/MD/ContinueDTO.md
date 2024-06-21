# ContinueDTO

![Image](<lib/ContinueDTO.png>)

&nbsp;

The ContinueDTO is used with the [ContinueEvent](<Overviewofcommunication.md#OverviewOfCommands>) command

##### EventGuid property

The guid of the [event](<Explanationofusedterms.md#Event>) that BCS should continue administering. If the [event](<Explanationofusedterms.md#Event>) has one [session](<Explanationofusedterms.md#Session>) the session's SessionGuid property can be used if this property was also used in the [event initialization](<Initializeanevent.md>).

##### Commands property

The command property is the sum of actions that BCS should perform after start-up:

| Value | Description | Remarks |
| --- | --- | --- |
| &#49; | Start BCS | Instructs Bridgemate Control Software (BCS) to launch. Always include. |
| &#52; | Start reading | Instructs BCS to start reading from the Bridgemates and the Bridgemate Data Connector. Optional |
| &#49;6 | Minimize | BCS will start minimized. Optional |
| &#51;2 | Auto shutdown | Instructs BCS to shut down after the last result has been processed. Optional |
| &#49;28 | Clear data | Instructs the Bridgemate Data Connector to clear all data. This prevents stale data of the same sessions that are contained in the InitDTO from being processed. On the other hand it will also delete data from other sessions that may await further processing. Use with caution in situations where multiple events may be ongoing. |


&nbsp;

Typically the Commands property will be 1+4=5. The other values are optional.

###### Note:

The other command values as specified for the [InitDTO](<InitDTO.md>) are invalid and if used will lead to the [ContinueEvent](<Overviewofcommunication.md#OverviewOfCommands>) command not being executed.

##### AlternativeDataFolder

Only used in advanced scenarios. Not documented here.

