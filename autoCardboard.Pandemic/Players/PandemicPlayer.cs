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
        private City _currentCity;
        private MapNode _currentMapNode;

        public int Id { get; set; }
        public string Name { get; set; }

        public PandemicPlayer(ICardboardLogger log)
        {
            _log = log;
            _actionsTaken = 0;
        }

        public void GetTurn(IPandemicTurn turn)
        {
            _log.Information($"Pandemic player {Id} taking turn");

            _actionsTaken = 0;
            _turn = turn;
            _currentPlayerId = turn.CurrentPlayerId;
            _currentPlayerState = turn.State.PlayerStates[_currentPlayerId];
            _currentCity = _currentPlayerState.Location;
            _currentMapNode = turn.State.Cities.Single(n => n.City == _currentCity);

            while (_actionsTaken < 4 && _currentMapNode.DiseaseCubeCount > 0)
            {
                TreatDiseases();
            }

            if (_actionsTaken < 4)
            {
                var connectionCount = _currentMapNode.ConnectedCities.Count();
                var moveDie = new Die(connectionCount);
                var moveDieRoll = moveDie.Roll();
                var moveTo = _currentMapNode.ConnectedCities.ToArray()[moveDieRoll - 1];

                _turn.DriveOrFerry(moveTo);
                _actionsTaken++;

                // TODO, we shouldnt be doing this 
                _currentCity = moveTo;
                _currentMapNode = turn.State.Cities.Single(n => n.City == moveTo);

            }                

            _log.Information($"Pandemic player {Id} has taken turn");
        }

        private void TreatDiseases()
        {
            foreach (var disease in Enum.GetValues(typeof(Disease)).Cast<Disease>())
            {
                if (_currentMapNode.DiseaseCubes[disease] > 0 && _actionsTaken < 4)
                {
                    _turn.TreatDisease(disease);
                    _currentMapNode.DiseaseCubes[disease]--; // TODO we shouldnt be doing this, see PandemicTurnHandler, call PandemicState.Methods
                    _actionsTaken++;
                }
            }
        }
    }
}
