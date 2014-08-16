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

    public class InterfaceActivationTcp {

        public static readonly int Port = 8001;
        public static readonly string Uid = "PingPong";

        public interface IPingable { void Ping(); }
        public interface IPongable { void Pong(); }

        public class PingPong : MarshalByRefObject, IPingable, IPongable {
            public void Ping() { }
            public void Pong() { }
        }

        public class Server {

            public static void Run() {

                ChannelServices.RegisterChannel(new TcpChannel(Port), false);

                RemotingConfiguration.RegisterWellKnownServiceType(
                    type: typeof(PingPong),
                    objectUri: Uid,
                    mode: WellKnownObjectMode.Singleton
                );
            }
        }

        public class Client {

            public static void Run() {

                // activate pingable
                var pingable = (IPingable)Activator.GetObject(
                    typeof(IPingable),
                    "tcp://localhost:" + Port + "/" + Uid
                );
                pingable.Ping();

                // activate pongable
                var pongable = (IPongable)Activator.GetObject(
                    typeof(IPingable),
                    "tcp://localhost:" + Port + "/" + Uid
                );
                pongable.Pong();

                // object identity is preserved
                Debug.Assert(pingable == pongable);

                // runtime inspection is allowed
                var pingableIsPongable = pingable is IPongable;
                var pongableFromPingable = (IPongable)pingable;

                // reflection shows nothing
                var type = pingable.GetType();
                var ifaces = type.GetInterfaces();
                Debug.Assert(!ifaces.Any());
            }
        }

        public static void Test() {

            // server
            Server.Run();

            // client
            Extensions.RunInDomain(Client.Run);
        }
    }
}
