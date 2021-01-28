using autoCardboard.Common;
using autoCardboard.Pandemic.State;
using autoCardboard.Pandemic.TurnState;

namespace autoCardboard.Pandemic.Game
{
    public interface IPandemicGame : IGame<IPandemicState, PandemicTurn>
    {
    }
}
