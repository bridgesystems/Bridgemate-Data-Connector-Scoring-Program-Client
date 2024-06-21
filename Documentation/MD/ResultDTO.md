# ResultDTO

![Image](<lib/ResultDTO.png>)

Represents the result on a board. Both natural and artificial results can be expressed, It is possible to send seperate results for the North-South scoring side and East-West scoring side, but currently Bridgemate Control Software, the Bridgemate App and the Bridgemate servers do not support this.

##### SessionGuid property

Required. A guid uniquely defining the session. Must be exactly 32 character long, uppercase and cannot contaim dashes or curly braces.

##### SectionLetters property

Required. Specifies the section in whicht the result was obtained.

##### TableNumber property

Required. Specifies the table on which&nbsp; the result was obtained.

##### RoundNumber property

Required. Specifies the round in which&nbsp; the result was obtained.

##### BoardNumber property

Required. Specifies the board on which&nbsp; the result was obtained.

##### ScoringDirection property

Required (cannot be zero). 1 for North-South, 2 for East-West, 3 for both, i.e.: no split score. As split scores for North-South and East-West currently are not supported use 3 always.

##### PairNorthSouth property

The number of the pair in the North-South position.

##### PairEastWest property

The number of the pair in the East-West position.

##### DeclaringPair property

Either the value of the PairNorthSouth property or the PairEastWest property.

##### DeclarerDirection property

Specifies the declarer on the board. Possible values are:

* &#48;: NA
* &#49;: North
* &#50;: East
* &#51;: South
* &#52;: West

**Note**\
When the declarer direction property is North or South while the de declaring pair is the East-West pair the result is marked as being a switched seating. Likewise for East or West and the North-South pair.

##### Level property

Represent the level of the contract: 1 to 7 for natural scores and:

| Value | Meaning | Comment |
| --- | --- | --- |
| &#48; | Pass |  |
| \-1 | Avg minus/Avg minus | Both sides get Average minus |
| \-2 | Avg minus/ Avg | Average minus for the scoringdirection, Average for the opponents. When scoring direction is 3 it is handled as 1 (North-South) |
| \-3 | Avg minus/Avg plus | Average minus for the scoringdirection, Average plus for the opponents. When scoring direction is 3 it is handled as 1 (North-South) |
| \-4 | Avg/Avg minus | Average for the scoringdirection, Average minus for the opponents. When scoring direction is 3 it is handled as 1 (North-South) |
| \-5 | Avg/Avg | Average for both sides. |
| \-6 | Avg/Avg plus | Average for the scoringdirection, Average plus for the opponents. When scoring direction is 3 it is handled as 1 (North-South) |
| \-7 | Avg plus/Avg minus | Average plus for the scoringdirection, Average minus for the opponents. When scoring direction is 3 it is handled as 1 (North-South) |
| \-8 | Avg plus/Avg | Average plus for the scoringdirection, Average for the opponents. When scoring direction is 3 it is handled as 1 (North-South) |
| \-9 | Avg plus/Avg plus | Average plus for both sides |
| \-10 | No play | The board was cancelled. |


##### Denomination property

The denomination for the contract, if applicable. Possible values are:

* &#48;: N/A (Pass, No play, artificial score)
* &#49;: Clubs
* &#50;: Diamonds
* &#51;: Hearts
* &#52;: Spades
* &#53;: No Trump

##### Stake property

Specifies if the contract was doubled or redoubled. Possible values are:

* &#48;: Not doubled
* &#49;: Doubled
* &#50;: Redoubled

##### TotalTricks property

Specifies the total number of tricks that were obtained by the declaring side. Zero if N/A.

##### LeadCardRank property

Optional. The rank of the lead card, if specified. Possible values are:

* &#48;: N/A
* &#50;-10: the card value
* &#49;1: Jack
* &#49;2: Queen
* &#49;3: King
* &#49;4: Ace

##### LeadCardSuit property

Optional, required if the lead card rank is other than zero. The suit of the leadcard. Possible values are:

* &#48;: N/A
* &#49;: Clubs
* &#50;: Diamonds
* &#51;: Hearts
* &#52;: Spades

