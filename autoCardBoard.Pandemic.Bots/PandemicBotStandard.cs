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

            // Bot needs to track its current location as it makes turn.
            var nextTurnStartsFromLocation = _currentPlayerState.Location;

            var curableDiseases = _playerDeckHelper.GetDiseasesCanCure(_currentPlayerState.PlayerRole, _currentPlayerState.PlayerHand).ToList();

            var shouldBuildResearchStation = _researchStationHelper.ShouldBuildResearchStation(turn.State, nextTurnStartsFromLocation, _currentPlayerState.PlayerRole, _currentPlayerState.PlayerHand);
            if (shouldBuildResearchStation)
            {
                _turn.BuildResearchStation(_currentPlayerState.Location);
                return;
            }

            var atResearchStation = turn.State.Cities.Single(c => c.City.Equals(_currentPlayerState.Location)).HasResearchStation;

            if (!atResearchStation)
            {
                var nearestCityWithResearchStation = _routeHelper.GetNearestCitywithResearchStation(turn.State, nextTurnStartsFromLocation);
                var routeToNearestResearchStation = new List<City>();
                if (nearestCityWithResearchStation != null)
                {
                    routeToNearestResearchStation = _routeHelper.GetShortestPath(turn.State, nextTurnStartsFromLocation, nearestCityWithResearchStation.Value);
                }

                while (nearestCityWithResearchStation != null  
                       && curableDiseases.Any() 
                       && routeToNearestResearchStation != null 
                       && routeToNearestResearchStation.Count > 1)
                {
                    moveTo = routeToNearestResearchStation[1];
                    _turn.DriveOrFerry(moveTo);
                    return;
                }
            }

            if (curableDiseases.Any() && atResearchStation)
            {
                var disease = curableDiseases[0];
                var cureCardsToDiscard = _playerDeckHelper.GetCardsToDiscardToCure(turn.State, disease, _currentPlayerState.PlayerRole, _currentPlayerState.PlayerHand);
                _turn.DiscoverCure(disease, cureCardsToDiscard);
                return;
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
                        mapNodeToTreatDiseases.DiseaseCubes[disease]--;
                        return;
                    }
                }
            }
            
            // Move towards nearest city with significant disease
            moveTo = _routeHelper.GetBestCityToTravelToWithoutDiscarding(turn.State, nextTurnStartsFromLocation);

            var startingNode = _turn.State.Cities.Single(n => n.City.Equals(nextTurnStartsFromLocation));
            var destinationNode = _turn.State.Cities.Single(n => n.City.Equals(moveTo));

            if (startingNode.ConnectedCities.Contains(moveTo))
            {
                _turn.DriveOrFerry(moveTo);
            }
            else if (startingNode.HasResearchStation && destinationNode.HasResearchStation)
            {
                _turn.ShuttleFlight(moveTo);
            }
            else
            {
                throw new CardboardException("Cant make this move");
            }
                 
        }
     
    }
}
