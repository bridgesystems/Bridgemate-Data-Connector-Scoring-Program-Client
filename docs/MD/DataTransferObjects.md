# Data Transfer Objects

# Introduction

All data exchange between the external scoring program and the Data Connector is done using DTOs: Data Transfer Objects. How a DTO is strucured will be dependent on the programming language used. This document will use UML to show the structure of the DTOs. A DTO must be serialized as json when sent to the Data Connector. When the Data Connector responds with data, this data will also be a json-serialized DTO.

Obviously a DTO can only have public properties and have no functions.

&nbsp;

Mind: the requests to and the responsesn from the Data Connector are layered and have two levels of json data. See the section on how to exchange data for a detailed explanation.

&nbsp;

# Example

This is a class diagram of the TableDTO, representing a table in the session's movement.

&nbsp;

![Image](<lib/TableDTO 1.png>)

&nbsp;

In C# the TableDTO class or struct would be something like:\
&nbsp;

&nbsp; &nbsp; public class TableDTO

&nbsp; &nbsp; {

&nbsp; &nbsp; &nbsp; &nbsp; public string SessionGuid

&nbsp; &nbsp; &nbsp; &nbsp; {

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; get; set;

&nbsp; &nbsp; &nbsp; &nbsp; }\
&nbsp; &nbsp; &nbsp; &nbsp;

&nbsp; &nbsp; &nbsp; &nbsp; public string SectionLetters

&nbsp; &nbsp; &nbsp; &nbsp; {

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; get; set;

&nbsp; &nbsp; &nbsp; &nbsp; }\
&nbsp;

&nbsp; &nbsp; &nbsp; &nbsp; public int TableNumber

&nbsp; &nbsp; &nbsp; &nbsp; {

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; get; set;

&nbsp; &nbsp; &nbsp; &nbsp; }\
&nbsp;

&nbsp; &nbsp; &nbsp; &nbsp; public RoundDTO\[\] Rounds

&nbsp; &nbsp; &nbsp; &nbsp; {

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; get; set;

&nbsp; &nbsp; &nbsp; &nbsp; }

&nbsp;&nbsp; &nbsp; )

&nbsp;

while in Java it would be something like:\
\
public class TableDTO {

&nbsp; &nbsp; private String sessionGuid;

&nbsp; &nbsp; private String sectionLetters;

&nbsp; &nbsp; private int tableNumber;

&nbsp; &nbsp; private RoundDTO\[\] rounds;\
&nbsp;

&nbsp; &nbsp; public String getSessionGuid() {

&nbsp; &nbsp; &nbsp; &nbsp; return sessionGuid;

&nbsp; &nbsp; }\
&nbsp;

&nbsp; &nbsp; public void setSessionGuid(String sessionGuid) {

&nbsp; &nbsp; &nbsp; &nbsp; this.sessionGuid = sessionGuid;

&nbsp; &nbsp; }\
&nbsp;

&nbsp; &nbsp; public String getSectionLetters() {

&nbsp; &nbsp; &nbsp; &nbsp; return sectionLetters;

&nbsp; &nbsp; }\
&nbsp;

&nbsp; &nbsp; public void setSectionLetters(String sectionLetters) {

&nbsp; &nbsp; &nbsp; &nbsp; this.sectionLetters = sectionLetters;

&nbsp; &nbsp; }\
&nbsp;

&nbsp; &nbsp; public int getTableNumber() {

&nbsp; &nbsp; &nbsp; &nbsp; return tableNumber;

&nbsp; &nbsp; }\
&nbsp;

&nbsp; &nbsp; public void setTableNumber(int tableNumber) {

&nbsp; &nbsp; &nbsp; &nbsp; this.tableNumber = tableNumber;

&nbsp; &nbsp; }\
&nbsp;

&nbsp; &nbsp; public RoundDTO\[\] getRounds() {

&nbsp; &nbsp; &nbsp; &nbsp; return rounds;

&nbsp; &nbsp; }\
&nbsp;

&nbsp; &nbsp; public void setRounds(RoundDTO\[\] rounds) {

&nbsp; &nbsp; &nbsp; &nbsp; this.rounds = rounds;

&nbsp; &nbsp; }

}

&nbsp;

The json serialized data for a TableDTO representing table A3 with two rounds would be something like:\
{

&nbsp; "SessionGuid": "4f24d8a2-2b6c-4a72-9e6a-8901a5a8b3c1",

&nbsp; "SectionLetters": "A",

&nbsp; "TableNumber": 3,

&nbsp; "Rounds": \[

&nbsp; &nbsp; {

&nbsp; &nbsp; &nbsp; "SessionGuid": "4f24d8a2-2b6c-4a72-9e6a-8901a5a8b3c1",

&nbsp; &nbsp; &nbsp; "SectionLetters": "A",

&nbsp; &nbsp; &nbsp; "TableNumber": 3,

&nbsp; &nbsp; &nbsp; "RoundNumber": 1,

&nbsp; &nbsp; &nbsp; "LowBoardNumber": 9,

&nbsp; &nbsp; &nbsp; "HighBoardNumber": 12,

&nbsp; &nbsp; &nbsp; "PairNS": 5,

&nbsp; &nbsp; &nbsp; "PairEW": 6,

&nbsp; &nbsp; },

&nbsp; &nbsp; {

&nbsp; &nbsp; &nbsp; "SessionGuid": "4f24d8a2-2b6c-4a72-9e6a-8901a5a8b3c1",

&nbsp; &nbsp; &nbsp; "SectionLetters": "A",

&nbsp; &nbsp; &nbsp; "TableNumber": 3,

&nbsp; &nbsp; &nbsp; "RoundNumber": 2,

&nbsp; &nbsp; &nbsp; "LowBoardNumber": 9,

&nbsp; &nbsp; &nbsp; "HighBoardNumber": 12,

&nbsp; &nbsp; &nbsp; "PairNS": 4,

&nbsp; &nbsp; &nbsp; "PairEW": 8,

&nbsp; &nbsp; }

&nbsp; \]

}

