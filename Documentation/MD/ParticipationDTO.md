# ParticipationDTO

![Image](<lib/participationDTO.png>)

'The ParticipationDTO specifies for each round on a table which players occupy the four seats, Only specify partiicipation for known players.

The ParticipationDTO can be used in three ways:

1. By specifiying the SessionGuid and PlayerNumber. Make sure that a corresponding [PlayerDataDTO](<PlayerDataDTO.md>) with the same SessionGuid and PlayerNumber has been sent before sending the participation.Do not include first name or last name of the player.
1. By specifying the SessionGuid and at least the PlayerLastName. Internally Bridgemate Data Connector will make a registration of this player. Do not include the player number of the player.
1. An array of ParticipationDTO will be returned by the [PollQueueForNewParticipations command](<Overviewofcommunication.md#PollingComands>). In this case the IsPlayerSwap property could have a value of "True".

&nbsp;

In theory you could send all participations for all rounds. However, currently this is not supported.Bridgemate Control Software will determine the participations for round two and higher from the movement as sent with the [SectionDTO](<SectionDTO.md>). So leave the RoundNumber at zero, or set it to 1.

&nbsp;

You can send the participations as part of the [InitDTO](<InitDTO.md>) when i[nitializing a new event](<Initializeanevent.md>). Or they can be sent seperatly as data for the [PutParticipations command](<Overviewofcommunication.md#OverviewOfCommands>).

##### SessionGuid property

Required. A guid uniquely defining the session. Must be exactly 32 character long, uppercase and cannot contaim dashes or curly braces.

##### SectionLetters property

Required. Specifies the section for the participation.

##### TableNumber property

Required. Specifies the table for the participation.

##### RoundNumber property

Currently not supported.

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

