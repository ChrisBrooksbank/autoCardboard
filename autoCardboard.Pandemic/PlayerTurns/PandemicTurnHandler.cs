using autoCardboard.Common;
using autoCardboard.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace autoCardboard.Pandemic
{
    // Responsible for modifying a PandemicGameState by applying a PandemicTurn
    public class PandemicTurnHandler : IGameTurnHandler<IPandemicGameState, IPandemicTurn>
    {
        private readonly ICardboardLogger _log;
        private readonly List<IPlayerActionHandler> _actionHandlers;

        private IPandemicGameState _state;
        private int _currentPlayerId;

        public PandemicTurnHandler(ICardboardLogger logger, IEnumerable<IPlayerActionHandler> actionHandlers)
        {
            _log = logger;
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

        // TODO complete and refactor somewhere more sensible
        private void OutputGameState()
        {
            //_log.Information("Game State : ");
        }


        private void TakeAction(PlayerActionWithCity action)
        {
            // TODO solid, inject IEnumerable<IPlayerActionHandler>
            switch(action.PlayerAction)
            {
                case PlayerStandardAction.TreatDisease:
                    TreatDisease(action.City, action.Disease);
                    break;
                case PlayerStandardAction.DriveOrFerry:
                    DriveOrFerry(action.City);
                    break;
            }

            OutputGameState();
        }

        // TODO move to PandemicGameState ?

        private void DriveOrFerry(City city)
        {
            var node = _state.Cities.Single(c => c.City == city);
            var playerState = _state.PlayerStates[_currentPlayerId];
            playerState.Location = city;
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
