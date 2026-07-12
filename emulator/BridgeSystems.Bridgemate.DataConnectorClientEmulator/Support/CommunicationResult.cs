using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BridgeSystems.Bridgemate.DataConnector.ScoringProgramClient;

namespace BridgeSystems.Bridgemate.DataConnectorClientEmulator.Support;
public struct CommunicationResult
{
    public ErrorType ErrorType { get; set; }
    public string RequestDescription { get; set; }
    public string ResponseMessage { get; set; }
}
