using autoCardboard.Pandemic.State;

namespace autoCardBoard.Pandemic.Bots
{
    public interface IEventCardHelper
    {
        bool ShouldPlayOneQuietNight(IPandemicState state);
        bool ShouldPlayGovernmentGrant(IPandemicState state);
        bool ShouldPlayAirLift(IPandemicState state);
    }
}
