using System;
using System.Collections.Generic;
using System.Linq;
using autoCardboard.Common;
using autoCardboard.Pandemic.State;

namespace autoCardBoard.Pandemic.Bots
{
    public class RouteHelper : IRouteHelper
    {
        private readonly IMapNodeFactory _mapNodeFactory;

        public RouteHelper(IMapNodeFactory mapNodeFactory)
        {
            _mapNodeFactory = mapNodeFactory;
        }

        public City GetBestCityToDriveOrFerryTo(IPandemicState state, City startingLocation)
        {
            var bestCandidate = 
                state.Cities.Where(c => c.DiseaseCubeCount > 1)
                    .Where(c => GetRoughDistance(startingLocation, c.City) < 3)
                    .OrderBy(c => GetRoughDistance(startingLocation, c.City)).FirstOrDefault();

            if (bestCandidate != null)
            {
                var startingNode = _mapNodeFactory.CreateMapNode(startingLocation);
                var bestMoveTo = startingNode.ConnectedCities.OrderBy(c => GetRoughDistance(bestCandidate.City, c)).First();
                return bestMoveTo;
            }

            return bestCandidate == null ? GetRandomNeighbour(state, startingLocation) : bestCandidate.City;
        }

        public City GetRandomNeighbour(IPandemicState state, City startingLocation)
        {
            var connectionCount = state.Cities.Single(n => n.City ==  startingLocation).ConnectedCities.Count();
            var moveDie = new Die(connectionCount);
            var moveDieRoll = moveDie.Roll();
            var moveTo = state.Cities.Single(n => n.City ==  startingLocation).ConnectedCities.ToArray()[moveDieRoll - 1];

            return moveTo;
        }

        public int GetRoughDistance(City city1, City city2)
        {
            var node1 = _mapNodeFactory.CreateMapNode(city1);
            var node2 = _mapNodeFactory.CreateMapNode(city2);

            var distanceRows = Math.Abs(node1.GridRow - node2.GridRow);
            var distanceColumns =  Math.Abs(node1.GridColumn - node2.GridColumn);

            // account for map wrapping
            //if (distanceRows > 7)
            //{
            //    distanceRows = 14 - distanceRows;
            //}
            
            return distanceRows + distanceColumns;
        }

        public IEnumerable<MapNode> GetCitiesWithDisease(IPandemicState state)
        {
            return state.Cities.Where(c => c.DiseaseCubeCount > 0).OrderBy(c => c.DiseaseCubeCount);
        }
    }
}
