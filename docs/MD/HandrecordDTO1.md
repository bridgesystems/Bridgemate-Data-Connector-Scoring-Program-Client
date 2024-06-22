# HandrecordDTO

![Image](<lib/HandrecordDTODoubleDummy 1.png>)![Image](<lib/HandrecordDTOCards.png>)

&nbsp;

The HandrecordDTO contains the cards of each hand for a [ScoringGroupDTO](<ScoringGroupDTO.md>). Moreover it can contain double dummy data,

HandrecordDTOs can be sent as&nbsp; part of the [InitDTO](<InitDTO.md>) when i[nitializing a new event](<Initializeanevent.md>). Or they can be sent seperatly as data for the [PutHandrecordsCommand](<Overviewofcommunication.md#OverviewOfCommands>).

**Note**

The handrecords are identified using the SessionGuid and Sectionletters properties, However, handrecords must be unique for each scoringgroup. When a scoringgroup has more than one section it suffices to send the handrecords for only one of these sections.

&nbsp;

##### SessionGuid property

Required. A guid uniquely defining the session. Must be exactly 32 character long, uppercase and cannot contaim dashes or curly braces.

##### SectionLetters property

Required. Together with the SessioGuid, the ScoringGroupNumber and BoardNumber it uniquely defines the handrecord.co

##### ScorinGroupNumber property

Required. Together with the SessioGuid, SectionLetters and BoardNumber it uniquely defines the handrecord.

##### BoardNumber property

Required.Together with the SessioGuid,ScoringGroupNumber and SectionLetters it uniquely defines the handrecord.

##### NorthClubs...WestSpades properties

Required. Strings that define the cards for a hand. Valid values are A K Q J T 9 8 7 6 5 4 3 2. Leave empty for a void. Note that the ten must be represented as 'T'.

##### HasHandEvaluationData property

Optional. Specifies if double dummy analasis data is included.

#### DoubleDummyNorthClubs...DoubleDummyWestNoTrump properties

Optional. Specifies the total&nbsp; number of tricks that can be made given the declarer and denomination.

