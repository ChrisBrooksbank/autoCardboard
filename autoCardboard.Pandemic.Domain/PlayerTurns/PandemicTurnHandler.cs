using autoCardboard.Common.Domain.Interfaces;
using autoCardboard.Pandemic.Domain.PlayerActionHandlers;
using autoCardboard.Pandemic.Domain.PlayerTurns;
using autoCardboard.Pandemic.Domain.State;
using System.Collections.Generic;
using System.Linq;

namespace autoCardboard.Pandemic.Domain
{
    // Responsible for modifying a PandemicGameState by applying a PandemicTurn
    public class PandemicTurnHandler : IGameTurnHandler<IPandemicGameState, IPandemicTurn>
    {
        readonly List<IPlayerActionHandler> _actionHandlers;

        private IPandemicGameState _state;
        private int _currentPlayerId;

        public PandemicTurnHandler(IEnumerable<IPlayerActionHandler> actionHandlers)
        {
            _actionHandlers = actionHandlers.ToList();
        }

        public void TakeTurn(IPandemicGameState state, IPandemicTurn turn)
        {
            _currentPlayerId = turn.CurrentPlayerId;
            _state = state;
            foreach (var action in turn.ActionsTaken)
            {
                TakeAction(action);
            }
        }

        private void TakeAction(PlayerActionWithCity action)
        {
            // TODO solid, inject IEnumerable<IPlayerActionHandler>
            switch(action.PlayerAction)
            {
                case PlayerStandardAction.TreatDisease:
                    TreatDisease(action.City, action.Disease);
                    break;
            }
        }

        private void TreatDisease(City city, Disease disease)
        {
            var diseaseState = _state.DiscoveredCures[disease];
            var currentPlayerRole = _state.PlayerStates[_currentPlayerId].PlayerRole;

            var node = _state.Cities.Single(c => c.City == city);

            if (disease == 0)
            {
                return;
            }

            if (diseaseState == DiseaseState.Cured || currentPlayerRole == PlayerRole.Medic)
            {
                _state.DiseaseCubeStock[disease] += node.DiseaseCubes[disease];
                node.DiseaseCubes[disease] = 0;
            }
            else
            {
                node.DiseaseCubes[disease]--;
                _state.DiseaseCubeStock[disease]++;
            }

          
        }

    }
}
