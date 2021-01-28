using System.Collections.Generic;
using autoCardboard.Pandemic.State;

namespace autoCardboard.Pandemic
{
    public interface IPandemicTurnValidator
    {
        IEnumerable<string> ValidatePlayerActions(int playerId, IPandemicState state,
            IEnumerable<PlayerAction> proposedTurns, PlayerAction newProposedTurn);
    }
}
