using Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;

namespace RemotingExperiments {

    public static class VersionedServer {

        public static readonly int Port = 8003;
        public static readonly string Uid = "Component";
        public static readonly string LocalUid = "LocalComponent";

        public class Server : IDisposable {

            private TcpChannel m_tcpChannel;

            public void Run() {
                m_tcpChannel = new TcpChannel(Port);
                ChannelServices.RegisterChannel(m_tcpChannel, false);

                var type = typeof(Component);
                var version = type.Assembly.GetName().Version;

                RemotingConfiguration.RegisterWellKnownServiceType(
                    type: type,
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
                ActivateComponent();
            }
            public static void ActivateComponent() {

                // use debugger to examine which assemblies got loaded
                AppDomain appDomain = AppDomain.CurrentDomain;
                var assemblies = appDomain.GetAssemblies();

                // use debugger to examine the location of the assemblies on disk
                var assembly = Assembly.GetExecutingAssembly();
                var assemblyLocation = Path.GetDirectoryName(assembly.Location);
                
                // get the version of the interface requested by the client
                var type = typeof(IComponent);
                var clientVersion = type.Assembly.GetName().Version.ToString();

                // activate component
                var component = (IComponent)Activator.GetObject(
                    type,
                    "tcp://localhost:" + Port + "/" + Uid
                );

                // see that the version of the client and server are different but it still works!
                var serverVersion = component.GetVersion();
                Debug.Assert(clientVersion != serverVersion);
            }
        }

        public static void Run(string clientVersion) {

            // server
            using (var server = new Server()) {
                server.Run();

                // client
                var assembly = Assembly.GetExecutingAssembly();
                var assemblyLocation = Path.GetDirectoryName(assembly.Location);
                var basePath = Path.Combine(assemblyLocation, clientVersion);

                Extensions.RunInDomain(Client.Run, basePath: basePath);
            }
        }
    }
}
