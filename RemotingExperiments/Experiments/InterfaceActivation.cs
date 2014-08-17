using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Hosting;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;

/*
 * Can a client interact with a server only through an interface so 
 * the client has no idea what type is implementing the interface on 
 * the server? If so, then the server can version the type so long
 * as the interface doesn't change. 
 * 
 * What type of reflection can the client do? Can the client test the
 * proxy for other interfaces it may implement? 
 */
namespace RemotingExperiments {

    public class InterfaceActivation {

        public interface IPingable { void Ping(); }
        public interface IPongable { void Pong(); }

        public class Server {

            // Pinger is not visible to cilent except through IPingable
            private class Pinger : MarshalByRefObject, IPingable, IPongable {
                public void Ping() { }
                public void Pong() { }
            }

            public static object Run() {
                return new Pinger();
            }
        }

        public class Client {

            public static void Run(object[] arguments) {

                // object identity is preserved
                Debug.Assert(arguments[0] == arguments[1]);

                var o = arguments[0];

                // runtime inspection is allowed
                var isPingable = o is IPingable;
                var pingable = (IPingable)o;
                pingable.Ping();

                // cross-domain marshalling allows for reflection
                var type = o.GetType();
                var ifaces = type.GetInterfaces();
                Debug.Assert(ifaces.Count() == 2);
                Debug.Assert(ifaces.Contains(typeof(IPingable)));
                Debug.Assert(ifaces.Contains(typeof(IPongable)));
            }
        }

        public static void Run() {

            var pinger = Server.Run();

            // use cross domain marshaller
            Extensions.RunInDomain(Client.Run, pinger, pinger);
        }
    }
}
