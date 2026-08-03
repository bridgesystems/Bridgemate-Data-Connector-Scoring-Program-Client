# ParticipationDTO

![Image](<lib/participationDTO.png>)

'The ParticipationDTO specifies for each round on a table which players occupy the four seats, Only specify partiicipation for known players.

The ParticipationDTO can be used in three ways:

1. By specifiying the SessionGuid and PlayerNumber. Make sure that a corresponding [PlayerDataDTO](<PlayerDataDTO.md>) with the same SessionGuid and PlayerNumber has been sent before sending the participation.Do not include first name or last name of the player.
1. By specifying the SessionGuid and at least the PlayerLastName. Internally Bridgemate Data Connector will make a registration of this player. Do not include the player number of the player.
1. An array of ParticipationDTO will be returned by the [PollQueueForNewParticipations command](<Overviewofcommunication.md#PollingComands>). In this case the IsPlayerSwap property could have a value of "True".

&nbsp;

For a section that was created with the HasExplicitParticipations property of its [SectionDTO](<SectionDTO.md>) set to "true", the scoring program specifies the seating for every round explicitly: one ParticipationDTO per (table, round, position), with the RoundNumber property set. BCS stores these participations exactly as sent and does not calculate seatings from the movement. Use this for individual sessions and other formats where partnerships change between rounds.

For all other sections Bridgemate Control Software will determine the participations for round two and higher from the movement as sent with the [SectionDTO](<SectionDTO.md>). So leave the RoundNumber at zero, or set it to 1: a RoundNumber greater than one is a validation error for these sections, and the whole batch that contains it is rejected.

&nbsp;

You can send the participations as part of the [InitDTO](<InitDTO.md>) when i[nitializing a new event](<Initializeanevent.md>). Or they can be sent seperatly as data for the [PutParticipations command](<Overviewofcommunication.md#OverviewOfCommands>).

##### SessionGuid property

Required. A guid uniquely defining the session. Must be exactly 32 character long, uppercase and cannot contaim dashes or curly braces.

##### SectionLetters property

Required. Specifies the section for the participation.

##### TableNumber property

Required. Specifies the table for the participation.

##### RoundNumber property

Optional. The values zero and one are equivalent: they denote the table where the player sits in the first round, and BCS will calculate the player's positions in the other rounds from the movement. Values greater than one are only valid for sections that were created with HasExplicitParticipations set to "true" on their [SectionDTO](<SectionDTO.md>); for such sections BCS stores every participation for its own round exactly as sent. Sending a RoundNumber greater than one for any other section is a validation error.

##### Direction property

Required. Represents the seating for the player: North, East, South or West.

##### PlayerNumber property

Optional. Together with the SessionGuid uniquely defnines the player. The player must have been sent to Bridgemate Data Connector using a PlauerDataDTO beforehand.Do not include first name or last name.

##### FirstName property

Optional .Leave empty when a player number has been specified.

##### LastName property

Optional. Required if no player number has been specified. Leave empty when a player number has been specified.

##### CountryCode property

Currently not supported.

##### IsPlayerSwap property

Can only be "True" after a [PollQueueForNewParticipations command](<Overviewofcommunication.md#PollingComands>). Signals that the North player was swapped with the South player, or that the East player was swapped with the West player. The Direction ans PlayerNumber properties reflect the values after the swap.

&nbsp;

