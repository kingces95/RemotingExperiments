using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace RemotingExperiments {

    public interface IPingable {
        void Ping();
    }

    public class MyProxy : RealProxy, IRemotingTypeInfo {
        private Type m_type;
        private MarshalByRefObject m_server;

        public MyProxy(Type type)
            : base (type) {
            m_type = type;
        }

        public override IMessage Invoke(IMessage message) {
            var ctorMessage = message as IConstructionCallMessage;
            if (ctorMessage != null) {
                var result = base.InitializeServerObject(ctorMessage);
                return result;
            }

            var methodCallMessage = message as IMethodCallMessage;
            if (methodCallMessage != null) {
                var server = base.GetUnwrappedServer();
                return RemotingServices.ExecuteMessage(server, methodCallMessage);
            }

            throw new InvalidOperationException();
        }
    
        public bool CanCastTo(Type fromType, object o) {
 	        return true;
        }

        public string TypeName{
	        get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class MyProxyAttribute : ProxyAttribute {

        // Create an instance of ServicedComponentProxy
        public override MarshalByRefObject CreateInstance(Type serverType) {
            return (CustomServer)new MyProxy(typeof(CustomServer)).GetTransparentProxy();
           // return base.CreateInstance(serverType);
        }

        public override RealProxy CreateProxy(
            ObjRef objRef,
           Type serverType,
           object serverObject,
           Context serverContext) {

            var myCustomProxy = new MyProxy(serverType);

            if (serverContext != null) 
                RealProxy.SetStubData(myCustomProxy, serverContext);

            if ((!serverType.IsMarshalByRef) && (serverContext == null)) 
                throw new RemotingException("Bad Type for CreateProxy");
            
            return myCustomProxy;
        }
    }

    [MyProxyAttribute]
    public class CustomServer : ContextBoundObject {

        public CustomServer() {
            Console.WriteLine("CustomServer Base Class constructor called");
        }

        public void HelloMethod(string message) {
            Console.WriteLine("HelloMethod of Server is invoked with message : " + message);
        }

        public extern void MyExternMethod();
    }

    public static class CustomProxy {

        public static void Run() {
            var server = new CustomServer();
            server.HelloMethod("My message");

            server = (CustomServer)new MyProxy(typeof(CustomServer)).GetTransparentProxy();
            server.MyExternMethod();
            var pingable = (IPingable)server;
            pingable.Ping();
            server.HelloMethod("My message");
        }
    }
}
