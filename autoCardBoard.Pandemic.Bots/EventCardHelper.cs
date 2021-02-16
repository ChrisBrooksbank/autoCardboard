using System.Linq;
using autoCardboard.Pandemic.State;

namespace autoCardBoard.Pandemic.Bots
{
    public class EventCardHelper : IEventCardHelper
    {
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
