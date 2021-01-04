using System;
using System.Collections.Generic;

namespace autoCardboard.Pandemic.Domain
{
    public class PandemicMap
    {
        public List<MapNode> Nodes { get; set; }

        public PandemicMap()
        {
            Nodes = new List<MapNode>();

            var nodeFactory = new MapNodeFactory();

            var cities = Enum.GetValues(typeof(City));
            foreach (var city in cities)
            {
                Nodes.Add(nodeFactory.CreateMapNode((City)city));
            }
        }
    }
}
