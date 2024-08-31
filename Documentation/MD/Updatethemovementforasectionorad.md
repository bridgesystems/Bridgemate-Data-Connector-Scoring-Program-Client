# Update the movement for a section or add one

&nbsp;

### ![Image](<lib/UpdateMovement 1.png>)

### Data structure

![Image](<lib/MovementUpdateDTOClosed.png>)

### Description

#### Update the movement for a section or add a section

When after the play of the session has started the need arises to adjust the movements, the movement for a section can be updated, or a section can be added, using a [SectionUpdateDTO](<SectionUpdateDTO.md>) and the [UpdateMovementCommand](<Overviewofcommunication.md#OverviewOfCommands>). Reasons for such an update could be a pair arriving late or playing a Swiss pairs event, The update is communicated to the Bridgemate Data Connector by sending a SectionDTO that contains the new movement data for the section. On reception of this data BCS will figure out how to update the section. Movements are expressed by adding the [tables](<TableDTO.md>) and their [rounds](<RoundDTO.md>) to the section.

The ScoringProgramResponse, if successful, contains a SectionDTO in its SerializedData property that specifies which rounds have been updated: Each round below the first specified round on a table is unchanged, each round above the last specified round will be deleted.

#### Adding a section

Adding a section works the same way. The Bridgemate Data Connector will detect that the secion did not exist before and will add it.

&nbsp;

**Note**

* Do not forget to send a Bridgemate2SettingsDTO or a Bridgemate3SettingsDTO for a new section.
* Boardresults will be deleted from the first round where the round data for a table has changed. The ScoringProgramResponse contains the rounds (as a SectionDTO) for which this is the case. If applicable send the results for these rounds again (but as a rule it should not apply).

