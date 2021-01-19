using System.Collections.Generic;

namespace autoCardboard.Pandemic
{
    public interface IPandemicTurnValidator
    {
        IEnumerable<string> ValidatePlayerTurns(int playerId, IPandemicState state,
            IEnumerable<PlayerActionWithCity> proposedTurns, PlayerActionWithCity newProposedTurn);
    }
}
