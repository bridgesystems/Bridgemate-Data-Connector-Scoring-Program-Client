# General

# Purpose of this document

This document describes how to use the Bridgemate Data Connector&nbsp; (the Data Connector for short) to exchange movement data, results, player data and handrecords between an external Bridge scoring program and the .Net version of the Bridgemate Control Software (BCS.Net, BCS for short). It is not a manual for how to use the Bridgemate. By reading this document and examining the the sample files, you should be able to integrate the wireless Bridgemate in your own scoring software and make full use of its functionality. We strongly advise to read the English manual of Bridgemate as well in order to have full understanding of the Bridgemate scoring system.

It still is possible to exchange data between Bridgemate and an external program by writing to and reading from a database (.bws) file. For this consult the Bridgemate developer's guide.

Mind that the Bridgemate Data Connector only works with the .Net version of BCS.

# Data exchange between Bridgemate and external programs

When using the Bridgemate Data Connector data is exchanged by sending serialized data (as json) to and reading serialized data (as json) from an intermediary process using a named pipe. This intermediary process is called the Bridgemate Data Connector. External programs can write movement data, board results, player data and handrecords to the Data Connector, where BCS will retrieve it. Likewise BCS writes board results, player data and handrecords to the Data Connector process where the external program can retrieve it. The Data Connector retains the data from one side until it has been retrieved by the other side and until the other side has signalled that it has processed the data. It will always be possible to retrieve all data again, even if it has been processed before.Apart from data exchange the external program can pass commands to BCS through the Data Connector to start up BCS, to start up BCS and create a scoring file for its own use and to shut down BCS.

# The Scoringprogram pipe client

Bridge Systems provides the source code for a client dll, the BridgeSystems.Bridgemate.DataConnector.ScoringProgramClient.dll, that handles connect, disconnect, data exchange and management commands with the Bridgemate Data Connector. This client can either be accessed directly if the progamming platform for the external program supports it, or its code can be used as examples how to implement communication with the Data Connector. The client comes with its own documentation, available at [the Bridgemate github space](<https://github.com/bridgesystems/BridgeSystems.Bridgemate.DataConnector.ScoringProgramClient> "target=\"\_blank\"").

# Data exchange format

All data is exchanged using serialization in the json format. Please refer to the examples. All programming platforms support both writing and reading of json formatted data, making it an universal data exchange format. More information can be found at [JSON](<https://www.json.org/json-en.html>)

# Differences in architecture

The table below summarizes the differences in architecture between using data exchange using the classic database based method and the newer intermediary process based approach.

| Topic | Database based approach | Data Connector based approach | Remarks |
| --- | --- | --- | --- |
| BCS data | BCS and external program both read and write to the same database (.bws) file | BCS has its own scoring file. No other program should read or write to it. | The Data Connector stores the exchange data in its own data tables |
| Consecutive actions | No new movement data, handrecords or settings updates can be sent before the previous batch has been processed by BCS as the new batch may overwrite the previous batch with instructions | The external program can send consecutive batches of instrucitons (with data). These batches will be processed by BCS in the same order. | The .bws Tables table has an UpdateFromRound field that contains instructions for updates to that table. This field should only be modified when its original value is zero. Failure to do so will lead to the loss of the previous instruction contained in this field. |
| Concurrency conflicts | Both BCS and the external program may have to deal with locked database files and concurrent writes to a table (or record).&nbsp; | All data sent is atomic: it is not visible to the other side before it has been completely sent. Conflicts are resolved by the Data Connector process.&nbsp; |  |
| New versions | Newer functions for the Bridgemates may at some time no longer be supported. | New functions will always be supported. Effort will be put in maintaining backward compatibility. | The .bws file is a legacy, MS-Access 2000 based, database format. Using the Data Connector BCS.Net has its own database format, which will support all future functions of the Bridgemates. |


&nbsp;

# 