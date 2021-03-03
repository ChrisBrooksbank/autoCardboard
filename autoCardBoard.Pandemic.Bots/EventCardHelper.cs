using System.Linq;
using autoCardboard.Pandemic.State;

namespace autoCardBoard.Pandemic.Bots
{
    public class EventCardHelper : IEventCardHelper
    {
        private readonly IRouteHelper _routeHelper;

        public EventCardHelper(IRouteHelper routeHelper)
        {
            _routeHelper = routeHelper;
        }

        public bool ShouldPlayAirLift(IPandemicState state)
        {
            var bestCityOnBoard = _routeHelper.GetBestLocationOnBoard(state.Cities);
            var valueOfBestCity = _routeHelper.GetLocationValue(state, bestCityOnBoard);
            var playerLocationValues = state.PlayerStates.Select( s => _routeHelper.GetLocationValue(state, s.Value.Location));
            return playerLocationValues.Any( lv => valueOfBestCity - lv > 3);
        }

        public bool ShouldPlayGovernmentGrant(IPandemicState state)
        {
            return true;
        }

        public bool ShouldPlayOneQuietNight(IPandemicState state)
        {
            var citiesWithThreeOfOneDisease = state.Cities
                .Where( c => c.DiseaseCubes[Disease.Black] == 3 ||
                             c.DiseaseCubes[Disease.Blue] == 3 ||
                             c.DiseaseCubes[Disease.Yellow] == 3 ||
                             c.DiseaseCubes[Disease.Blue] == 3 );
            return citiesWithThreeOfOneDisease.Count() > 3 && state.InfectionRateMarker > 1;
        }
    }
}
