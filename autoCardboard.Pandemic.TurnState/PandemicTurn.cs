using System;
using System.Collections.Generic;
using System.Linq;
using autoCardboard.Infrastructure;
using autoCardboard.Infrastructure.Exceptions;
using autoCardboard.Pandemic.State;

namespace autoCardboard.Pandemic.TurnState
{
     [Serializable]
    // This class is responsible for presenting the choices to a IPlayer that they have in making a turn in a way thats easy to understand.
    // The state class in here is a clone of the game state as players dont have ability to directly modify the game state.
    // It performs some validation to prevent invalid player turns being presented back to the game.
    public class PandemicTurn : IPandemicTurn
    {
        private readonly ICardboardLogger _log;

        // _state is a clone of the game state 
        // game state can only be permanently changed by game, not by players, otherwise they could cheat
        private PandemicState _state;

        private readonly IPandemicActionValidator _validator;

        public int CurrentPlayerId { get; set; }

        public PlayerAction ActionTaken { get; set; }
        public IEnumerable<PandemicPlayerCard> CardsToDiscard { get;set; }

        public PandemicTurnType TurnType { get; set; }

        public bool OneQuietNight { get; set; }

        public IPandemicState State
        {
            get
            {
                return _state;
            }

            set
            {
                // Clone the game state, so if a inmemory player modifies it, this doesnt modify the actual game state
                _state = value.Clone() as PandemicState;
            }
        }

        public PandemicTurn(ICardboardLogger log, IPandemicActionValidator validator)
        {
            _log = log;
            _validator = validator;
            CardsToDiscard = new List<PandemicPlayerCard>();
        }

        public void BuildResearchStation(City city)
        {
            var playerAction = new PlayerAction {PlayerId = CurrentPlayerId, PlayerActionType = PlayerActionType.BuildResearchStation, City = city};

            var validationFailures = _validator.ValidatePlayerAction(CurrentPlayerId, State, playerAction).ToList();

            if (validationFailures.Any())
            {
                throw new CardboardException(validationFailures[0]);
            }

            ActionTaken = playerAction;
        }

        public void DriveOrFerry(City toConnectedCity)
        {
            var playerAction = new PlayerAction {PlayerId = CurrentPlayerId, PlayerActionType = PlayerActionType.DriveOrFerry, City = toConnectedCity};
            var validationFailures = _validator.ValidatePlayerAction(CurrentPlayerId, State, playerAction).ToList();

            if (validationFailures.Any())
            {
                throw new CardboardException(validationFailures[0]);
            }

            ActionTaken = playerAction;
        }
      
        public void DirectFlight(City discardCityCardOfDestination)
        {
            var playerState = State.PlayerStates[CurrentPlayerId];
            var newPlayerTurn = new PlayerAction { PlayerId = CurrentPlayerId, PlayerActionType = PlayerActionType.DirectFlight, City = discardCityCardOfDestination };

            var validationFailures = _validator.ValidatePlayerAction(CurrentPlayerId, State, newPlayerTurn).ToList();

            if (validationFailures.Any())
            {
                throw new CardboardException(validationFailures[0]);
            }

            playerState.Location = discardCityCardOfDestination;
            ActionTaken = newPlayerTurn;
        }

        /// <summary>
        /// Build research station in current city, by discarding city card of current city
        /// </summary>
        public void BuildResearchStation()
        {
            var playerState = State.PlayerStates[CurrentPlayerId];
            var currentMapLocation = State.Cities.Single(n => n.City == playerState.Location);
            var newPlayerTurn = new PlayerAction { PlayerId = CurrentPlayerId, PlayerActionType = PlayerActionType.BuildResearchStation, City = playerState.Location };

           var validationFailures = _validator.ValidatePlayerAction(CurrentPlayerId, State, newPlayerTurn).ToList();

            if (validationFailures.Any())
            {
                throw new CardboardException(validationFailures[0]);
            }

            currentMapLocation.HasResearchStation = true;
            ActionTaken = newPlayerTurn;
        }


        /// <summary>
        /// PLayer must have the city card of the city they are currently in, and discard that card
        /// To fly anywhere they want
        /// </summary>
        /// <param name="anyCityAsDestination"></param>
        public void CharterFlight(City anyCityAsDestination)
        {
            var playerState = State.PlayerStates[CurrentPlayerId];
            var newPlayerTurn = new PlayerAction { PlayerId = CurrentPlayerId, PlayerActionType = PlayerActionType.CharterFlight, City = anyCityAsDestination };

            var validationFailures = _validator.ValidatePlayerAction(CurrentPlayerId, State, newPlayerTurn).ToList();

            if (validationFailures.Any())
            {
                throw new CardboardException(validationFailures[0]);
            }

            playerState.Location = anyCityAsDestination;
            ActionTaken = newPlayerTurn;
        }

        /// <summary>
        /// Player must be in a city with a research city
        /// Allows travel to any other city with a research station
        /// </summary>
        /// <param name="anyCityAlsoWithResearchStation"></param>
        public void ShuttleFlight(City anyCityAlsoWithResearchStation)
        {
            var playerState = State.PlayerStates[CurrentPlayerId];
            var newPlayerTurn = new PlayerAction { PlayerId = CurrentPlayerId, PlayerActionType = PlayerActionType.ShuttleFlight, City = anyCityAlsoWithResearchStation };

            var validationFailures = _validator.ValidatePlayerAction(CurrentPlayerId, State, newPlayerTurn).ToList();

            if (validationFailures.Any())
            {
                throw new CardboardException(validationFailures[0]);
            }

            playerState.Location = anyCityAlsoWithResearchStation;
            ActionTaken = newPlayerTurn;
        }

        public void TreatDisease(Disease disease)
        {
            var playerState = State.PlayerStates[CurrentPlayerId];
            ActionTaken = new PlayerAction { PlayerId = CurrentPlayerId, PlayerActionType = PlayerActionType.TreatDisease, City = playerState.Location, Disease = disease };
        }

        public void ShareKnowledge(City shareCity, int playerId)
        {
            var playerState = State.PlayerStates[CurrentPlayerId];

            var newPlayerTurn = new PlayerAction { PlayerId = CurrentPlayerId, PlayerActionType = PlayerActionType.ShareKnowledge, City = playerState.Location };

            var validationFailures = _validator.ValidatePlayerAction(CurrentPlayerId, State, newPlayerTurn).ToList();

            if (validationFailures.Any())
            {
                throw new CardboardException(validationFailures[0]);
            }

            ActionTaken = newPlayerTurn;
        }

        public void DiscoverCure(Disease disease, IEnumerable<PandemicPlayerCard> cardsToDiscard)
        {
            var playerState = State.PlayerStates[CurrentPlayerId];
            var newPlayerTurn = new PlayerAction
            {
                PlayerId = CurrentPlayerId, 
                PlayerActionType = PlayerActionType.DiscoverCure, 
                City = playerState.Location, 
                Disease = disease,
                CardsToDiscard = cardsToDiscard
            };

            var validationFailures = _validator.ValidatePlayerAction(CurrentPlayerId, State, newPlayerTurn).ToList();

            if (validationFailures.Any())
            {
                throw new CardboardException(validationFailures[0]);
            }

            ActionTaken = newPlayerTurn;
        }

        public void PlayOneQuietNight()
        {
            this.OneQuietNight = true;
        }

    }
}
