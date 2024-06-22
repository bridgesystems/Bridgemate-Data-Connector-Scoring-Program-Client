# Delete scoringgroups

![Image](<lib/Delete scoringgroups.png>)

### Description

To delete scoringgroups send [ScoringGroupDTOs](<ScoringGroupDTO.md>) with their IsDeleted property set to "True" using a UpdateScoringGroupsCommand. Do not add any sections to the scoringgroups. A scoringgroup cannot be deleted while it has sections attached to it. [Update the scoringgroups first to assign the sections of the to be deleted scoringgroup an exisiting or new scoringgroup.](<Updatescoringgroups.md>)

