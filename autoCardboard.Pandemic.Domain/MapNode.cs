using System;
using System.Collections.Generic;
using System.Linq;

namespace autoCardboard.Pandemic.Domain
{
    [Serializable]
    public class MapNode
    {
        public City City { get; set; }
        public IEnumerable<City> ConnectedCities { get; set; }
        public bool HasResearchStation { get; set; }
        public Dictionary<Disease,int> DiseaseCubes { get; set; }
        public int DiseaseCubeCount => DiseaseCubes.Values.Sum();
    }
}
