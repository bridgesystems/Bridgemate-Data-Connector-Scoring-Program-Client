# Update handrecords

![Image](<lib/Update handrecords.png>)

### Data structure

![Image](<lib/handrecordDTOClosed.png>)

### Description

Handrecords can be updated with an array of [HandrecordDTOs](<HandrecordDTO1.md>) using the [PutHandrecords command](<Overviewofcommunication.md#OverviewOfCommands>). Existing handrecords will be updated, the rest will be added. Handrecords preferably are sent with the [InitDTO](<InitDTO.md>) while [initializing the event](<Initializeanevent.md>): this is more performant. Use the updates for movement changes or corrections after the event has started.

