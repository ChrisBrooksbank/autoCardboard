using autoCardboard.Pandemic.State;
using System.Collections.Generic;
using System.Linq;

namespace autoCardboard.Pandemic.TurnState
{
    
    /// <summary>
    /// This class is responsible for validating that players actions are valid.
    /// Its called when a player is indicating their actions, using a PandemicTurn.
    /// Its also called by the PandemicGame to do the same validation before updating the actual game state.
    /// </summary>
    public class PandemicTurnValidator: IPandemicTurnValidator
    {
        private IPandemicState _state;
        private int _playerId;
        private PandemicPlayerState _pandemicPlayerState;
        private MapNode _currentMapLocation;
        private IPandemicStateEditor _stateEditor;

        public PandemicTurnValidator(IPandemicStateEditor stateEditor)
        {
            _stateEditor = stateEditor;
        }

        public IEnumerable<string> ValidatePlayerAction(int playerId, IPandemicState state, PlayerAction playerAction)
        {
            _state = state.Clone() as IPandemicState;
            state = null;
            _playerId = playerId;
            _pandemicPlayerState = _state.PlayerStates[_playerId];
            _currentMapLocation = _state.Cities.Single(n => n.City == _pandemicPlayerState.Location);

            var validationFailures = new List<string>();
            
            var validationFailure = GetTurnValidationFailure(playerAction);
            if (string.IsNullOrWhiteSpace(validationFailure))
            {
                _stateEditor.ApplyPlayerAction(_state, playerAction);
                _pandemicPlayerState = _state.PlayerStates[_playerId];
                _currentMapLocation = _state.Cities.Single(n => n.City == _pandemicPlayerState.Location);
            }
            else
            {
                validationFailures.Add(validationFailure);
            }
            
            return validationFailures;
        }

        private string GetTurnValidationFailure(PlayerAction playerAction)
        {
            var _playerTurnCity = _state.Cities.Single(n => n.City == playerAction.City);
            var _playersAtSameLocation = _state.PlayerStates.Where(p => p.Value.Location == _currentMapLocation.City && p.Key != _playerId).ToList();

            // TODO SOLID, inject IEnumerable<IActionValidator>

            if (playerAction.PlayerActionType == PlayerActionType.DriveOrFerry)
            {
                if(_currentMapLocation.ConnectedCities.All(c => c != playerAction.City))
                {
                    return $"Cant drive or ferry to {playerAction.City} because its not connected to {_currentMapLocation.City}";
                }
            }

            if (playerAction.PlayerActionType == PlayerActionType.DirectFlight)
            {
                if (!_pandemicPlayerState.PlayerHand.Exists(c => c.PlayerCardType == PlayerCardType.City && (City)c.Value == _playerTurnCity.City))
                {
                    return "Cant take directflight without citycard of destination";
                }
            }

            if (playerAction.PlayerActionType == PlayerActionType.BuildResearchStation)
            {
                if (_currentMapLocation.HasResearchStation)
                {
                    return "City already has research station";
                }
                if (!_pandemicPlayerState.PlayerRole.Equals(PlayerRole.OperationsExpert))
                {
                    if (!_pandemicPlayerState.PlayerHand.Any(c => c.PlayerCardType == PlayerCardType.City && (City)c.Value == _currentMapLocation.City))
                    {
                        return "Cant build research station without city card in player hand.";
                    }
                }
            }

            if (playerAction.PlayerActionType == PlayerActionType.CharterFlight)
            {
                if (!_pandemicPlayerState.PlayerHand.Exists(c => c.PlayerCardType == PlayerCardType.City && (City)c.Value == _currentMapLocation.City))
                {
                    return "Cant take charterflight without citycard matching current location";
                }
            }

            if (playerAction.PlayerActionType == PlayerActionType.ShuttleFlight)
            {
                if (!_currentMapLocation.HasResearchStation || !_playerTurnCity.HasResearchStation || _currentMapLocation.City == _playerTurnCity.City)
                {
                    return "Can only take shuttleflight between two cities with research stations";
                }
            }

            if (playerAction.PlayerActionType == PlayerActionType.ShareKnowledge)
            {
                if (!_playersAtSameLocation.Any())
                {
                   return "No other player at same location";
                }

                if (_currentMapLocation != _playerTurnCity)
                {
                    return "Can only share in same city";
                }
                var currentPlayerHasCityCard = _pandemicPlayerState.PlayerHand.Any(c => c.PlayerCardType == PlayerCardType.City && (City)c.Value == _currentMapLocation.City);
                if (!currentPlayerHasCityCard)
                {
                    return "Doesnt have city card";
                }
                
                
                // TODO check that another player at same location, has current location city card in hand


            }

            if ( playerAction.PlayerActionType == PlayerActionType.DiscoverCure )
            {
                if (!_currentMapLocation.HasResearchStation)
                {
                    return "You are not at a research station";
                }
                var matchingCards = _pandemicPlayerState.PlayerHand.Count(c => c.PlayerCardType == PlayerCardType.City && ((City)c.Value).GetDefaultDisease() == playerAction.Disease);
                var cardsNeededForCure = _pandemicPlayerState.PlayerRole == PlayerRole.Scientist ? 4 : 5;
                if (matchingCards < cardsNeededForCure)
                {
                    return $"You need {cardsNeededForCure} cards of matching color for cure";
                }
            }

            return "";
        }
    }
}
