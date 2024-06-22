# Update participations

![Image](<lib/Update participations.png>)

### Description

&nbsp;

Participations can be updated with an array of [ParticipationDTOs](<ParticipationDTO.md>) using the [PutParticipations command](<Overviewofcommunication.md#OverviewOfCommands>). Existing participations will be updated, enabling changing the player in a pair, the rest will be added. Each ParticipationDTO that is sent to the Bridgemate Data Connector with its SessionGuid and PlayerNumber propertes set must have a corresponding PlayerData that was sent before. Participations preferably are sent with the [InitDTO](<InitDTO.md>) while [initializing the event](<Initializeanevent.md>): this is more performant. Use the updates for movement changes after the event has started.

**Note**

Currently only participations for round zero or one are accepted. BCS will calculate the participations for the subsequent rounds and add or update them.

