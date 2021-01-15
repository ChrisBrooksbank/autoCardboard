using autoCardboard.Common.Domain.Interfaces;
using autoCardboard.Pandemic.Domain.State;
using System.Linq;

namespace autoCardboard.Pandemic.Domain
{
    // Responsible for modifying a PandemicGameState by applying a PandemicTurn
    public class PandemicTurnHandler : IGameTurnHandler<IPandemicGameState, PandemicTurn>
    {
        private IPandemicGameState _state;

        public void TakeTurn(IPandemicGameState state, PandemicTurn turn)
        {
            _state = state;
            foreach (var action in turn.ActionsTaken)
            {
                TakeAction(action);
            }
        }

        private void TakeAction(PlayerActionWithCity action)
        {
            switch(action.PlayerAction)
            {
                case PlayerStandardAction.TreatDisease:
                    TreatDisease(action.City, action.Disease);
                    break;
            }
        }

        // TODO what if disease is cured, you then remove all
        // what if player role allows removal of all cubes
        private void TreatDisease(City city, Disease disease)
        {
            var node = _state.Cities.Single(c => c.City == city);

            if (disease == 0)
            {
                return;
            }

            node.DiseaseCubes[disease]--;
            _state.DiseaseCubeStock[disease]++;
        }

    }
}
