using System.Collections.Generic;
using System.Linq;
using autoCardboard.Pandemic.State;

namespace autoCardBoard.Pandemic.Bots
{
    public class ResearchStationHelper : IResearchStationHelper
    {
        private readonly IRouteHelper _routeHelper;

        public ResearchStationHelper(IRouteHelper routeHelper)
        {
            _routeHelper = routeHelper;
        }

        public bool CouldBuildResearchStation(IPandemicState state, City currentLocation, PlayerRole playerRole, List<PandemicPlayerCard> playerHand)
        {
           if (state.ResearchStationStock < 1)
           {
               return false;
           }

           var atResearchStation = state.Cities.Single(c => c.City.Equals(currentLocation)).HasResearchStation;
           if (atResearchStation)
           {
               return false;
           }

           if (playerRole == PlayerRole.OperationsExpert)
           {
               return true;
           }

           if (playerHand.Any(c => c.PlayerCardType == PlayerCardType.City && (City)c.Value == currentLocation))
           {
               return true;
           }

           return false;
        }

        public bool ShouldBuildResearchStation(IPandemicState state, City currentLocation, PlayerRole playerRole, List<PandemicPlayerCard> playerHand)
        {
           if (!CouldBuildResearchStation(state, currentLocation, playerRole, playerHand))
           {
               return false;
           }

           var nearestCityWithResearchStation = _routeHelper.GetNearestCitywithResearchStation(state, currentLocation);
           if (nearestCityWithResearchStation == null)
           {
               return true;
           }

           var minimumDistanceBetweenResearchStations = 2; // could be dynamic

           var routeToNearestResearchStation = _routeHelper.GetShortestPath(state , currentLocation, nearestCityWithResearchStation.Value);
           if (routeToNearestResearchStation.Count > minimumDistanceBetweenResearchStations)
           {
               return true;
           }

           return false;
        }
    }
}
