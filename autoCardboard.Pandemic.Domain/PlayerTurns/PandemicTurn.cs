using autoCardboard.Common.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace autoCardboard.Pandemic.Domain
{
    [Serializable]
    // This class is responsible for presenting the choices to a IPlayer that they have in making a turn in a way thats easy to understand.
    // The state class in here is a clone of the game state as players dont have ability to directly modify the game state.
    // It performs some validation to prevent invalid player turns being presented back to the game.

    // TODO move code out e.g. => PandemicTurnHandler, e.g. for DriveOrFerry, keep calls to validator in here ???
    public class PandemicTurn : IGameTurn
    {
        // _state is a clone of the game state 
        // game state can only be permanently changed by game, not by players, otherwise they could cheat
        private PandemicGameState _state;

        private List<PlayerActionWithCity> _playerActions = new List<PlayerActionWithCity>();

        private readonly PandemicTurnValidator _validator;

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

        public PandemicTurn()
        {
            _validator = new PandemicTurnValidator();
        }

        public void DriveOrFerry(City toConnectedCity)
        {
            var playerAction = new PlayerActionWithCity {PlayerAction = PlayerStandardAction.DriveOrFerry, City = toConnectedCity};
            var validationFailures = _validator.ValidatePlayerTurns(CurrentPlayerId, State, _playerActions, playerAction).ToList();

            if (validationFailures.Any())
            {
                throw new ApplicationException(validationFailures[0]);
            }

            _playerActions.Add(playerAction);
        }
      
        public void DirectFlight(City discardCityCardOfDestination)
        {
            var playerState = State.PlayerStates[CurrentPlayerId];
            var newPlayerTurn = new PlayerActionWithCity { PlayerAction = PlayerStandardAction.DirectFlight, City = discardCityCardOfDestination };

            var validationFailures = _validator.ValidatePlayerTurns(CurrentPlayerId, State, _playerActions, newPlayerTurn).ToList();

            if (validationFailures.Any())
            {
                throw new ApplicationException(validationFailures[0]);
            }

            playerState.Location = discardCityCardOfDestination;
            _playerActions.Add(newPlayerTurn);
        }

        /// <summary>
        /// Build research station in current city, by discarding city card of current city
        /// </summary>
        public void BuildResearchStation()
        {
            var playerState = State.PlayerStates[CurrentPlayerId];
            var currentMapLocation = State.Cities.Single(n => n.City == playerState.Location);
            var newPlayerTurn = new PlayerActionWithCity { PlayerAction = PlayerStandardAction.BuildResearchStation, City = playerState.Location };

           var validationFailures = _validator.ValidatePlayerTurns(CurrentPlayerId, State, _playerActions, newPlayerTurn).ToList();

            if (validationFailures.Any())
            {
                throw new ApplicationException(validationFailures[0]);
            }

            currentMapLocation.HasResearchStation = true;
            _playerActions.Add(newPlayerTurn);
        }


        /// <summary>
        /// PLayer must have the city card of the city they are currently in, and discard that card
        /// To fly anywhere they want
        /// </summary>
        /// <param name="anyCityAsDestination"></param>
        public void CharterFlight(City anyCityAsDestination)
        {
            var playerState = State.PlayerStates[CurrentPlayerId];
            var newPlayerTurn = new PlayerActionWithCity { PlayerAction = PlayerStandardAction.CharterFlight, City = anyCityAsDestination };

            var validationFailures = _validator.ValidatePlayerTurns(CurrentPlayerId, State, _playerActions, newPlayerTurn).ToList();

            if (validationFailures.Any())
            {
                throw new ApplicationException(validationFailures[0]);
            }

            playerState.Location = anyCityAsDestination;
            _playerActions.Add(newPlayerTurn);
        }

        /// <summary>
        /// Player must be in a city with a research city
        /// Allows travel to any other city with a research station
        /// </summary>
        /// <param name="anyCityAlsoWithResearchStation"></param>
        public void ShuttleFlight(City anyCityAlsoWithResearchStation)
        {
            var playerState = State.PlayerStates[CurrentPlayerId];
            var currentMapLocation = State.Cities.Single(n => n.City == playerState.Location);
            var destinationMapLocation = State.Cities.Single(n => n.City == anyCityAlsoWithResearchStation);
            var newPlayerTurn = new PlayerActionWithCity { PlayerAction = PlayerStandardAction.ShuttleFlight, City = anyCityAlsoWithResearchStation };

            var validationFailures = _validator.ValidatePlayerTurns(CurrentPlayerId, State, _playerActions, newPlayerTurn).ToList();

            if (validationFailures.Any())
            {
                throw new ApplicationException(validationFailures[0]);
            }

            playerState.Location = anyCityAlsoWithResearchStation;
            _playerActions.Add(newPlayerTurn);
        }

        public void TreatDisease(Disease disease)
        {
            var playerState = State.PlayerStates[CurrentPlayerId];
            _playerActions.Add(new PlayerActionWithCity { PlayerAction = PlayerStandardAction.TreatDisease, City = playerState.Location, Disease = disease });
        }

        public void ShareKnowledge(City shareCity, int playerId)
        {
            var playerState = State.PlayerStates[CurrentPlayerId];

            var currentMapLocation = State.Cities.Single(n => n.City == playerState.Location);
            var playersAtSameLocation = State.PlayerStates.Where(p => p.Value.Location == currentMapLocation.City && p.Key != CurrentPlayerId).ToList();
            var newPlayerTurn = new PlayerActionWithCity { PlayerAction = PlayerStandardAction.ShareKnowledge, City = playerState.Location };

            var validationFailures = _validator.ValidatePlayerTurns(CurrentPlayerId, State, _playerActions, newPlayerTurn).ToList();

            if (validationFailures.Any())
            {
                throw new ApplicationException(validationFailures[0]);
            }
           
            var currentPlayerHasCityCard = playerState.PlayerHand.Any(c => c.PlayerCardType == PlayerCardType.City && (City)c.Value == currentMapLocation.City);
            if (currentPlayerHasCityCard)
            {
                _playerActions.Add(newPlayerTurn);
            }
            else
            {
                // TODO check that another player at same location, has current location city card in hand
            }

        }

        public void DiscoverACure(Disease disease)
        {
            var playerState = State.PlayerStates[CurrentPlayerId];
            var currentMapLocation = State.Cities.Single(n => n.City == playerState.Location);
            var newPlayerTurn = new PlayerActionWithCity { PlayerAction = PlayerStandardAction.DiscoverCure, City = playerState.Location, Disease = disease };

            var validationFailures = _validator.ValidatePlayerTurns(CurrentPlayerId, State, _playerActions, newPlayerTurn).ToList();

            if (validationFailures.Any())
            {
                throw new ApplicationException(validationFailures[0]);
            }

            _playerActions.Add(newPlayerTurn);
        }

        // TODO Dispatcher can move another players pawn
    }

}