# AddSessionDTO

![Image](<lib/AddSessionDTO 2.png>)

The AddSessionDTO is used with the [AddSessionCommand](<Overviewofcommunication.md#OverviewOfCommands>) to [add a session to an existing event](<Addasession.md>).

##### EventGuid property

The EventGuid property specifies to which previously sent event the session should be added. Mind that for events with one session the eventguid usually is the same as that of its session.

##### Session property

Required. This specifies the session that should be added. Mind that it stronly advised to set its sections' letters to unique values as no duplicate section letters are allowed within an event. The same applies for the scoringgroupnumber of the sections. It is not possible to join the sections of an added session to the existing scoringgroups of the event that it is added to.

##### PlayerData and Participations properties

Optional. Specifies the players that will sit at the first round tables. For each player specified in a ParticipationDTO a corresponding PlayerDataDTO must be present.

##### Handrecords property

Optional. Specifies the handrecords for a given scoringgroup

##### Bridgemate3Settings and Bridgemate2Settings properties

Optional. Specifies the settings for the Bridgemates in the added session's sections.
