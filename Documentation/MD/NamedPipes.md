# Named Pipes

Named pipes are used to send and receive data to and from the Data Connector. This section describes how to set up communication using named pipes.

# The Named Pipe server

The server side Named Pipe is installed when installing the Bridgemate Control Software. It runs as a Windows service as soon as the computer has been started up. Its name is "BridgeSystems.Bridgemate.DataConnectorService".

Mind that the pipe only accepts one connection.

# The Named Pipe client

The Named Pipe client must be created by the scoring program and should connect to the "BridgeSystems.Bridgemate.DataConnectorService.ScoringProgram.\<username\>" Named Pipe. The \<username\> is the name of the user as Windows had defined it based on the Windows account name. It can easily be retrieved by checking the C:\\Users\\\<username\> folders.

Note that the pipe only accepts one connection. The pipe server will detect when then pipe client dies and will then accept a new connection. Alternatively the client can issue a Disconnect command to free up the pipe. A full ScoringProgramPipeClient class is provided in the BridgeSystems.Bridgemate.DataConnector.ScoringProgram.dll written by Bridge Systems. This class provides functions to connect, ping and exchange data with the pipe server. To use it the external scoring progam must be written in .Net 4.5 or higher or it must be able to use an adapter for using .Net Standard 2.0 code for its programming platform.

The source code for the &nbsp; ScoringProgramPipeClientClass can be found [at github](<https://github.com/bridgesystems/Bridgemate-Data-Connector-Scoring-Program-Client> "target=\"\_blank\""),&nbsp;

## Code examples

A typical procedure to connect to the pipe server and test communication with it would be in .Net (\<username\> arbitrarily chosen as johndoe):

&nbsp;

private NamedPipeClientStream \_pipeClient;

public StreamWriter Writer {get;set;};\
public StreamReader Reader {get;set);

&nbsp;

public async Task\<bool\> TryConnect()

{

\_pipeClient = new NamedPipeClientStream(".", "BridgeSystems.Bridgemate.DataConnectorService.ScoringProgram.*johndoe*",

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; PipeDirection.InOut, PipeOptions.None,

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; TokenImpersonationLevel.Impersonation);

bool connected;

&nbsp; &nbsp; &nbsp; &nbsp; try

&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; {

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; await \_pipeClient.ConnectAsync(TimeOutInMilliSeconds);

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; connected = true;

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; Reader = new StreamReader(\_middleManStream);

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; Writer = new StreamWriter(\_middleManStream);

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; Writer.AutoFlush = true;

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp;

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; string message="Hello world\!";

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; string serializedMessage=JsonSerializer.Serialize(message);

&nbsp;&nbsp; &nbsp; &nbsp; await Writer.WriteLineAsync(serializedMessage);

&nbsp;&nbsp; &nbsp; &nbsp; string response = await Reader.ReadLineAsync();

&nbsp;&nbsp; &nbsp; &nbsp; }

&nbsp; &nbsp; &nbsp; &nbsp; catch (TimeoutException ex)

&nbsp; &nbsp; &nbsp; {

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; connected=false;

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; }

&nbsp; &nbsp; return connected;

}

&nbsp;

In Java the code could be something like:

&nbsp;

try {\
&nbsp; &nbsp; // Connect to the pipe\
&nbsp; &nbsp; RandomAccessFile pipe =

&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; new RandomAccessFile("\\\\\\\\.\\\\pipe\\\\BridgeSystems.Bridgemate.DataConnectorService.ScoringProgram", "rw");\
&nbsp; &nbsp; String echoText = "Hello word\\n";\
&nbsp; &nbsp; // write to pipe\
&nbsp; &nbsp; pipe.write ( echoText.getBytes() );\
&nbsp; &nbsp; // read response\
&nbsp; &nbsp; String echoResponse = pipe.readLine();\
&nbsp; &nbsp; System.out.println("Response: " + echoResponse );\
&nbsp; &nbsp; pipe.close();\
&nbsp;&nbsp; &nbsp; } catch (Exception e) {\
&nbsp; &nbsp; // TODO Auto-generated catch block\
&nbsp; &nbsp; e.printStackTrace();

&nbsp;&nbsp; &nbsp; }

&nbsp;

&nbsp;

