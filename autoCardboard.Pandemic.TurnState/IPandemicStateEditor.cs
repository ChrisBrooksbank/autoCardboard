using System.Collections.Generic;
using autoCardboard.Common;
using autoCardboard.Pandemic.State;
using autoCardboard.Pandemic.State.Delta;

namespace autoCardboard.Pandemic.TurnState
{
    public interface IPandemicStateEditor
    {
        void Clear(IPandemicState state, int pandemicCardCount = 4);
        IEnumerable<IDelta> Setup(IPandemicState state, IEnumerable<IPlayer<IPandemicTurn>> players, int pandemicCardCount = 4);
        IEnumerable<IDelta> Epidemic(IPandemicState state);
        IEnumerable<IDelta> InfectCities(IPandemicState state);
        IEnumerable<IDelta> ApplyTurn(IPandemicState state, IPandemicTurn turn);
    }
}
