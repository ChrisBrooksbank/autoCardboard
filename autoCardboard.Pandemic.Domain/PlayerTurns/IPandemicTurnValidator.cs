using System.Collections.Generic;
using autoCardboard.Pandemic.Domain.State;

namespace autoCardboard.Pandemic.Domain.PlayerTurns
{
    public interface IPandemicTurnValidator
    {
        IEnumerable<string> ValidatePlayerTurns(int playerId, IPandemicGameState state,
            IEnumerable<PlayerActionWithCity> proposedTurns, PlayerActionWithCity newProposedTurn);
    }
}
