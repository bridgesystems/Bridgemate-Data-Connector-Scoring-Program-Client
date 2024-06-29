# Update scoringgroups

![Image](<lib/Update scoringgroups 1.png>)

### Data structure

![Image](<lib/ScoringGroupUpdateDTOClosed.png>)

### Description

The configuration of the scoringgroups can be upated using a [ScoringGroupDTO](<ScoringGroupDTO.md>) and attachting the [SectionDTOs](<SectionDTO.md>) to it that should beccome part of it. Send the ScoringGroupDTOs using the [UpdateScoringGroups](<Overviewofcommunication.md#OverviewOfCommands>) command. Mind that each section must have a scoringgroup, so take care that no section is without one. Existing scoringgroups will be updated, the rest will be added.

**Note**\
When adding a section that will be part of a new scoringgroup be sure to add the scoringgroup first.

