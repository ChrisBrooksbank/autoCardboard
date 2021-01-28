using autoCardboard.Pandemic.State;
using System.Collections.Generic;

namespace autoCardboard.Pandemic.TurnState
{
    public interface IPandemicTurnValidator
    {
        IEnumerable<string> ValidatePlayerActions(int playerId, IPandemicState state,
            IEnumerable<PlayerAction> proposedTurns, PlayerAction newProposedTurn);
    }
}
