using autoCardboard.Common;
using autoCardboard.Infrastructure;
using System;
using System.Linq;

namespace autoCardboard.Pandemic
{
    public class PandemicPlayer: IPlayer<IPandemicTurn>
    {
        private readonly ICardboardLogger _log;

        private IPandemicTurn _turn;
        private int _actionsTaken;
        private int _currentPlayerId;
        private PandemicPlayerState _currentPlayerState;
        private readonly IPandemicStateEditor _pandemicStateEditor;

        public int Id { get; set; }
        public string Name { get; set; }

        public PandemicPlayer(ICardboardLogger log, IPandemicStateEditor pandemicStateEditor)
        {
            _log = log;
            _actionsTaken = 0;
            _pandemicStateEditor = pandemicStateEditor;
        }

        public void GetTurn(IPandemicTurn turn)
        {
            _log.Information($"Pandemic player {Id} taking turn");

            _actionsTaken = 0;
            _turn = turn;
            _currentPlayerId = turn.CurrentPlayerId;
            _currentPlayerState = turn.State.PlayerStates[_currentPlayerId];

            
            while (_actionsTaken < 4 && turn.State.Cities.Single(n => n.City ==  _currentPlayerState.Location).DiseaseCubeCount > 0)
            {
                var mapNodeToTreatDiseases = turn.State.Cities.Single(n => n.City == _currentPlayerState.Location);
                TreatDiseases(mapNodeToTreatDiseases);
            }

            // TODO if I change this to 'WHEN (_actionsTaken < 4)' validation fails for some reason
            if (_actionsTaken < 4)
            {
                var connectionCount = turn.State.Cities.Single(n => n.City ==  _currentPlayerState.Location).ConnectedCities.Count();
                var moveDie = new Die(connectionCount);
                var moveDieRoll = moveDie.Roll();
                var moveTo = turn.State.Cities.Single(n => n.City ==  _currentPlayerState.Location).ConnectedCities.ToArray()[moveDieRoll - 1];

                _turn.DriveOrFerry(moveTo);
                _actionsTaken++;
             
                _pandemicStateEditor.TakePlayerAction( turn.State, new PlayerAction
                {
                    PlayerId = _currentPlayerId,
                    PlayerActionType = PlayerActionType.DriveOrFerry,
                    City = moveTo
                } );

            }                

            _log.Information($"Pandemic player {Id} has taken turn");
        }

        private void TreatDiseases(MapNode mapNode)
        {
            foreach (var disease in Enum.GetValues(typeof(Disease)).Cast<Disease>())
            {
                if (mapNode.DiseaseCubes[disease] > 0 && _actionsTaken < 4)
                {
                    _turn.TreatDisease(disease);
                    mapNode.DiseaseCubes[disease]--; // TODO we shouldnt be doing this, see PandemicTurnHandler, call PandemicState.Methods
                    _actionsTaken++;
                }
            }
        }
    }
}
