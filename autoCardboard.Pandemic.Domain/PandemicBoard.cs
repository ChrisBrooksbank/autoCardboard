using System;
using System.Collections.Generic;
using System.Linq;

namespace autoCardboard.Pandemic.Domain
{
    public class PandemicBoard
    {
        private int _outbreakCount;
        public int OutbreakCount => _outbreakCount;

        public List<MapNode> Cities { get; set; }

        private Dictionary<Disease, int> _diseaseCubeStock;

        public Dictionary<Disease, int> DiseaseCubeStock => _diseaseCubeStock;

        public void Clear()
        {
            Cities = new List<MapNode>();

            var nodeFactory = new MapNodeFactory();

            var cities = Enum.GetValues(typeof(City));
            foreach (var city in cities)
            {
                Cities.Add(nodeFactory.CreateMapNode((City)city));
            }

            _diseaseCubeStock = new Dictionary<Disease, int>
            {
                {Disease.Blue, 24},
                {Disease.Black, 24},
                {Disease.Red, 24 },
                {Disease.Yellow, 24}
            };
        }

        // TODO what if disease is cured, you then remove all
        // what if player role allows removal of all cubes
        public void TreatDisease(Disease disease, City city)
        {
            var node = Cities.Single(c => c.City == city);

            var diseaseCount = node.DiseaseCubes[disease];

            if (disease == 0)
            {
                return;
            }

            node.DiseaseCubes[disease]--;
            _diseaseCubeStock[disease]++;
        }

        public void AddDiseaseCube(Disease disease, City city, List<City> ignoreCities = null)
        {
            ignoreCities = ignoreCities ?? new List<City>();

            var node = Cities.Single(n => n.City == city);

            if (node.DiseaseCubes[disease] < 3)
            {
                node.DiseaseCubes[disease] += 1;
                _diseaseCubeStock[disease]--;
                return;
            }

            _outbreakCount++;
            ignoreCities.Add(city);
            foreach (var connectedCity in node.ConnectedCities.Where(c => !ignoreCities.Contains(c)))
            {
                AddDiseaseCube(disease,connectedCity, ignoreCities);
            }
        }

        public PandemicBoard()
        {
            Clear();
        }
    }
}
