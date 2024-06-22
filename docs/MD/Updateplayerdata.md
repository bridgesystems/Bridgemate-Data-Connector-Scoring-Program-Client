# Update player data

![Image](<lib/Update player data.png>)

### Description

Player data can be updated with an array of [PlayerDataDTOs](<PlayerDataDTO.md>) using the [PutPlayerData command](<Overviewofcommunication.md#OverviewOfCommands>). Existing player data will be updated, enabling changing first name, last name and country code, the rest will be added. Each ParticipationDTO that is sent to the Bridgemate Data Connector with its SessionGuid and PlayerNumber propertes set must have a corresponding PlayerData that was sent before. Player data preferably is sent with the [InitDTO](<InitDTO.md>) while [initializing the event](<Initializeanevent.md>): this is more performant. Use the updates for movement changes after the event has started.

