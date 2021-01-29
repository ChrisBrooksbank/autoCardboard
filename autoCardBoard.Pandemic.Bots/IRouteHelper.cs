using System.Collections.Generic;
using autoCardboard.Pandemic.State;
using Dijkstra.NET.Graph;

namespace autoCardBoard.Pandemic.Bots
{
    public interface IRouteHelper
    {
        City GetBestCityToDriveOrFerryTo(IPandemicState state, City startingLocation);
        int GetDistance(List<MapNode> cities, City city1, City city2);
        City GetRandomNeighbour(IPandemicState state, City startingLocation);
        List<City> GetShortestPath(List<MapNode> cities, City fromCity, City toCity);
        Graph<City, string> GetCityGraph(List<MapNode> cities);
    }
}
