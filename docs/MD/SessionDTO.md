# SessionDTO

![Image](<lib/SessionDTO 1.png>)

&nbsp;

The SessionDTO can be used with the InitializeEvent command and will then be added to the InitDTO.Sessions property.

### EventGuid property

Optional. Must be present if there is more than one session.

##### SessionGuid property

Required. A guid uniquely defining the session. Must be exactly 32 character long, uppercase and cannot contaim dashes or curly braces.

##### Name property

Optional, but recommended.

##### Year property

Required. Must be at least 2000.

##### Month property

Required. Must be between 1 and 12

##### Day property

Requred. Must be between 1 and 31. Must match with the highest day for the month

##### Hour property

Optional. Must be between 0 and 23

##### Minute property

Optional. Must be between 0 and 59

##### ShowInApp property

Signals that the session should be uploaded to the Bridgemate App.

##### ScoringGroups property

Required. Array of ScoringGroupDTOs. At least one must be present. See [ScoringGroupDTO](<ScoringGroupDTO.md>) for details.

##### EWReturnHome property

Currently not supported.

##### PairsMoveAccrossField property

Currently not supported.

