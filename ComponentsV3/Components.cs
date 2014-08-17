using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Components {

    public class Component : MarshalByRefObject, IComponent {
        public string GetVersion() {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
    public interface IComponent {
        string GetVersion();
    }
}
