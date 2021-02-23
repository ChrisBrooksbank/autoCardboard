using autoCardboard.Pandemic.State;
using System.Collections.Generic;

namespace autoCardboard.Pandemic.TurnState
{
    public interface IPandemicActionValidator
    {
        IEnumerable<string> ValidatePlayerAction(int playerId, IPandemicState state, PlayerAction newProposedTurn);
    }
}
