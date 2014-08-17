using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RemotingExperiments {

    public static class Extensions {

        public sealed class AppDomainHost : MarshalByRefObject {

            public void Run(Action<object[]> run, params object[] arguments) {
                run(arguments);
            }

            // host is never collected by the GC even
            public override object InitializeLifetimeService() {
                return null;
            }
        }

        public static void RunInDomain(AppDomainInitializer run, 
            string basePath = null,
            string[] arguments = null) {

            AppDomain appDomain = null;

            try {
                appDomain = AppDomain.CreateDomain(
                    friendlyName: string.Empty,
                    securityInfo: null,
                    appBasePath: basePath,
                    appRelativeSearchPath: null,
                    shadowCopyFiles: false,
                    adInit: run,
                    adInitArgs: arguments
                );

            } finally {
                AppDomain.Unload(appDomain);
            }
        }
        public static void RunInDomain(Action<object[]> run, params object[] arguments) {

            AppDomain appDomain = null;

            try {
                appDomain = AppDomain.CreateDomain(friendlyName: string.Empty);
                var host = (AppDomainHost)appDomain.CreateInstanceAndUnwrap(
                    assemblyName: Assembly.GetExecutingAssembly().FullName,
                    typeName: typeof(AppDomainHost).FullName
                );

                // serialize deleage and arguments for execution in domain
                host.Run(run, arguments);

            } finally {
                AppDomain.Unload(appDomain);
            }
        }
    }
}
