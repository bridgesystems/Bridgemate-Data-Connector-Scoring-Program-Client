# RoundDTO

![Image](<lib/RoundDTO.png>)

The RoundDTO defines which pairs play on its table in the specified round and which boards they will play. Currently only consecutive boards are supported.

##### SessionGuid property

Required. A guid uniquely defining the session. Must be exactly 32 character long, uppercase and cannot contaim dashes or curly braces.

##### SectionLetters property

Required. Refers to the section the table is part of.

##### TableNumber property

Required. Uniquely defines the table in the section

##### RoundNumber property

Required. Uniquely defines the round on its table.

##### PairNS property

The number of the pair that takes the North-South position.If the value is greater than zero and if the section has one [winner](<SectionDTO.md#Winners>) the number must be unique within all rounds with the same number in the section. If the section has two winners the number must be unique within all PairNS values within all rounds with the samen number in the section. A value of zero will indicate a sit-out if the PairEW value is greater than zero or an empty table if PairEW is zero too.

##### PairEW property

The number of the pair that takes the East-West position. If the value is greater than zero and if the section has one [winner](<SectionDTO.md#Winners>) the number must be unique within all rounds with the same number in the section. If the section has two winners the number must be unique within all PairEW values within all rounds with the samen number in the section. A value of zero will indicate a sit-out if the PairNS value is greater than zero or an empty table if PairNS is zero too.

##### LowBoardNumber property

The number of the lowest board that the pairs will play. Together with the high board number property the value defines all boards that the pairs will play against each other.

##### HighBoardNumber property

The number of the highest board that the pairs will play. Together with the low board number property the value defines all boards that the pairs will play against each other.

##### TeamNS property

Optional. The number of the team for the North-South pair.

##### TeamEW property

Optional. The number of the team for the East-West pair.

##### MatesTableSectionLetters property

Optional. The letter of the section where the two other pairs of the teams match will play the same boards.

##### MatesTableTableNumber property

Optional. The number of the table where the two other pairs of the teams match will play the same boards.

##### MatesTableRoundNumber property

Optional. The number of the round where the two other pairs of the teams match will play the same boards.

##### Updated

Not in use for external scoring programs.

