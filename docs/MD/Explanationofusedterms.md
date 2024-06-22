# Explanation of used terms

* BCS or Bridgemate Control Software: Windows software program to communicate with the Bridgemate base station, which communicates with the wireless Bridgemate terminals. Movement data is uploaded by this program to the base station, and results are retrieved from it the other way around.Without further specification the term "BCS" refers tot the newer, .Net based, version as the original, classic, version does not support use of the Bridgemate Data Connector.
* The (external) scoring program: Windows software program that is used for scoring bridge sessions, calculating results, printing rankings etc and which uses Bridgemates for input of results.
* The Data Connector:The Bridgemate Data Connector process: a messaging system to send requests and commands from the external scoring program to BCS and to receive data from BCS for the external scoring program.&nbsp;
* DTO: Data Transfer Object. A class or structure holding data for BCS, to be sent to it using the Data Connector. A DTO must be serialized as json in a request to the Data Connector and must be deserialzed from json when returned from the Data Connector
* The communication channel: the method used to exchange data with the Bridgemate Data Connector. The data will always be serialized as JSON. The transfer method currently is by using NamedPipes only.
* Event: one or more sessions that are grouped together in the Bridgemate servers.
* Session: a separately scored group of sections.
* Sit-out: a round on a table where a pair has no opponent.
* Empty table: a round on a table without pairs.
* Participation: the position of a player on a table in a given round, identified by his playernumber.

