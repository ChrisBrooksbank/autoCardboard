using System.Collections.Generic;
using autoCardboard.Pandemic.State;
using Dijkstra.NET.Graph;
using autoCardboard.Pandemic.TurnState;

namespace autoCardBoard.Pandemic.Bots
{
    public interface IRouteHelper
    {
        City GetBestCityToTravelToWithoutDiscarding(IPandemicState state, City startingLocation);
        int GetDistance(IPandemicState state, City city1, City city2);
        City GetRandomNeighbour(IPandemicState state, City startingLocation);
        City? GetNearestCitywithResearchStation(IPandemicState state, City city);
        List<City> GetShortestPath(IPandemicState state, City fromCity, City toCity);
        Graph<City, string> GetCityGraph(IPandemicState state);
        int GetLocationValue(IPandemicState state, City city);
    }
}
