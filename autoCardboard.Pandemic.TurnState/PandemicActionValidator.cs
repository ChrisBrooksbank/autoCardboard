﻿using autoCardboard.Pandemic.State;
using System.Collections.Generic;
using System.Linq;

namespace autoCardboard.Pandemic.TurnState
{
    
    /// <summary>
    /// This class is responsible for validating that players actions are valid.
    /// Its called when a player is indicating their actions, using a PandemicTurn.
    /// Its also called by the PandemicGame to do the same validation before updating the actual game state.
    /// </summary>
    public class PandemicActionValidator: IPandemicActionValidator
    {
        public IEnumerable<string> ValidatePlayerAction(int playerId, IPandemicState state, PlayerAction newPlayerAction)
        {
            var validationFailures = new List<string>();
            var validationFailure = GetActionValidationFailures(state, playerId, newPlayerAction);
            if (!string.IsNullOrWhiteSpace(validationFailure))
            {
                validationFailures.Add(validationFailure);
            }
            
            return validationFailures;
        }

        public IEnumerable<string> ValidatePlayerEventPlayed(int playerId, IPandemicState state, PlayerEventPlayed newEventCard)
        {
            var validationFailures = new List<string>();
            var playerState = state.PlayerStates[playerId];

            if (!playerState.PlayerHand.Any(c => c.PlayerCardType == PlayerCardType.Event && (EventCard)c.Value == newEventCard.EventCard))
            {
                validationFailures.Add($"Player doesn't have event card {newEventCard.EventCard}");
            }

            return validationFailures;
        }

        private string GetActionValidationFailures(IPandemicState state, int playerId, PlayerAction playerAction)
        {
            var playerState = state.PlayerStates[playerId];
            var currentMapLocation = state.Cities.Single(n => n.City == playerState.Location);
            var playerTurnCity = state.Cities.Single(n => n.City == playerAction.City);
            var playersAtSameLocation = state.PlayerStates.Where(p => p.Value.Location == currentMapLocation.City && p.Key != playerId).ToList();


            if (playerAction.PlayerActionType == PlayerActionType.DriveOrFerry)
            {
                if(currentMapLocation.ConnectedCities.All(c => c != playerAction.City))
                {
                    return $"Cant drive or ferry to {playerAction.City} because its not connected to {currentMapLocation.City}";
                }
            }

            if (playerAction.PlayerActionType == PlayerActionType.DirectFlight)
            {
                if (!playerState.PlayerHand.Exists(c => c.PlayerCardType == PlayerCardType.City && (City)c.Value == playerAction.City))
                {
                    return "Cant take directflight without citycard of destination";
                }
            }

            if (playerAction.PlayerActionType == PlayerActionType.BuildResearchStation)
            {
                if (currentMapLocation.HasResearchStation)
                {
                    return "City already has research station";
                }
                if (!playerState.PlayerRole.Equals(PlayerRole.OperationsExpert))
                {
                    if (!playerState.PlayerHand.Any(c => c.PlayerCardType == PlayerCardType.City && (City)c.Value == currentMapLocation.City))
                    {
                        return "Cant build research station without city card in player hand.";
                    }
                }
            }

            if (playerAction.PlayerActionType == PlayerActionType.CharterFlight)
            {
                if (!playerState.PlayerHand.Exists(c => c.PlayerCardType == PlayerCardType.City && (City)c.Value == currentMapLocation.City))
                {
                    return "Cant take charterflight without citycard matching current location";
                }
            }

            if (playerAction.PlayerActionType == PlayerActionType.ShuttleFlight)
            {
                if (!currentMapLocation.HasResearchStation || !playerTurnCity.HasResearchStation || currentMapLocation.City == playerTurnCity.City)
                {
                    return "Can only take shuttleflight between two cities with research stations";
                }
            }

            if (playerAction.PlayerActionType == PlayerActionType.ShareKnowledge)
            {
                if (!playersAtSameLocation.Any())
                {
                   return "No other player at same location";
                }

                if (currentMapLocation != playerTurnCity)
                {
                    return "Can only share in same city";
                }

                var otherPlayerState = state.PlayerStates.SingleOrDefault(p => p.Key == playerAction.OtherPlayerId).Value;

                var currentPlayerHasCityCard = playerState.PlayerHand
                    .Any(c => c.PlayerCardType == PlayerCardType.City && (City)c.Value == currentMapLocation.City);
                var otherPlayerHasCityCard = otherPlayerState.PlayerHand
                    .Any(c => c.PlayerCardType == PlayerCardType.City && (City)c.Value == currentMapLocation.City);
                if (!currentPlayerHasCityCard && !otherPlayerHasCityCard && playerState.PlayerRole != PlayerRole.Researcher && otherPlayerState.PlayerRole != PlayerRole.Researcher)
                {
                    return "Neither player has city card, of current location, and neither is a researcher";
                }
            }

            if ( playerAction.PlayerActionType == PlayerActionType.DiscoverCure )
            {
                if (!currentMapLocation.HasResearchStation)
                {
                    return "You are not at a research station";
                }
                var matchingCards = playerState.PlayerHand.Count(c => c.PlayerCardType == PlayerCardType.City && ((City)c.Value).GetDefaultDisease() == playerAction.Disease);
                var cardsNeededForCure = playerState.PlayerRole == PlayerRole.Scientist ? 4 : 5;
                if (matchingCards < cardsNeededForCure)
                {
                    return $"You need {cardsNeededForCure} cards of matching color for cure";
                }
            }

            return "";
        }
    }
}
