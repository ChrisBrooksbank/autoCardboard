using System;
using System.Collections.Generic;
using System.Linq;
using autoCardboard.Common;
using autoCardboard.Infrastructure;
using autoCardboard.Infrastructure.Exceptions;
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
        private int _currentPlayerId;
        private PandemicPlayerState _currentPlayerState;
        private readonly IMessageSender _messageSender;
        private readonly IPlayerDeckHelper _playerDeckHelper;
        private readonly IResearchStationHelper _researchStationHelper;
        private readonly IPandemicMetaState _pandemicMetaState;

        public int Id { get; set; }
        public string Name { get; set; }

        public PandemicBotStandard(ICardboardLogger log, IRouteHelper routeHelper, IMessageSender messageSender, 
            IPlayerDeckHelper playerDeckHelper, IResearchStationHelper researchStationHelper, IPandemicMetaState pandemicMetaState)
        {
            _log = log;
            _routeHelper = routeHelper;
            _messageSender = messageSender;
            _playerDeckHelper = playerDeckHelper;
            _researchStationHelper = researchStationHelper;
            _pandemicMetaState = pandemicMetaState;
        }

        public void GetTurn(IPandemicTurn turn)
        {
            _pandemicMetaState.Load(turn);
            
            switch (turn.TurnType)
            {
                case PandemicTurnType.TakeActions:
                    GetActionsTurn(turn);
                    break;
                case PandemicTurnType.DiscardCards:
                    GetDiscardCardsTurn(turn);
                    break;
            }
        }

        public void GetDiscardCardsTurn(IPandemicTurn turn)
        {
            _currentPlayerId = turn.CurrentPlayerId;
            _currentPlayerState = turn.State.PlayerStates[_currentPlayerId];

            var maxHandSize = 7;
            var toDiscardCount = _currentPlayerState.PlayerHand.Count - maxHandSize;

            var cardsToDiscard = new List<PandemicPlayerCard>();
            while (cardsToDiscard.Count() < toDiscardCount)
            {
                var weakestCard = _playerDeckHelper.GetWeakCard(turn.State, _currentPlayerState.PlayerRole, _currentPlayerState.PlayerHand);
                cardsToDiscard.Add(weakestCard);
                _currentPlayerState.PlayerHand.Remove(weakestCard);
            }

            turn.CardsToDiscard = cardsToDiscard;
        }

        public void GetActionsTurn(IPandemicTurn turn)
        {
            City moveTo;
            _turn = turn;
            _currentPlayerId = turn.CurrentPlayerId;
            _currentPlayerState = turn.State.PlayerStates[_currentPlayerId];
           
            var curableDiseases = _playerDeckHelper.GetDiseasesCanCure(_currentPlayerState.PlayerRole, _currentPlayerState.PlayerHand).ToList();
            var atResearchStation = turn.State.Cities.Single(c => c.City.Equals(_currentPlayerState.Location)).HasResearchStation;

            // If we have a cure, and are at a research station, then cure
            if (curableDiseases.Any() && atResearchStation)
            {
                var disease = curableDiseases[0];
                var cureCardsToDiscard = _playerDeckHelper.GetCardsToDiscardToCure(
                    turn.State, disease, _currentPlayerState.PlayerRole, _currentPlayerState.PlayerHand);
                _turn.DiscoverCure(disease, cureCardsToDiscard);
                return;
            }
            
            // If we have a cure, but are not at a research station, then move towards the nearest research station
            if (!atResearchStation)
            {
                var nearestCityWithResearchStation = _routeHelper.GetNearestCitywithResearchStation(turn.State, _currentPlayerState.Location);
                var routeToNearestResearchStation = nearestCityWithResearchStation == null ? new List<City>() : _routeHelper.GetShortestPath(turn.State, _currentPlayerState.Location, nearestCityWithResearchStation.Value);
                if (nearestCityWithResearchStation != null  
                    && curableDiseases.Any() 
                    && routeToNearestResearchStation != null 
                    && routeToNearestResearchStation.Count > 1)
                {
                    _turn.DriveOrFerry(routeToNearestResearchStation[1]);
                    return;
                }
            }

            // If there is disease here then treat it
            while (turn.State.Cities.Single(n => n.City ==  _currentPlayerState.Location).DiseaseCubeCount > 2)
            {
                var mapNodeToTreatDiseases = turn.State.Cities.Single(n => n.City == _currentPlayerState.Location);
                foreach (var disease in Enum.GetValues(typeof(Disease)).Cast<Disease>())
                {
                    if (mapNodeToTreatDiseases.DiseaseCubes[disease] > 0)
                    {
                        _turn.TreatDisease(disease);
                        return;
                    }
                }
            }

            // Consider building a research station here
            var shouldBuildResearchStation = _researchStationHelper.ShouldBuildResearchStation(turn.State, _currentPlayerState.Location, _currentPlayerState.PlayerRole, _currentPlayerState.PlayerHand);
            if (shouldBuildResearchStation)
            {
                _turn.BuildResearchStation(_currentPlayerState.Location);
                return;
            }
            
            // Move towards nearest city with significant disease
            moveTo = _routeHelper.GetBestCityToTravelToWithoutDiscarding(turn.State, _currentPlayerState.Location);
            var startingNode = _turn.State.Cities.Single(n => n.City.Equals(_currentPlayerState.Location));
            var destinationNode = _turn.State.Cities.Single(n => n.City.Equals(moveTo));
            if (startingNode.ConnectedCities.Contains(moveTo))
            {
                _turn.DriveOrFerry(moveTo);
                return;
            }
            else if (startingNode.HasResearchStation && destinationNode.HasResearchStation)
            {
                _turn.ShuttleFlight(moveTo);
                return;
            }
            else
            {
                throw new CardboardException("Cant make this move");
            }
                 
        }
     
    }
}
