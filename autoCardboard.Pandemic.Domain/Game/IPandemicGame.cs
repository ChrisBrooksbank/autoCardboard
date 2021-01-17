using autoCardboard.Common;

namespace autoCardboard.Pandemic.Domain
{
    public interface IPandemicGame : IGame<IPandemicGameState, PandemicTurn>
    {
    }
}
