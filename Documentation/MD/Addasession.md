# Add a session

### Procedure

![Image](<lib/Add session 1.png>)

### Data structure

![Image](<lib/AddSection.png>)

### Description

Using the [InitDTO](<InitDTO.md>) and the [InitializeEvent command](<Overviewofcommunication.md>) it is possible to [start Bridgemate Control Software](<Initializeanevent.md>) with more than one session. After initialization it is possible to [add a section](<Updatethemovementforasectionorad.md#AddSection>) to one of the events that were initialized.

Apart from this it is possible to add a session after initialization. This works using a [SessionDTO](<SessionDTO.md>) and the [AddSession command](<Overviewofcommunication.md>).

* Set the ScoringProgramRequest.SessionGuid property to the *EventGuid* that was used when sending the InitDTO. Mind that when the event was initialized with one session and no event guid was specified at that time, the event guid will be equal to the single session's SessionGuid property.
* Add scoringgroups, sections, tables and rounds.
* Send the ScoringProgramRequest with the [AddSection command.](<Overviewofcommunication.md>)
* Optionally proceed with sennding [playerdata](<Updateplayerdata.md>),[ participations, ](<Updateparticipations.md>)[handrecords](<Updatehandrecords.md>) and [Bridgemate settings](<UpdateBridgematesettings.md>).

