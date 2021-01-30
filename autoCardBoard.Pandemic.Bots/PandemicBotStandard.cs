using System;
using System.Collections.Generic;
using System.Linq;
using autoCardboard.Common;
using autoCardboard.Infrastructure;
using autoCardboard.Messaging;
using autoCardboard.Pandemic.State;
using autoCardboard.Pandemic.TurnState;

namespace autoCardBoard.Pandemic.Bots
{
     public class PandemicBotStandard: IPlayer<IPandemicTurn>
    {
        private readonly ICardboardLogger _log;
        private readonly IRouteHelper _routeHelper;

        private IPandemicTurn _turn;
        private int _actionsTaken;
        private int _currentPlayerId;
        private PandemicPlayerState _currentPlayerState;
        private readonly IMessageSender _messageSender;
        private readonly IPlayerDeckHelper _playerDeckHelper;

        public int Id { get; set; }
        public string Name { get; set; }

        public PandemicBotStandard(ICardboardLogger log, IRouteHelper routeHelper, IMessageSender messageSender, IPlayerDeckHelper playerDeckHelper)
        {
            _log = log;
            _routeHelper = routeHelper;
            _actionsTaken = 0;
            _messageSender = messageSender;
            _playerDeckHelper = playerDeckHelper;
        }

        public void GetTurn(IPandemicTurn turn)
        {
            _log.Information($"Pandemic player {Id} taking turn");

            _actionsTaken = 0;
            _turn = turn;
            _currentPlayerId = turn.CurrentPlayerId;
            _currentPlayerState = turn.State.PlayerStates[_currentPlayerId];
            var hasCardForCurrentCity = _currentPlayerState.PlayerHand.Any(c => c.PlayerCardType == PlayerCardType.City
                && (City) c.Value == _currentPlayerState.Location);

            var cardsByCity = _playerDeckHelper.GetCityCardsByColour(_currentPlayerState.PlayerHand);
            var readyToCure = cardsByCity[Disease.Blue].Count == 5 
                                  || cardsByCity[Disease.Red].Count == 5 
                                  || cardsByCity[Disease.Yellow].Count == 5
                                  || cardsByCity[Disease.Black].Count == 5;
            var atResearchStation = turn.State.Cities.Single(c => c.City.Equals(_currentPlayerState.Location)).HasResearchStation;
            var nearestCityWithResearchStation = _routeHelper.GetNearestCitywithResearchStation(turn.State.Cities, _currentPlayerState.Location);
            var routeToNearestResearchStation = new List<City>();
            if (nearestCityWithResearchStation != null)
            {
                routeToNearestResearchStation = _routeHelper.GetShortestPath(turn.State.Cities , _currentPlayerState.Location, nearestCityWithResearchStation.Value);
            }

            // TODO this should loop towards research station until actions used up
            if (_actionsTaken < 4 && readyToCure && !atResearchStation)
            {
                var moveTo = routeToNearestResearchStation[1];
                _messageSender.SendMessageASync($"AutoCardboard/Pandemic/Player/{_turn.CurrentPlayerId}", $"Driving to {moveTo}");
                _turn.DriveOrFerry(moveTo);
                _actionsTaken++;
            }

            if (_actionsTaken < 4 && readyToCure && atResearchStation)
            {
                // TODO cure disease
            }

            // If we can build a research station here, and we are not too close to another one, then build one
            if (_actionsTaken < 4 && (_currentPlayerState.PlayerRole == PlayerRole.OperationsExpert || hasCardForCurrentCity))
            {
                if (routeToNearestResearchStation.Count > 2)
                {
                    _turn.BuildResearchStation(_currentPlayerState.Location);
                    _actionsTaken++;
                }
            }
            
            // If there is disease here, use remaining actions to cure it
            while (_actionsTaken < 4 && turn.State.Cities.Single(n => n.City ==  _currentPlayerState.Location).DiseaseCubeCount > 2)
            {
                var mapNodeToTreatDiseases = turn.State.Cities.Single(n => n.City == _currentPlayerState.Location);
                TreatDiseases(mapNodeToTreatDiseases);
            }

            // Use any remaining actions, to move towards nearest city with significant disease
            var nextTurnStartsFromLocation = _currentPlayerState.Location;
            while (_actionsTaken < 4)
            {
                var moveTo = _routeHelper.GetBestCityToDriveOrFerryTo(turn.State, nextTurnStartsFromLocation);
                _messageSender.SendMessageASync($"AutoCardboard/Pandemic/Player/{_turn.CurrentPlayerId}", $"Driving to {moveTo}");

                _turn.DriveOrFerry(moveTo);
                _actionsTaken++;
                nextTurnStartsFromLocation = moveTo;
            }             

            // TODO we want control over which cards to discard, probably a new PlayerAction type,
            // e.g. if we have five red cards, dont discard a red card !!!

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
