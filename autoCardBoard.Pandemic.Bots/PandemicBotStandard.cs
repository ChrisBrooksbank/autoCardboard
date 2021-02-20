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
        private readonly IMessageSender _messageSender;
        private readonly IPlayerDeckHelper _playerDeckHelper;
        private readonly IResearchStationHelper _researchStationHelper;
        private readonly IPandemicMetaState _pandemicMetaState;

        public int Id { get; set; }
        public string Name { get; set; }

        public PandemicBotStandard(ICardboardLogger log, IRouteHelper routeHelper, IMessageSender messageSender, 
            IPlayerDeckHelper playerDeckHelper, IResearchStationHelper researchStationHelper,
            IPandemicMetaState pandemicMetaState)
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
            var maxHandSize = 7;
            var toDiscardCount = _pandemicMetaState.PlayerState.PlayerHand.Count - maxHandSize;

            var cardsToDiscard = new List<PandemicPlayerCard>();
            while (cardsToDiscard.Count() < toDiscardCount)
            {
                var weakestCard = _playerDeckHelper.GetWeakCard(turn.State, _pandemicMetaState.PlayerState.PlayerRole, _pandemicMetaState.PlayerState.PlayerHand);
                cardsToDiscard.Add(weakestCard);
                _pandemicMetaState.PlayerState.PlayerHand.Remove(weakestCard);
            }

            turn.CardsToDiscard = cardsToDiscard;
        }

        public void GetActionsTurn(IPandemicTurn turn)
        {
            _turn = turn;

            // Bot needs to track its current location as it makes turn.
            var nextTurnStartsFromLocation = _pandemicMetaState.PlayerState.Location;
            _pandemicMetaState.UpdateLocation(nextTurnStartsFromLocation);
            var curableDiseases = _playerDeckHelper.GetDiseasesCanCure(_pandemicMetaState.PlayerState.PlayerRole, _pandemicMetaState.PlayerState.PlayerHand).ToList();
        
            if (_turn.ActionsTaken.Count() < 4 && _pandemicMetaState.ShouldBuildResearchStation)
            {
                _turn.BuildResearchStation(_pandemicMetaState.PlayerState.Location);
            }

            var atResearchStation = turn.State.Cities.Single(c => c.City.Equals(_pandemicMetaState.PlayerState.Location)).HasResearchStation;

            if (!atResearchStation)
            {
               while (_pandemicMetaState.NearestCityWithResearchStation != null && _turn.ActionsTaken.Count() < 4 && curableDiseases.Any() 
                       && !atResearchStation && _pandemicMetaState.RouteToNearestResearchStation != null && _pandemicMetaState.RouteToNearestResearchStation.Count > 1)
                {
                    var moveTo = _pandemicMetaState.RouteToNearestResearchStation[1];
                    _turn.DriveOrFerry(moveTo);
                    nextTurnStartsFromLocation = moveTo;
                    _pandemicMetaState.UpdateLocation(nextTurnStartsFromLocation);
                    atResearchStation = turn.State.Cities.Single(c => c.City.Equals(nextTurnStartsFromLocation)).HasResearchStation;
                }
            }

            if (_turn.ActionsTaken.Count() < 4 && curableDiseases.Any() && atResearchStation)
            {
                var disease = curableDiseases[0];
                var cureCardsToDiscard = _playerDeckHelper.GetCardsToDiscardToCure(turn.State, disease, _pandemicMetaState.PlayerState.PlayerRole, _pandemicMetaState.PlayerState.PlayerHand);
                _turn.DiscoverCure(disease, cureCardsToDiscard);
            }

            // If there is disease here, use remaining actions to treat
            while (_turn.ActionsTaken.Count() < 4 && turn.State.Cities.Single(n => n.City ==  _pandemicMetaState.PlayerState.Location).DiseaseCubeCount > 2)
            {
                var mapNodeToTreatDiseases = turn.State.Cities.Single(n => n.City == _pandemicMetaState.PlayerState.Location);
                TreatDiseases(mapNodeToTreatDiseases);
            }
            
            // Use any remaining actions, to move towards nearest city with significant disease
            while (_turn.ActionsTaken.Count() < 4)
            {
                var moveTo = _routeHelper.GetBestCityToTravelToWithoutDiscarding(turn.State, nextTurnStartsFromLocation);
                var startingNode = _turn.State.Cities.Single(n => n.City.Equals(nextTurnStartsFromLocation));
                var destinationNode = _turn.State.Cities.Single(n => n.City.Equals(moveTo));

                if (startingNode.ConnectedCities.Contains(moveTo))
                {
                    _turn.DriveOrFerry(moveTo);
                    nextTurnStartsFromLocation = moveTo;
                    _pandemicMetaState.UpdateLocation(nextTurnStartsFromLocation);
                }
                else if (startingNode.HasResearchStation && destinationNode.HasResearchStation)
                {
                    _turn.ShuttleFlight(moveTo);
                    nextTurnStartsFromLocation = moveTo;
                    _pandemicMetaState.UpdateLocation(nextTurnStartsFromLocation);
                }
                else
                {
                    throw new CardboardException("Cant make this move");
                }

            }             
        }

        private void TreatDiseases(MapNode mapNode)
        {
            foreach (var disease in Enum.GetValues(typeof(Disease)).Cast<Disease>())
            {
                if (mapNode.DiseaseCubes[disease] > 0 && _turn.ActionsTaken.Count() < 4)
                {
                    _turn.TreatDisease(disease);
                    mapNode.DiseaseCubes[disease]--;
                }
            }
        }
    }
}
