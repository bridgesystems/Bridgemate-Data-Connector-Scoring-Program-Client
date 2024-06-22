# PlayerDataDTO

![Image](<lib/PlayerDataDTO.png>)

The PlayerDataDTO contains the name and identification data for a player that may compete in a specific session.

The player is uniquely defnied by the combination of SessionGuid and PlayerNumber. Both values are therefor required.

**Note**

Because of the above a player must be sent to Bridgemate Data Connector for each session that it may compete it.

&nbsp;

If players can make themselved known at their first table by entering their player number on the Bridgemate make sure that all for all possible players a PlayerDataDTO has been sent for that session.

The PlayerDataDTO can be used as part of the [InitDTO](<InitDTO.md>) for the [event initialization](<Initializeanevent.md>), or it can be sent as data for the [PutPlayerData command](<Overviewofcommunication.md#OverviewOfCommands>).

##### SessionGuid property

Required. A guid uniquely defining the session. Must be exactly 32 character long, uppercase and cannot contaim dashes or curly braces.

##### PlayerNumber property

Required. Uniquely defines the player (for this session).

##### FirstName property

Optional. The first name of the player.

##### LastName property

Required. The last name of the player.

##### CountryCode property

Currently not supported.

