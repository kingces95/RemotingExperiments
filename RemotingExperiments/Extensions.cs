using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotingExperiments {

    public static class Extensions {

        public static void RunInDomain(Action run) {
            RunInDomain(o => run());
        }
        public static void RunInDomain(Action<string[]> run, params string[] arguments) {

            AppDomain appDomain = null;

            try {
                appDomain = AppDomain.CreateDomain(
                    friendlyName: null,
                    securityInfo: null,
                    appBasePath: null,
                    appRelativeSearchPath: null,
                    shadowCopyFiles: false,
                    adInit: o => run(o),
                    adInitArgs: null
                );

            } finally {
                AppDomain.Unload(appDomain);
            }
        }
    }
}
