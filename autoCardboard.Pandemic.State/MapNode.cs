using System;
using System.Collections.Generic;
using System.Linq;

namespace autoCardboard.Pandemic.State
{
    [Serializable]
    public class MapNode
    {
        public City City { get; set; }
        public IEnumerable<City> ConnectedCities { get; set; }
        public bool HasResearchStation { get; set; }
        public Dictionary<Disease,int> DiseaseCubes { get; set; }
        public int DiseaseCubeCount => DiseaseCubes.Values.Sum();
        public int GridRow { get; set; } // top row is 0
        public int GridColumn { get; set; } // left column is 0
        public Disease DefaultDisease { get; set; }
    }
}
