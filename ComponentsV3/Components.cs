using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components {

    public class Component : MarshalByRefObject, IComponent {
        public void Ping() { }
    }
    public interface IComponent {
        void Ping();
    }
}
