using autoCardboard.Pandemic.State;

namespace autoCardBoard.Pandemic.Bots
{
    public interface IEventCardHelper
    {
        bool ShouldPlayOneQuietNight(IPandemicState state);
    }
}
