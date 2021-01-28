using autoCardboard.Common;
using autoCardboard.Pandemic.State;

namespace autoCardboard.Pandemic
{
    public interface IPandemicGame : IGame<IPandemicState, PandemicTurn>
    {
    }
}
