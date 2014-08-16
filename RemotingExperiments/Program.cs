using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotingExperiments {

    public class Program {
        static void Main(string[] args) {

            Experiment.Run();
            InterfaceActivation.Test();
            InterfaceActivationTcp.Test();
        }
    }
}
