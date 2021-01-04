using System.Collections.Generic;

namespace autoCardboard.Pandemic.Domain
{
    public class MapNode
    {
        public City City { get; set; }
        public IEnumerable<City> ConnectedCities { get; set; }
        public bool HasResearchStation { get; set; }
        public Dictionary<Disease,int> DiseaseCubes { get; set; }
    }
}
