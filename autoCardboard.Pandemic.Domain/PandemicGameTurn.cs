using autoCardboard.Common.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace autoCardboard.Pandemic.Domain
{
    [Serializable]
    // TODO add operations for available play actions
    // each player has four actions
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
            if (_playerActions.Count == 4)
            {
                throw new ApplicationException("Only four actions allowed per turn");
            }

            var playerState = State.PlayerStates[CurrentPlayerId];
            var currentMapLocation = State.Board.Cities.Single(n => n.City == playerState.Location);

            if (!currentMapLocation.ConnectedCities.Any(c => c == toConnectedCity))
            {
                throw new ApplicationException("Cant driver or ferry here because its not connected to current location");
            }

            // this state change wont last beyond takinbg turn, but prevents turn validation failing on next action
            _playerActions.Add(new PlayerActionWithCity { PlayerAction = PlayerStandardAction.DriveOrFerry, City = toConnectedCity });
        }

        public void DirectFlight(City discardCityCardOfDestination)
        {
            if (_playerActions.Count == 4)
            {
                throw new ApplicationException("Only four actions allowed per turn");
            }

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
            if (_playerActions.Count == 4)
            {
                throw new ApplicationException("Only four actions allowed per turn");
            }

            var playerState = State.PlayerStates[CurrentPlayerId];
            var currentMapLocation = State.Board.Cities.Single(n => n.City == playerState.Location);

            if (currentMapLocation.HasResearchStation)
            {
                throw new ApplicationException("City already has research station");
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
            if (_playerActions.Count == 4)
            {
                throw new ApplicationException("Only four actions allowed per turn");
            }

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
            if (_playerActions.Count == 4)
            {
                throw new ApplicationException("Only four actions allowed per turn");
            }

            var playerState = State.PlayerStates[CurrentPlayerId];
            var currentMapLocation = State.Board.Cities.Single(n => n.City == playerState.Location);
            var destinationMapLocation = State.Board.Cities.Single(n => n.City == anyCityAlsoWithResearchStation);

            if (!currentMapLocation.HasResearchStation || !destinationMapLocation.HasResearchStation || currentMapLocation.City == destinationMapLocation.City)
            {
                throw new ApplicationException("Can only take shuttleflight between two cities with research stations");
            }

            playerState.Location = anyCityAlsoWithResearchStation;
            _playerActions.Add(new PlayerActionWithCity { PlayerAction = PlayerStandardAction.ShuttleFlight, City = anyCityAlsoWithResearchStation });
        }

        public void TreatDisease()
        {
            if (_playerActions.Count == 4)
            {
                throw new ApplicationException("Only four actions allowed per turn");
            }

            var playerState = State.PlayerStates[CurrentPlayerId];
            _playerActions.Add(new PlayerActionWithCity { PlayerAction = PlayerStandardAction.TreatDisease, City = playerState.Location });
        }

        public void ShareKnowledge(int shareKnowledgeWithOtherPlayerId)
        {
            // TODO
            if (_playerActions.Count == 4)
            {
                throw new ApplicationException("Only four actions allowed per turn");
            }
        }

        public void DiscoverACure(Disease disease)
        {
            // TODO
            if (_playerActions.Count == 4)
            {
                throw new ApplicationException("Only four actions allowed per turn");
            }
        }
    }
}