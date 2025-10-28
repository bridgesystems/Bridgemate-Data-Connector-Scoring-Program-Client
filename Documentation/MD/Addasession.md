# Add a session

### Procedure

![Image](<lib/Add session.png>)

### Data structure

![Image](<lib/AddSessionDTO 2.png>)

### Description

Using the [InitDTO](<InitDTO.md>) and the [InitializeEvent command](<Overviewofcommunication.md>) it is possible to [start Bridgemate Control Software](<Initializeanevent.md>) with more than one session. After initialization it is possible to [add a section](<Updatethemovementforasectionorad.md#AddSection>) to one of the events that were initialized.

Apart from this it is possible to add a session after initialization. This works using an AddSessionDTO and the [AddSession command](<Overviewofcommunication.md>).

* Set the ScoringProgramRequest.SessionGuid property to the *EventGuid* that was used when sending the InitDTO. Mind that when the event was initialized with one session and no event guid was specified at that time, the event guid will be equal to the single session's SessionGuid property.
* Specify the movement data for the added session by adding a [SessionDTO](<SessionDTO.md>). Mind that that is strongly advised to set its sections' letters to unique values. Likewise, the sections must have scoringgroupnumbers that do not coincide with the existing scoringgroupnumbers of the event that they are added to.
* Send the ScoringProgramRequest with the [AddSection command.](<Overviewofcommunication.md>)
* Optionally set [playerdata and ](<Updateplayerdata.md>)[participations, ](<Updateparticipations.md>)[handrecords](<Updatehandrecords.md>) or [Bridgemate settings](<UpdateBridgematesettings.md>).

