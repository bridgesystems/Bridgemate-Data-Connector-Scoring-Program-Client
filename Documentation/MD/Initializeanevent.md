# Initialize an event

### Procedure

![Image](<lib/Initialize.png>)

### Data structure

![Image](<lib/InitDTOPlus.png>)

&nbsp;

### Description

An event can be initialized with one or more sessions.Initialization is done using the [InitializeEvent](<Overviewofcommunication.md>) command and an [InitDTO](<InitDTO.md>). The SessionGuid must not be set in the [ScoringProgramRequest](<Overviewofcommunication.md#Diagram>). This DTO must at least contain information on the scoring method, the scoring group for each section and the movement.

Player data, starting posititons and handrecords are optional. Board results are no part of initialization data but can be sent later.N

Furthermore, the InitDTO contains a [Commands](<InitDTO.md#Commands>) property that specifies which actions the Bridgemate Control Software must undertake&nbsp; when it has been launched.

**Note**

A player, as identified by his playernumber, may only exist once in the event. In the case that a player participates in both events make sure to duplicate the player with a different playernumber.

### Example json code

The code below shows the minimum required json data to send to initialize a session with four pairs that all meet each other.

{

&nbsp; "Command": 5,

&nbsp; "SessionGuid: """,

&nbsp; "SerializedData":

&nbsp; {

&nbsp; &nbsp; "Commands": 135,

&nbsp; &nbsp; "EventGuid": "6D115AF1ABEB4462A299B1FE86274949",

&nbsp; &nbsp; "AlternativeDataFolder": null,

&nbsp; &nbsp; "Sessions": \[

&nbsp; &nbsp; {

&nbsp; &nbsp; &nbsp; "ScoringGroups": \[

&nbsp; &nbsp; &nbsp; &nbsp; {

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "SessionGuid": "6D115AF1ABEB4462A299B1FE86274949",

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "Sections": \[

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; {

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "SessionGuid": "6D115AF1ABEB4462A299B1FE86274949",

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "ScoringGroupNumber": 1,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "Letters": "A",

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "Name": null,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "Winners": 1,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "GameType": 10,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "EWMoveBeforePlay": 0,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "MissingPair": 0,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "IsCombiSection": false,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "NorthSouthPairSectionLetters": null,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "EastWestPairSectionLetters": null,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "IsDeleted": false,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "Tables": \[

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; {

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "SessionGuid": "6D115AF1ABEB4462A299B1FE86274949",

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "SectionLetters": "A",

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "TableNumber": 1,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "Rounds": \[

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; {

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "SessionGuid": "6D115AF1ABEB4462A299B1FE86274949",

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "SectionLetters": "A",

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "TableNumber": 1,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "RoundNumber": 1,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "LowBoardNumber": 1,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "HighBoardNumber": 4,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "PairNS": 1,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "PairEW": 2,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "TeamNS": 0,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "TeamEW": 0,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "MatesTableSectionLetters": null,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "MatesTableTableNumber": 0,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "MatesTableRoundNumber": 0,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "Updated": false

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; },

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; {

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "SessionGuid": "6D115AF1ABEB4462A299B1FE86274949",

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "SectionLetters": "A",

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "TableNumber": 1,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "RoundNumber": 2,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "LowBoardNumber": 5,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "HighBoardNumber": 8,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "PairNS": 1,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "PairEW": 3,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "TeamNS": 0,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "TeamEW": 0,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "MatesTableSectionLetters": null,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "MatesTableTableNumber": 0,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "MatesTableRoundNumber": 0,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "Updated": false

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; },

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; {

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "SessionGuid": "6D115AF1ABEB4462A299B1FE86274949",

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "SectionLetters": "A",

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "TableNumber": 1,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "RoundNumber": 3,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "LowBoardNumber": 9,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "HighBoardNumber": 12,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "PairNS": 1,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "PairEW": 4,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "TeamNS": 0,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "TeamEW": 0,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "MatesTableSectionLetters": null,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "MatesTableTableNumber": 0,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "MatesTableRoundNumber": 0,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "Updated": false

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; }

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; \]

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; },

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; {

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "SessionGuid": "6D115AF1ABEB4462A299B1FE86274949",

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "SectionLetters": "A",

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "TableNumber": 2,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "Rounds": \[

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; {

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "SessionGuid": "6D115AF1ABEB4462A299B1FE86274949",

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "SectionLetters": "A",

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "TableNumber": 2,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "RoundNumber": 1,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "LowBoardNumber": 1,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "HighBoardNumber": 4,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "PairNS": 3,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "PairEW": 4,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "TeamNS": 0,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "TeamEW": 0,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "MatesTableSectionLetters": null,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "MatesTableTableNumber": 0,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "MatesTableRoundNumber": 0,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "Updated": false

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; },

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; {

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "SessionGuid": "6D115AF1ABEB4462A299B1FE86274949",

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "SectionLetters": "A",

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "TableNumber": 2,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "RoundNumber": 2,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "LowBoardNumber": 5,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "HighBoardNumber": 8,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "PairNS": 4,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "PairEW": 2,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "TeamNS": 0,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "TeamEW": 0,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "MatesTableSectionLetters": null,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "MatesTableTableNumber": 0,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "MatesTableRoundNumber": 0,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "Updated": false

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; },

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; {

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "SessionGuid": "6D115AF1ABEB4462A299B1FE86274949",

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "SectionLetters": "A",

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "TableNumber": 2,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "RoundNumber": 3,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "LowBoardNumber": 9,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "HighBoardNumber": 12,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "PairNS": 2,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "PairEW": 3,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "TeamNS": 0,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "TeamEW": 0,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "MatesTableSectionLetters": null,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "MatesTableTableNumber": 0,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "MatesTableRoundNumber": 0,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "Updated": false

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; }

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; \]

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; }

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; \]

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; }

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; \],

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "ScoringGroupNumber": 1,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "ScoringMethod": 10,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "Name": null,

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; "IsDeleted": false

&nbsp; &nbsp; &nbsp; &nbsp; }

&nbsp; &nbsp; &nbsp; \],

&nbsp; &nbsp; &nbsp; "EventGuid": "6D115AF1ABEB4462A299B1FE86274949",

&nbsp; &nbsp; &nbsp; "SessionGuid": "6D115AF1ABEB4462A299B1FE86274949",

&nbsp; &nbsp; &nbsp; "Name": "Minimal Session",

&nbsp; &nbsp; &nbsp; "Year": 2024,

&nbsp; &nbsp; &nbsp; "Month": 6,

&nbsp; &nbsp; &nbsp; "Day": 3,

&nbsp; &nbsp; &nbsp; "Hour": 0,

&nbsp; &nbsp; &nbsp; "Minute": 0,

&nbsp; &nbsp; &nbsp; "ShowInApp": true,

&nbsp; &nbsp; &nbsp; "EWReturnHome": false,

&nbsp; &nbsp; &nbsp; "PairsMoveAccrossField": false

&nbsp; &nbsp; }

&nbsp; \],

&nbsp; &nbsp; "PlayerData": null,

&nbsp; &nbsp; "Participations": null,

&nbsp; &nbsp; "Handrecords": null,

&nbsp; &nbsp; "Bridgemate2Settings": null,

&nbsp; &nbsp; "Bridgemate3Settings": null

&nbsp; }&nbsp;

}

&nbsp;

&nbsp;

