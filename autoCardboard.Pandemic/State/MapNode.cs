using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace autoCardboard.Pandemic
{
    [Serializable]
    public class MapNode
    {
        [JsonConverter(typeof(StringEnumConverter))] 
        public City City { get; set; }
        public IEnumerable<City> ConnectedCities { get; set; }
        public bool HasResearchStation { get; set; }
        public Dictionary<Disease,int> DiseaseCubes { get; set; }
        public int DiseaseCubeCount => DiseaseCubes.Values.Sum();
    }
}
