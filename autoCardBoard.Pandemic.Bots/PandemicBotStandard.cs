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
        
        private readonly IMessageSender _messageSender;
        private readonly IHandManagementHelper _playerDeckHelper;
        private readonly IResearchStationHelper _researchStationHelper;

        public int Id { get; set; }
        public string Name { get; set; }

        public PandemicBotStandard(ICardboardLogger log, IRouteHelper routeHelper, IMessageSender messageSender, 
            IHandManagementHelper playerDeckHelper, IResearchStationHelper researchStationHelper)
        {
            _log = log;
            _routeHelper = routeHelper;
            _messageSender = messageSender;
            _playerDeckHelper = playerDeckHelper;
            _researchStationHelper = researchStationHelper;
        }

        public void GetTurn(IPandemicTurn turn)
        {
            switch (turn.TurnType)
            {
                case PandemicTurnType.TakeActions:
                    GetActionTurn(turn);
                    break;
                case PandemicTurnType.DiscardCards:
                    GetDiscardCardsTurn(turn);
                    break;
            }
        }

        /// <summary>
        /// Bot is being asked to select an action, by calling relevant method on the supplied IPandemicTurn
        /// </summary>
        /// <param name="turn"></param>
        public void GetActionTurn(IPandemicTurn turn)
        {
            var currentPlayerId = turn.CurrentPlayerId;
            var currentPlayerState = turn.State.PlayerStates[currentPlayerId];
            var curableDiseases = _playerDeckHelper.GetDiseasesCanCure(currentPlayerState.PlayerRole, currentPlayerState.PlayerHand).ToList();
            var atResearchStation = turn.State.Cities.Single(c => c.City.Equals(currentPlayerState.Location)).HasResearchStation;

            if (CureIfAtResearchStationAndHaveCure(turn, curableDiseases, atResearchStation, currentPlayerState)) return;
            if (MoveTowardsNearestResearchStationIfHaveCure(turn, atResearchStation, currentPlayerState, curableDiseases)) return;
            if (IfThereIsDiseaseHereThenTreatIt(turn, currentPlayerState)) return;
            if (BuildResearchStationIfSensible(turn, currentPlayerState)) return;
            if (TakeDirectFlightIfSensible(turn, currentPlayerState)) return;
            MoveTowardsNearestDiseaseWithoutDiscarding(turn, currentPlayerState);
            return;
        }

        private bool TakeDirectFlightIfSensible(IPandemicTurn turn, PandemicPlayerState currentPlayerState)
        {
            var weakCityCards = _playerDeckHelper.GetWeakCityCards(turn.State, 
                currentPlayerState.PlayerRole, currentPlayerState.PlayerHand);
            if (weakCityCards == null || !weakCityCards.Any())
            {
                return false;
            }

            var valueOfCurrentLocation = _routeHelper.GetLocationValue(turn.State, currentPlayerState.Location);

            foreach (var candidateCity in weakCityCards)
            {
                var valueOfCandidateCity =  _routeHelper.GetLocationValue(turn.State, (City)candidateCity.Value);
                var distanceToCandidateCity = _routeHelper.GetDistance(turn.State, currentPlayerState.Location, (City)candidateCity.Value);

                if ( (valueOfCandidateCity - valueOfCurrentLocation > 3) && distanceToCandidateCity > 3)
                {
                    turn.DirectFlight((City)candidateCity.Value);
                    return true;
                }
            }

            return false;
        }

        private void MoveTowardsNearestDiseaseWithoutDiscarding(IPandemicTurn turn, PandemicPlayerState currentPlayerState)
        {
            var bestCityToTravelToWithoutDiscard =
                _routeHelper.GetBestCityToTravelToWithoutDiscarding(turn.State, currentPlayerState.Location);
            var startingNode = turn.State.Cities.Single(n => n.City.Equals(currentPlayerState.Location));
            var destinationNode = turn.State.Cities.Single(n => n.City.Equals(bestCityToTravelToWithoutDiscard));
            if (startingNode.ConnectedCities.Contains(bestCityToTravelToWithoutDiscard))
            {
                turn.DriveOrFerry(bestCityToTravelToWithoutDiscard);
                return;
            }
            else if (startingNode.HasResearchStation && destinationNode.HasResearchStation)
            {
                turn.ShuttleFlight(bestCityToTravelToWithoutDiscard);
                return;
            }
            else
            {
                throw new CardboardException("Cant make this move");
            }
        }

        private bool BuildResearchStationIfSensible(IPandemicTurn turn, PandemicPlayerState currentPlayerState)
        {
            var shouldBuildResearchStation = _researchStationHelper.ShouldBuildResearchStation(
                turn.State, currentPlayerState.Location, currentPlayerState.PlayerRole, currentPlayerState.PlayerHand);
            if (shouldBuildResearchStation)
            {
                turn.BuildResearchStation(currentPlayerState.Location);
                return true;
            }

            return false;
        }

        private static bool IfThereIsDiseaseHereThenTreatIt(IPandemicTurn turn, PandemicPlayerState currentPlayerState)
        {
            while (turn.State.Cities.Single(n => n.City == currentPlayerState.Location).DiseaseCubeCount > 2)
            {
                var mapNodeToTreatDiseases = turn.State.Cities.Single(n => n.City == currentPlayerState.Location);
                foreach (var disease in Enum.GetValues(typeof(Disease)).Cast<Disease>())
                {
                    if (mapNodeToTreatDiseases.DiseaseCubes[disease] > 0)
                    {
                        turn.TreatDisease(disease);
                        return true;
                    }
                }
            }

            return false;
        }

        private bool MoveTowardsNearestResearchStationIfHaveCure(IPandemicTurn turn, bool atResearchStation,
            PandemicPlayerState currentPlayerState, List<Disease> curableDiseases)
        {
            if (!atResearchStation)
            {
                var nearestCityWithResearchStation =
                    _routeHelper.GetNearestCitywithResearchStation(turn.State, currentPlayerState.Location);
                var routeToNearestResearchStation = nearestCityWithResearchStation == null
                    ? new List<City>()
                    : _routeHelper.GetShortestPath(turn.State, currentPlayerState.Location,
                        nearestCityWithResearchStation.Value);
                if (nearestCityWithResearchStation != null
                    && curableDiseases.Any()
                    && routeToNearestResearchStation != null
                    && routeToNearestResearchStation.Count > 1)
                {
                    turn.DriveOrFerry(routeToNearestResearchStation[1]);
                    return true;
                }
            }

            return false;
        }

        private bool CureIfAtResearchStationAndHaveCure(IPandemicTurn turn, List<Disease> curableDiseases, bool atResearchStation,
            PandemicPlayerState currentPlayerState)
        {
            if (curableDiseases.Any() && atResearchStation)
            {
                var disease = curableDiseases[0];
                var cureCardsToDiscard = _playerDeckHelper.GetCardsToDiscardToCure(
                    turn.State, disease, currentPlayerState.PlayerRole, currentPlayerState.PlayerHand);
                turn.DiscoverCure(disease, cureCardsToDiscard);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Bot is being asked to discard cards down to hand limit
        /// </summary>
        /// <param name="turn"></param>
        public void GetDiscardCardsTurn(IPandemicTurn turn)
        {
            var currentPlayerId = turn.CurrentPlayerId;
            var currentPlayerState = turn.State.PlayerStates[currentPlayerId];

            var maxHandSize = 7;
            var toDiscardCount = currentPlayerState.PlayerHand.Count - maxHandSize;

            var cardsToDiscard = new List<PandemicPlayerCard>();
            while (cardsToDiscard.Count() < toDiscardCount)
            {
                var weakestCard = _playerDeckHelper.GetWeakCard(turn.State, currentPlayerState.PlayerRole, currentPlayerState.PlayerHand);
                if (weakestCard == null)
                {
                    weakestCard = currentPlayerState.PlayerHand[0];
                }
                cardsToDiscard.Add(weakestCard);
                currentPlayerState.PlayerHand.Remove(weakestCard);
            }

            turn.CardsToDiscard = cardsToDiscard;
        }
     
    }
}
