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

        public class Server : IDisposable {

            // PingPong is not visible to client except via the interfaces
            private class PingPong : MarshalByRefObject, IPingable, IPongable {
                public void Ping() { }
                public void Pong() { }
            }

            private TcpChannel m_tcpChannel;

            public void Run() {

                m_tcpChannel = new TcpChannel(Port);
                ChannelServices.RegisterChannel(m_tcpChannel, false);

                RemotingConfiguration.RegisterWellKnownServiceType(
                    type: typeof(PingPong),
                    objectUri: Uid,
                    mode: WellKnownObjectMode.Singleton
                );
            }

            public void Dispose() {
                ChannelServices.UnregisterChannel(m_tcpChannel);
            }
        }

        public class Client {

            public static void Run(string[] args) {

                // activate pingable
                var pingable = (IPingable)Activator.GetObject(
                    typeof(IPingable),
                    "tcp://localhost:" + Port + "/" + Uid
                );
                pingable.Ping();

                // activate pongable
                var pongable = (IPongable)Activator.GetObject(
                    typeof(IPongable),
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

        public static void Run() {

            // server
            using (var server = new Server()) {
                server.Run();

                // client
                Extensions.RunInDomain(Client.Run);
            }
        }
    }
}
