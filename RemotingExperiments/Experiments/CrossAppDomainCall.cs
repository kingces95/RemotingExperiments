using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotingExperiments {

    public static class CrossAppDomainCall {

        public static class Server {

            public static string Run() {
                return AppDomain.CurrentDomain.FriendlyName;
            }
        }

        public static class Client {

            public static void Run(string[] args) {
                var serverAppDomain = args[0];
                var clientAppDomain = AppDomain.CurrentDomain.FriendlyName;

                Debug.Assert(serverAppDomain != clientAppDomain);
            }
        }

        public static void Run() {
            var appDomainName = Server.Run();
            Extensions.RunInDomain(Client.Run, arguments: new[] { appDomainName });
        }
    }
}
