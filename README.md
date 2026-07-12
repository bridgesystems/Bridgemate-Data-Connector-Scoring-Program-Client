# BridgeSystems Bridgemate DataConnector ScoringProgramClient
Welcome to the source code for the scoring program client to the Bridgemate Data Connector.
Communication through the Bridgemate Data Connector is the preferred way for communication between bridge scoring programs and the Bridgemate 3, the Bridgemate 2 and the Bridgemate App back end. Bridgemate Control Software 5 is needed to receive, process and return data from the Data Connector.
In this repository you will find the documentation on how to write a client yourself as well as the source code for a scoring program client, written in C# for .Net Standard 2.0, provided by Bridge Systems BV. This client takes care of connecting, reconnecting and communication with the Data Connector. You can either use the source code as a reference to write your own code, or you can interface with the compiled BridgeSystems.Bridgemate.DataConnector.ScoringProgram.dll.

# Clients for other platforms
The scoring program client is also available for PHP and Java. Both speak a wire format generated from this repository and verified against golden fixtures, so the three clients behave identically:
- **PHP**: [Bridgemate-Data-Connector-Scoring-Program-Client-PHP](https://github.com/BridgeSystems/Bridgemate-Data-Connector-Scoring-Program-Client-PHP) — Composer package `bridgemate/dataconnector-client`
- **Java**: [Bridgemate-Data-Connector-Scoring-Program-Client-Java](https://github.com/BridgeSystems/Bridgemate-Data-Connector-Scoring-Program-Client-Java) — Maven coordinates `nl.bridgemate:bridgemate-dataconnector-client`

Questions about any client are welcome in the [Discussions](https://github.com/BridgeSystems/Bridgemate-Data-Connector-Scoring-Program-Client/discussions) of this repository; see [SUPPORT.md](SUPPORT.md).

# The getting-started console sample
[samples/GettingStarted](samples/GettingStarted/) is a small console application that exercises the core workflow over http against a live Data Connector: connect/ping, initialize or continue an event, send player data and results, poll and accept the queues. Run it interactively or as an unattended scenario:
```
dotnet run --project samples/GettingStarted
dotnet run --project samples/GettingStarted -- --scenario
```
Mind that "Initialize event" starts Bridgemate Control Software and creates a small test event. Visual Studio Code launch configurations are included (`.vscode/launch.json`); the project is deliberately not part of the solution, so the NuGet package build is unaffected.

# The scoring program emulator (BridgeSystems.Bridgemate.DataConnectorClientEmulator)
The [emulator folder](emulator/) contains the full WPF scoring program emulator that demonstrates how to use the ScoringProgramClient: it initializes events, sends players and results and polls the queues through the same client library this repository publishes. The emulator builds against the client source of the same commit (a project reference), so it always matches the released package — including the http transport.

- **Run it without building:** download the ready-to-run emulator from the [Releases page](../../releases); every package release includes a matching emulator build.
- **Open the source:** `emulator/BridgeSystems.Bridgemate.DataConnectorClientEmulator` (Visual Studio or `dotnet build`; requires the .NET 10 SDK on Windows).

Be free to use and adapt this code to learn and test how to communicate with the Bridgemate Data Connector. Mind that the emulator has its own licence: see [emulator/EULA.txt](emulator/EULA.txt) — unlike the client library it may not be redistributed.

## The compiled libraries
The client is available as a NuGet package on [nuget.org](https://www.nuget.org/packages/BridgeSystems.Bridgemate.DataConnector.ScoringProgramClient). To install:
```
dotnet add package BridgeSystems.Bridgemate.DataConnector.ScoringProgramClient
```
A symbol package (.snupkg) is included for debugging support.

Alternatively, the compiled libraries can be found in the Dll folder. Make sure to copy all *.dll files. The .pdb file can be useful when debugging. The .deps file may make it possible that the dlls other than the main one do not need to be copied. This may be the case when the programming environment can parse it and knows where to find the dependencies.

### Transports
The client supports two transports, both implementing the same `IScoringProgramClient` interface:
- **Named pipes** (`ScoringProgramDataConnectorPipeClient`) — the default; requires the scoring program to run on the same computer as BCS.
- **Http** (`ScoringProgramDataConnectorHttpClient`) — for scoring programs on the same computer or on another computer on the local network. Pass the club credentials and, for a remote Data Connector, its base address:
```csharp
var client = ScoringProgramDataConnectorHttpClient.Instance(clubId, licenceKey, "http://192.168.1.50:5079");
```
When no base address is set the client targets the Data Connector on the local computer, discovering its port through the registry (default 5079). See [Http](Documentation/MD/Http.md) for details.

### Logging
The library uses `Microsoft.Extensions.Logging.Abstractions` so you can plug in any logging framework (NLog, Serilog, etc.). To enable logging, set the logger factory before using the client:
```csharp
DataConnectorLogging.LoggerFactory = yourLoggerFactory;
```
If not set, logging is silently disabled.

## Documentation
There are two main sources of documentation:
### 1. The general description of how to implement communication with the Bridgemate Data Connector.
Help documents that describe en detail how to connect to and communicate with the Data Connector, enabling creation of your own client on the programming platform of your choice:
1. Pdf: [Bridgemate Data Connector developer's guide](https://github.com/bridgesystems/BridgeSystems.Bridgemate.DataConnector.ScoringProgramClient/blob/master/Documentation/Doc/Bridgemate%20Data%20Connector%20developers%20guide.pdf)
2. Markdown: [Bridgemate Data Connector developer's guide](https://github.com/bridgesystems/Bridgemate-Data-Connector-Scoring-Program-Client/blob/master/Documentation/MD/index.md)

### 2. The context sensitive help for the source code of the BridgeSystems.Bridgemate.DataConnector.ScoringProgram.dll.
A help file that describes the methods and properties of the scoring program client provided by Bridge Systems BV.
1. Html: [Data Connector context sensitive help ](https://bridgesystems.github.io/Bridgemate-Data-Connector-Scoring-Program-Client/html/b11ca58b-c149-48f8-af9a-cf6a2c7bfe53.htm)

As of March 10th 2026, this repository has been moved from "bridgesystemsbv" to "BridgeSystems"