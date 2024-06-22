# ScoringGroupDTO

&nbsp;

![Image](<lib/ScoringGroup 1.png>)

The ScoringGroupDTO defines the scoring method for its sections and is used for the score calculation: it will group the results of all the participants of its sections together. It will be part of the a SessionDTO in an InitDTO, or it can be sent seperately using the [UpdateScoringGroups](<Overviewofcommunication.md#OverviewOfCommands>) command.

##### SessionGuid property

Required. A guid uniquely defining the session. Must be exactly 32 character long, uppercase and cannot contaim dashes or curly braces.

##### ScoringGroupNumber property

Required. An integer value uniquely defining the scoring group within the event. Must be greater than zero.

##### ScoringMethod property

Required. Values can be:

* &#49;0 for Matchpoints
* &#50;0 for Imps
* &#51;0 for Cross Imps
* &#52;0 for Team imps
* &#53;0 for Discrete Victory Points
* &#53;1 for Continuous Victory Points
* &#54;0 for Board-a-Match
* &#55;0 for Patton

##### Name property

Optional

##### Sections property

Required. An array of [SectionDTOs](<SectionDTO.md>) whose results will be calculated together as if they were one section.

##### IsDeleted property

Only used for the Update Scoringgroups command. Indicates that the scoring group has no more sections and can be deleted. Before deleting a scoring group make sure that its former sections have been assigned to a different scoring group.

