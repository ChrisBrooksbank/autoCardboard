using autoCardboard.Common.Domain.Interfaces;

namespace autoCardboard.Pandemic.Domain.PlayerTurns
{
    public interface IPandemicTurn : IGameTurn
    {
        int CurrentPlayerId { get; set; }
    }
}
