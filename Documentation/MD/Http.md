# Communication over http

As an alternative to [named pipes](<NamedPipes.md>) the scoring program can communicate with the Bridgemate Data Connector over http. This is the recommended transport when the scoring program does not run on the same computer as BCS: the Data Connector runs on the BCS computer and listens on a http port (5079 by default) that is reachable from other computers on the local network.

The requests and responses are identical to those used over named pipes: a [ScoringProgramRequest](<Overviewofcommunication.md>) serialized as JSON, posted to the `dc-scoringprogram` endpoint, answered with a JSON serialized ScoringProgramResponse. Two extra properties on the request are mandatory for http communication:

| **Property** | **Description** |
| --- | --- |
| ClubId | The id of the club that is using the client |
| LicenceKey | The licence key for the club using the client |

Requests with missing or non-matching credentials are answered with an Error response with ErrorType Validation.

## Endpoints

With `{baseAddress}` being, for example, `http://192.168.1.50:5079`:

| **Endpoint** | **Method** | **Description** |
| --- | --- | --- |
| {baseAddress}/ | GET | Ping. Returns a string containing "Bridgemate dataconnector service version" when the Data Connector is reachable. |
| {baseAddress}/dc-scoringprogram | POST | The scoring program endpoint. The body is the JSON serialized ScoringProgramRequest, content type `application/json`. |

Because http is stateless there is no persistent connection: the Connect command is implemented as a ping and the Disconnect command is not implemented.

## Using the .Net Standard client

The `ScoringProgramDataConnectorHttpClient` class implements the same `IScoringProgramClient` interface as the named pipes client. Obtain it with its club credentials and, when the Data Connector runs on a different computer, the base address:

```csharp
//Data Connector on the same computer (default base address http://localhost:5079):
var client = ScoringProgramDataConnectorHttpClient.Instance(clubId, licenceKey);

//Optionally start the local Data Connector service if it is not running before pinging:
client.EnsureLocalDataConnectorIsRunning = true;

//Data Connector on a different computer on the local network:
var client = ScoringProgramDataConnectorHttpClient.Instance(clubId, licenceKey, "http://192.168.1.50:5079");

var response = await client.ConnectAsync();
```

All further usage (initializing events, sending results, polling the queues) is identical to the named pipes client.

## Firewall

When the scoring program runs on a different computer, the Windows firewall on the BCS computer must allow inbound TCP traffic to the Data Connector service on the chosen port. The BCS installer creates this rule; when running the Data Connector manually Windows will show the "allow access" dialog the first time it listens on the network.
