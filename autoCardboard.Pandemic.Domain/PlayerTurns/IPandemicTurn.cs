using autoCardboard.Common.Domain.Interfaces;
using System.Collections.Generic;

namespace autoCardboard.Pandemic.Domain.PlayerTurns
{
    public interface IPandemicTurn : IGameTurn
    {
        int CurrentPlayerId { get; set; }
        IEnumerable<PlayerActionWithCity> ActionsTaken { get; }
    }
}
