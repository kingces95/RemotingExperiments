using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotingExperiments {

    public class Program {
        static void Main(string[] args) {

            CrossAppDomainCall.Run();
            InterfaceActivationTcp.Run();
            InterfaceActivation.Run();
            VersionedServer.Run(clientVersion: "v1"); // old client
            VersionedServer.Run(clientVersion: "v3"); // new client

            CustomProxy.Run();
        }
    }
}
