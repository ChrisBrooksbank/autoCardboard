using System;
using System.Collections.Generic;

namespace autoCardboard.Pandemic.Domain
{
    public class PandemicMap
    {
        public List<MapNode> Cities { get; set; }

        public PandemicMap()
        {
            Cities = new List<MapNode>();

            var nodeFactory = new MapNodeFactory();

            var cities = Enum.GetValues(typeof(City));
            foreach (var city in cities)
            {
                Cities.Add(nodeFactory.CreateMapNode((City)city));
            }
        }
    }
}
