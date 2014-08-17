using Components;
using System;
using System.Collections;
using System.Collections.Generic;
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

    public static class OldServer {

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

                var assembly = Assembly.GetExecutingAssembly();
                var assemblyLocation = Path.GetDirectoryName(assembly.Location);
                
                AppDomain appDomain = AppDomain.CurrentDomain;
                var assemblies = appDomain.GetAssemblies();

                var type = typeof(IComponent);
                var version = type.Assembly.GetName().Version;

                // activate pingable
                var component = (IComponent)Activator.GetObject(
                    type,
                    "tcp://localhost:" + Port + "/" + Uid
                );
                component.Ping();
            }
        }

        public static void Run() {

            // server
            using (var server = new Server()) {
                server.Run();

                // client
                var assembly = Assembly.GetExecutingAssembly();
                var assemblyLocation = Path.GetDirectoryName(assembly.Location);
                var basePath = Path.Combine(assemblyLocation, "v3");

                var appDomain = AppDomain.CreateDomain(
                    friendlyName: string.Empty,
                    securityInfo: null,
                    appBasePath: basePath,
                    appRelativeSearchPath: null,
                    shadowCopyFiles: false,
                    adInit: Client.Run,
                    adInitArgs: null
                );

                AppDomain.Unload(appDomain);
            }
        }
    }
}
