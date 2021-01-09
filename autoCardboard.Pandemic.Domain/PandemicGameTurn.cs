﻿using autoCardboard.Common.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace autoCardboard.Pandemic.Domain
{
    [Serializable]
    // TODO add operations for available play actions
    // each player has four actions

    // TODO move validation logic to a validator interface/class for sharing with Game type
    public class PandemicGameTurn : IGameTurn
    {
        // _state is a clone of the game state 
        // game state can only be permanently changed by game, not by players, otherwise they could cheat
        private PandemicGameState _state;

        private List<PlayerActionWithCity> _playerActions = new List<PlayerActionWithCity>();

        public int CurrentPlayerId { get; set; }

        public IEnumerable<PlayerActionWithCity> ActionsTaken => _playerActions;

        public PandemicGameState State
        {
            get
            {
                return _state;
            }

            set
            {
                // Clone the game state, so if a inmemory player modifies it, this doesnt modify the actual game state
                _state = value.Clone() as PandemicGameState;
            }
        }

        public void DriveOrFerry(City toConnectedCity)
        {
            ValidateAvailableAction();

            var playerState = State.PlayerStates[CurrentPlayerId];
            var currentMapLocation = State.Cities.Single(n => n.City == playerState.Location);

            if (!currentMapLocation.ConnectedCities.Any(c => c == toConnectedCity))
            {
                throw new ApplicationException("Cant driver or ferry here because its not connected to current location");
            }

            // this state change wont last beyond takinbg turn, but prevents turn validation failing on next action
            _playerActions.Add(new PlayerActionWithCity { PlayerAction = PlayerStandardAction.DriveOrFerry, City = toConnectedCity });
        }

        public void DirectFlight(City discardCityCardOfDestination)
        {
            ValidateAvailableAction();

            var playerState = State.PlayerStates[CurrentPlayerId];
            if (!playerState.PlayerHand.Exists(c => c.PlayerCardType == PlayerCardType.City && (City)c.Value == discardCityCardOfDestination))
            {
                throw new ApplicationException("Cant take directflight without citycard of destination");
            }

            playerState.Location = discardCityCardOfDestination;
            _playerActions.Add(new PlayerActionWithCity { PlayerAction = PlayerStandardAction.DirectFlight, City = discardCityCardOfDestination });
        }

        /// <summary>
        /// Build research station in current city, by discarding city card of current city
        /// </summary>
        public void BuildResearchStation()
        {
            ValidateAvailableAction();

            var playerState = State.PlayerStates[CurrentPlayerId];
            var currentMapLocation = State.Cities.Single(n => n.City == playerState.Location);

            if (currentMapLocation.HasResearchStation)
            {
                throw new ApplicationException("City already has research station");
            }

            if (playerState.PlayerRole.Equals(PlayerRole.OperationsExpert))
            {
                _playerActions.Add(new PlayerActionWithCity { PlayerAction = PlayerStandardAction.BuildResearchStation, City = playerState.Location });
            }

            if (!playerState.PlayerHand.Any(c => c.PlayerCardType == PlayerCardType.City && (City)c.Value == currentMapLocation.City))
            {
                throw new ApplicationException("Cant build research station without city card in player hand.");
            }

            currentMapLocation.HasResearchStation = true;
            _playerActions.Add(new PlayerActionWithCity { PlayerAction = PlayerStandardAction.BuildResearchStation, City = playerState.Location });
        }


        /// <summary>
        /// PLayer must have the city card of the city they are currently in, and discard that card
        /// To fly anywhere they want
        /// </summary>
        /// <param name="anyCityAsDestination"></param>
        public void CharterFlight(City anyCityAsDestination)
        {
            ValidateAvailableAction();

            var playerState = State.PlayerStates[CurrentPlayerId];
            var currentLocation = playerState.Location;

            if (!playerState.PlayerHand.Exists(c => c.PlayerCardType == PlayerCardType.City && (City)c.Value == currentLocation))
            {
                throw new ApplicationException("Cant take charterflight without citycard matching current location");
            }

            playerState.Location = anyCityAsDestination;
            _playerActions.Add(new PlayerActionWithCity { PlayerAction = PlayerStandardAction.CharterFlight, City = anyCityAsDestination });
        }

        /// <summary>
        /// Player must be in a city with a research city
        /// Allows travel to any other city with a research station
        /// </summary>
        /// <param name="anyCityAlsoWithResearchStation"></param>
        public void ShuttleFlight(City anyCityAlsoWithResearchStation)
        {
            ValidateAvailableAction();

            var playerState = State.PlayerStates[CurrentPlayerId];
            var currentMapLocation = State.Cities.Single(n => n.City == playerState.Location);
            var destinationMapLocation = State.Cities.Single(n => n.City == anyCityAlsoWithResearchStation);

            if (!currentMapLocation.HasResearchStation || !destinationMapLocation.HasResearchStation || currentMapLocation.City == destinationMapLocation.City)
            {
                throw new ApplicationException("Can only take shuttleflight between two cities with research stations");
            }

            playerState.Location = anyCityAlsoWithResearchStation;
            _playerActions.Add(new PlayerActionWithCity { PlayerAction = PlayerStandardAction.ShuttleFlight, City = anyCityAlsoWithResearchStation });
        }

        public void TreatDisease()
        {
            ValidateAvailableAction();

            var playerState = State.PlayerStates[CurrentPlayerId];
            _playerActions.Add(new PlayerActionWithCity { PlayerAction = PlayerStandardAction.TreatDisease, City = playerState.Location });
        }

        public void ShareKnowledge(City shareCity, int playerId)
        {
            // TODO, handle all playerroles
            ValidateAvailableAction();

            var playerState = State.PlayerStates[CurrentPlayerId];

            var currentMapLocation = State.Cities.Single(n => n.City == playerState.Location);
            var playersAtSameLocation = State.PlayerStates.Where(p => p.Value.Location == currentMapLocation.City && p.Key != CurrentPlayerId).ToList();

            if (!playersAtSameLocation.Any())
            {
                throw new ApplicationException("No other player at same location");
            }

            if (playerState.PlayerRole == PlayerRole.Researcher)
            {
                _playerActions.Add(new PlayerActionWithCity { PlayerAction = PlayerStandardAction.ShareKnowledge, City = playerState.Location });
            }

            if (currentMapLocation.City != shareCity)
            {
                throw new ApplicationException("Can onl");
            }

            var currentPlayerHasCityCard = playerState.PlayerHand.Any(c => c.PlayerCardType == PlayerCardType.City && (City)c.Value == currentMapLocation.City);
            if (currentPlayerHasCityCard)
            {
                _playerActions.Add(new PlayerActionWithCity { PlayerAction = PlayerStandardAction.ShareKnowledge, City = playerState.Location });
            }
            else
            {
                // TODO check that another player at same location, has current location city card in hand
            }

        }


        public void DiscoverACure(Disease disease)
        {
            ValidateAvailableAction();

            var playerState = State.PlayerStates[CurrentPlayerId];
            var currentMapLocation = State.Cities.Single(n => n.City == playerState.Location);
            if (!currentMapLocation.HasResearchStation)
            {
                throw new ApplicationException("You are not at a research station");
            }

            var matchingCards = playerState.PlayerHand.Count(c => c.PlayerCardType == PlayerCardType.City && ((City)c.Value).GetDefaultDisease() == disease);
            var cardsNeededForCure = playerState.PlayerRole == PlayerRole.Scientist ? 4 : 5;
            if (matchingCards < cardsNeededForCure)
            {
                throw new ApplicationException($"You need {cardsNeededForCure} cards of matching color for cure");
            }

            _playerActions.Add(new PlayerActionWithCity { PlayerAction = PlayerStandardAction.DiscoverCure, City = playerState.Location, Disease = disease });
        }

        // TODO Dispatcher can move another players pawn

        // 

        private void ValidateAvailableAction()
        {
            if (_playerActions.Count == 4)
            {
                throw new ApplicationException("Only four actions allowed per turn");
            }
        }
    }

}