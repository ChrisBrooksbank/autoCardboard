using System;
using System.Collections.Generic;
using System.Text;

namespace autoCardboard.Pandemic.State
{
    public class ConnectionsAttribute: Attribute
    {
        public IEnumerable<City> Connections { get; set; }

        public ConnectionsAttribute(params City[] connections)
        {
            Connections = connections;
        }
    }
}
