# Update Bridgemate settings

### Procedure

### ![Image](<lib/UpdateSettings.png>)

### Data structure

&nbsp;

![Image](<lib/Bridgemate3SettingsDTOClosed.png>)![Image](<lib/Bridgemate2SettingsDTOClosed.png>)

### Description

Bridgemate settings can be included int the [InitDTO](<InitDTO.md>) when [initializing the event](<Initializeanevent.md>), or can be updated later using the [PutBridgemate2Settings command or the PutBridgemate3Settings command](<Overviewofcommunication.md#OverviewOfCommands>). These commands use the [Bridgemate2SettingsDTO](<Bridgemate2SettingsDTO.md>) and [Bridgemate3SettingsDTO](<Bridgemate3SettingsDTO.md>) respectively. The settings must be updated for all sections in the event, even if they are the same.

### Example json code

The code below shows json data that needs to be sent to update the Bridgemate 3 settings for a section. Updating the Bridgemate 2 settings will have a similar structure, but with different setting properties.

