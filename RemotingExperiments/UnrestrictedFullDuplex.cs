using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;

namespace RemotingExperiments {

    public class UnrestrictedFullDuplex {

        public class Server {

            public Server() {

                // server sink
                var serverSinkProvider = new BinaryServerFormatterSinkProvider();
                serverSinkProvider.TypeFilterLevel = TypeFilterLevel.Full;

                // client sink
                var clientSinkProvider = new BinaryClientFormatterSinkProvider();

                // channel properties
                var channelProperties = new Hashtable();
                channelProperties["name"] = "InterfaceActivation";
                channelProperties["port"] = 8080;

                // register chanel
                ChannelServices.RegisterChannel(
                    new TcpChannel(
                        channelProperties,
                        clientSinkProvider,
                        serverSinkProvider
                    ), false
                );
            }
        }

        public class Client {

            public Client() {
                ChannelServices.RegisterChannel(new TcpChannel(0), false);
            }
        }
    }
}
