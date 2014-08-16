using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotingExperiments {

    public static class Experiment {

        public sealed class Target : MarshalByRefObject {
        }

        public static class Server {

            public static object Run() {
                return new Target();
            }
        }

        public static class Client {

            public static void Run(object argument) {
            }
        }

        public static void Run() {
            var target = Server.Run();
            Extensions.RunInDomain(Client.Run, target);
        }
    }
}
