# TableDTO

![Image](<lib/TableDTO 1.png>)

The TableDTO represents the location where each round two pairs will meet to play boards against each other. A table automatically has a Bridgemate associated with it.

##### SessionGuid property

Required. A guid uniquely defining the session. Must be exactly 32 character long, uppercase and cannot contaim dashes or curly braces.

##### SectionLetters property

Required. Refers to the section the table is part of.

##### TableNumber property

Required. Uniquely defines the table in the section.

##### Rounds property

Required if the TableDTO is part of an InitDTO. If the TableDTO is part of a movement update, zero rounds means that the table should be deleted. An array of [RoundDTOs](<RoundDTO.md>).

