using System.Collections.Generic;
using autoCardboard.Pandemic.State;

namespace autoCardBoard.Pandemic.Bots
{
    public interface IRouteHelper
    {
        City GetBestCityToDriveOrFerryTo(IPandemicState state, City startingLocation);
        int GetRoughDistance(City city1, City city2);
        City GetRandomNeighbour(IPandemicState state, City startingLocation);
    }
}
