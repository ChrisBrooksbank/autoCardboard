﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using autoCardboard.Common;
using autoCardboard.Infrastructure.Exceptions;
using autoCardboard.Messaging;
using autoCardboard.Pandemic.State;
using autoCardboard.Pandemic.TurnState;
using Newtonsoft.Json;

namespace autoCardBoard.Pandemic.Bots
{

    public class PandemicBotStandard: IPlayer<IPandemicTurn>
    {
        private readonly IMessageSender _messageSender;
        private IPandemicTurn _currentTurn;
        private readonly MessageSenderConfiguration _messageSenderConfiguration;
        private readonly IRouteHelper _routeHelper;
        private readonly IHandManagementHelper _handManagementHelper;
        private readonly IResearchStationHelper _researchStationHelper;
        private readonly IEventCardHelper _eventCardHelper;
        private readonly IKnowledgeShareHelper _knowledgeShareHelper;
        private readonly Die _d20 = new Die(20);

        public int Id { get; set; }
        public string Name { get; set; }

        public PandemicBotStandard(IRouteHelper routeHelper, 
            IHandManagementHelper handManagementHelper, IResearchStationHelper researchStationHelper,
            IEventCardHelper eventCardHelper, IKnowledgeShareHelper knowledgeShareHelper, 
            IMessageSender messageSender, MessageSenderConfiguration messageSenderConfiguration)
        {
            _routeHelper = routeHelper;
            _handManagementHelper = handManagementHelper;
            _researchStationHelper = researchStationHelper;
            _eventCardHelper = eventCardHelper;
            _knowledgeShareHelper = knowledgeShareHelper;
            _messageSender = messageSender;
            _messageSenderConfiguration = messageSenderConfiguration;
        }

        public void GetTurn(IPandemicTurn turn)
        {
            _currentTurn = turn;

            switch (turn.TurnType)
            {
                case PandemicTurnType.TakeActions:
                    GetActionTurn(turn);
                    break;
                case PandemicTurnType.DiscardCards:
                    GetDiscardCardsTurn(turn);
                    break;
                case PandemicTurnType.PlayEventCards:
                    GetEventCardsToPlay(turn);
                    break;
            }
        }

        private void GetEventCardsToPlay(IPandemicTurn turn)
        {
            var currentPlayerId = turn.CurrentPlayerId;
            var currentPlayerState = turn.State.PlayerStates[currentPlayerId];

            // One Quiet Night
            var oneQuietNightCard = currentPlayerState.PlayerHand.SingleOrDefault(c =>
                c.PlayerCardType == PlayerCardType.Event && (EventCard) c.Value == EventCard.OneQuietNight);
            if (oneQuietNightCard != null && !_eventCardHelper.ShouldPlayOneQuietNight(turn.State))
            {
                BroadCastThought("I should play One Quiet Night");
                turn.PlayEventCard(EventCard.OneQuietNight);
            }

            // Government Grant
            var governmentGrantCard = currentPlayerState.PlayerHand.SingleOrDefault(c =>
                c.PlayerCardType == PlayerCardType.Event && (EventCard) c.Value == EventCard.GovernmentGrant);
            if (governmentGrantCard != null && _eventCardHelper.ShouldPlayGovernmentGrant(turn.State))
            {
                var locationForNewResearchStation = _routeHelper.GetBestLocationForNewResearchStation(turn.State);
                if (locationForNewResearchStation.HasValue)
                {
                    BroadCastThought($"I should play government grant in {locationForNewResearchStation}");
                    turn.PlayEventCard(EventCard.GovernmentGrant, locationForNewResearchStation.Value);
                }
            }

            // Airlift
            var airliftCard = currentPlayerState.PlayerHand.SingleOrDefault(c =>
                c.PlayerCardType == PlayerCardType.Event && (EventCard) c.Value == EventCard.Airlift);
            if (airliftCard != null && _eventCardHelper.ShouldPlayAirLift(turn.State))
            {
                var locationToAirliftTo = _routeHelper.GetBestLocationOnBoard(turn.State.Cities);
                var playerToAirlift =
                    turn.State.PlayerStates
                        .OrderBy(p => _routeHelper.GetLocationValue(turn.State, p.Value.Location))
                        .Select(ps => ps.Key).First();
                BroadCastThought($"I should play airlift on {playerToAirlift} and move them to {locationToAirliftTo}");
                turn.PlayEventCard(EventCard.Airlift, playerToAirlift, locationToAirliftTo);
            }
        }

        /// <summary>
        /// Bot is being asked to select an action, by calling relevant method on the supplied IPandemicTurn
        /// </summary>
        /// <param name="turn"></param>
        private void GetActionTurn(IPandemicTurn turn)
        {
            var currentPlayerId = turn.CurrentPlayerId;
            var currentPlayerState = turn.State.PlayerStates[currentPlayerId];
            var curableDiseases = _handManagementHelper.GetDiseasesCanCure(currentPlayerState.PlayerRole, currentPlayerState.PlayerHand).ToList();
            var atResearchStation = turn.State.Cities.Single(c => c.City.Equals(currentPlayerState.Location)).HasResearchStation;

            if (CureIfAtResearchStationAndHaveCure(turn, curableDiseases, atResearchStation, currentPlayerState)) return;
            if (MoveTowardsNearestResearchStationIfHaveCure(turn, atResearchStation, currentPlayerState, curableDiseases)) return;
            if (KnowledgeShareIfSensible(turn)) return;
            if (IfThereIsDiseaseHereThenTreatIt(turn, currentPlayerState)) return;
            if (BuildResearchStationIfSensible(turn, currentPlayerState)) return;
            if (TakeDirectFlightIfSensible(turn, currentPlayerState)) return;
            if (TakeCharterFlightIfSensible(turn, currentPlayerState)) return;
            MoveTowardsNearestDiseaseWithoutDiscarding(turn, currentPlayerState);
            return;
        }

        private bool TakeDirectFlightIfSensible(IPandemicTurn turn, PandemicPlayerState currentPlayerState)
        {
            var weakCityCards = _handManagementHelper.GetWeakCityCards(turn.State, 
                currentPlayerState.PlayerRole, currentPlayerState.PlayerHand);
            if (weakCityCards == null || !weakCityCards.Any())
            {
                return false;
            }

            if (_d20.Roll() >= 14)
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
                    BroadCastThought($"I should take a direct flight to {(City)candidateCity.Value}");
                    turn.DirectFlight((City)candidateCity.Value);
                    return true;
                }
            }

            return false;
        }

        private bool TakeCharterFlightIfSensible(IPandemicTurn turn, PandemicPlayerState currentPlayerState)
        {
            if (!_handManagementHelper.HasCityCardForCurrentLocation(currentPlayerState))
            {
                return false;
            }

            var currentCityValue = _routeHelper.GetLocationValue(turn.State,  currentPlayerState.Location);
            var bestCity = _routeHelper.GetBestLocationOnBoard(turn.State.Cities);
            var bestCityValue = _routeHelper.GetLocationValue(turn.State, bestCity);
            var distanceToBestCity = _routeHelper.GetDistance( turn.State, currentPlayerState.Location, bestCity);
            if ((bestCityValue - currentCityValue <= 3) || distanceToBestCity <= 3)
            {
                return false;
            }

            BroadCastThought($"I should take a charter flight to {bestCity}");
            turn.CharterFlight(bestCity);
            return true;

        }

        private void MoveTowardsNearestDiseaseWithoutDiscarding(IPandemicTurn turn, PandemicPlayerState currentPlayerState)
        {
            var bestCityToTravelToWithoutDiscard =
                _routeHelper.GetBestCityToTravelToWithoutDiscarding(turn.State, currentPlayerState.Location);
            var startingNode = turn.State.Cities.Single(n => n.City.Equals(currentPlayerState.Location));
            var destinationNode = turn.State.Cities.Single(n => n.City.Equals(bestCityToTravelToWithoutDiscard));
            if (startingNode.ConnectedCities.Contains(bestCityToTravelToWithoutDiscard))
            {
                BroadCastThought($"I should drive/ferry to {bestCityToTravelToWithoutDiscard} so I can move towards disease");
                turn.DriveOrFerry(bestCityToTravelToWithoutDiscard);
                return;
            }
            else if (startingNode.HasResearchStation && destinationNode.HasResearchStation)
            {
                BroadCastThought($"I should take a shuttle flight to {destinationNode.City} so I can move towards disease");
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
                BroadCastThought($"I should build a research station at {currentPlayerState.Location}");
                turn.BuildResearchStation(currentPlayerState.Location);
                return true;
            }

            return false;
        }

        private bool KnowledgeShareIfSensible(IPandemicTurn turn)
        {
            if (_knowledgeShareHelper.CanKnowledgeShare(turn.CurrentPlayerId, turn.State))
            {
                var suggestedKnowledgeShare = _knowledgeShareHelper.GetSuggestedKnowledgeShare(turn.CurrentPlayerId, turn.State);
                if (suggestedKnowledgeShare != null)
                {
                    BroadCastThought($"I should knowledge share");
                    turn.KnowledgeShare( suggestedKnowledgeShare);
                    return true;
                }
            }

            return false;
        }

        private bool IfThereIsDiseaseHereThenTreatIt(IPandemicTurn turn, PandemicPlayerState currentPlayerState)
        {
            while (turn.State.Cities.Single(n => n.City == currentPlayerState.Location).DiseaseCubeCount > 2)
            {
                var mapNodeToTreatDiseases = turn.State.Cities.Single(n => n.City == currentPlayerState.Location);
                foreach (var disease in Enum.GetValues(typeof(Disease)).Cast<Disease>())
                {
                    if (mapNodeToTreatDiseases.DiseaseCubes[disease] > 0)
                    {
                        BroadCastThought($"I should treat the disease {disease} in my city of {currentPlayerState.Location}");
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
                    BroadCastThought($"I should move torwards the research station at {nearestCityWithResearchStation} so I can cure a disease");
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
                var cureCardsToDiscard = _handManagementHelper.GetCardsToDiscardToCure(
                    turn.State, disease, currentPlayerState.PlayerRole, currentPlayerState.PlayerHand);
                turn.DiscoverCure(disease, cureCardsToDiscard);
                BroadCastThought($"I should cure {disease}");
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
                var weakestCard = _handManagementHelper.GetWeakCard(turn.State, currentPlayerState.PlayerRole, currentPlayerState.PlayerHand);
                if (weakestCard == null)
                {
                    weakestCard = currentPlayerState.PlayerHand[0];
                }
                cardsToDiscard.Add(weakestCard);
                currentPlayerState.PlayerHand.Remove(weakestCard);
            }

            turn.CardsToDiscard = cardsToDiscard;
        }

        private void BroadCastThought(string thought)
        {
            var topic = ExpandPlaceHolders(_messageSenderConfiguration.TopicBotThought);
            var payload = JsonConvert.SerializeObject(new {  bot = _currentTurn.CurrentPlayerId, thought });
            Task.WaitAll(_messageSender.SendMessageASync(topic, payload));
        }

        private string ExpandPlaceHolders(string payLoad)
        {
            return string.IsNullOrEmpty(payLoad) ? payLoad : payLoad.Replace("{gameId}",_currentTurn?.State?.Id);
        }
     
    }
}
