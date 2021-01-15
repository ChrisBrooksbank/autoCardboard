using autoCardboard.Common.Domain;
using autoCardboard.Common.Domain.Interfaces;
using autoCardboard.Pandemic.Domain.State;

namespace autoCardboard.Pandemic.Domain.Game
{
    public interface IPandemicGame : IGame<IPandemicGameState, PandemicTurn>
    {
    }
}
