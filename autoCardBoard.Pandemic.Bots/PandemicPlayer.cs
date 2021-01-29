using System;
using System.Linq;
using autoCardboard.Common;
using autoCardboard.Infrastructure;
using autoCardboard.Messaging;
using autoCardboard.Pandemic.State;
using autoCardboard.Pandemic.TurnState;

namespace autoCardBoard.Pandemic.Bots
{
     public class PandemicPlayer: IPlayer<IPandemicTurn>
    {
        private readonly ICardboardLogger _log;
        private readonly IRouteHelper _routeHelper;

        private IPandemicTurn _turn;
        private int _actionsTaken;
        private int _currentPlayerId;
        private PandemicPlayerState _currentPlayerState;
        private readonly IMessageSender _messageSender;

        public int Id { get; set; }
        public string Name { get; set; }

        public PandemicPlayer(ICardboardLogger log, IRouteHelper routeHelper, IMessageSender messageSender)
        {
            _log = log;
            _routeHelper = routeHelper;
            _actionsTaken = 0;
            _messageSender = messageSender;
        }

        public void GetTurn(IPandemicTurn turn)
        {
            _log.Information($"Pandemic player {Id} taking turn");

            _actionsTaken = 0;
            _turn = turn;
            _currentPlayerId = turn.CurrentPlayerId;
            _currentPlayerState = turn.State.PlayerStates[_currentPlayerId];

            // Consider building a research station
            var hasCardForCurrentCity = _currentPlayerState.PlayerHand.Any(c => c.PlayerCardType == PlayerCardType.City
                && (City) c.Value == _currentPlayerState.Location);
            if (_actionsTaken < 4 && (_currentPlayerState.PlayerRole == PlayerRole.OperationsExpert || hasCardForCurrentCity))
            {
                var nearestCityWithResearchStation = _routeHelper.GetNearestCitywithResearchStation(turn.State.Cities, _currentPlayerState.Location);
                var distanceToNearestResearchStation = nearestCityWithResearchStation == null
                    ? 999999
                    : _routeHelper.GetDistance(turn.State.Cities, _currentPlayerState.Location,
                        nearestCityWithResearchStation.Value);

                if (distanceToNearestResearchStation > 3)
                {
                    _turn.BuildResearchStation(_currentPlayerState.Location);
                    _actionsTaken++;
                }
            }
            
            while (_actionsTaken < 4 && turn.State.Cities.Single(n => n.City ==  _currentPlayerState.Location).DiseaseCubeCount > 0)
            {
                var mapNodeToTreatDiseases = turn.State.Cities.Single(n => n.City == _currentPlayerState.Location);
                TreatDiseases(mapNodeToTreatDiseases);
            }

            var nextTurnStartsFromLocation = _currentPlayerState.Location;
            while (_actionsTaken < 4)
            {
                var moveTo = _routeHelper.GetBestCityToDriveOrFerryTo(turn.State, nextTurnStartsFromLocation);
                _messageSender.SendMessageASync($"AutoCardboard/Pandemic/Player/{_turn.CurrentPlayerId}", $"Driving to {moveTo}");

                _turn.DriveOrFerry(moveTo);
                _actionsTaken++;
                nextTurnStartsFromLocation = moveTo;
            }                

            _log.Information($"Pandemic player {Id} has taken turn");
        }

        private void TreatDiseases(MapNode mapNode)
        {
            foreach (var disease in Enum.GetValues(typeof(Disease)).Cast<Disease>())
            {
                if (mapNode.DiseaseCubes[disease] > 0 && _actionsTaken < 4)
                {
                    _messageSender.SendMessageASync($"AutoCardboard/Pandemic/Player/{_turn.CurrentPlayerId}", $"Treating {disease} at {mapNode.City}");
                    _turn.TreatDisease(disease);
                    mapNode.DiseaseCubes[disease]--; // TODO we shouldnt be doing this, see PandemicTurnHandler, call PandemicState.Methods
                    _actionsTaken++;
                }
            }
        }
    }
}
