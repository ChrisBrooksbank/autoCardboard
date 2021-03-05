using System.Collections.Generic;
using System.Linq;
using autoCardboard.Pandemic.State;

namespace autoCardBoard.Pandemic.Bots
{
    public class KnowledgeShareHelper: IKnowledgeShareHelper
    {
        public bool CanKnowledgeShare(int currentPlayerId, IPandemicState state)
        {
            var playersWeCouldShareKnowledgeWith = PlayersWeCouldKnowledgeShareWith(currentPlayerId, state);
            return playersWeCouldShareKnowledgeWith.Any();
        }

        public KnowledgeShare GetSuggestedKnowledgeShare(int currentPlayerId, IPandemicState state)
        {
            var currentPlayerState = state.PlayerStates[currentPlayerId];
            var currentPlayerCityCards = currentPlayerState.PlayerHand
                .Where(c => c.PlayerCardType == PlayerCardType.City).Select(c => (City)c.Value).ToList();
            var diseaseStates = state.DiscoveredCures;
            var citiesWithoutCures = state.Cities.Where( n => diseaseStates[n.City.GetDefaultDisease()] == DiseaseState.NotCured)
                .Select(c => c.City).ToList();
            var city = currentPlayerState.Location;
            var disease = city.GetDefaultDisease();
            var playersWeCouldShareKnowledgeWith = PlayersWeCouldKnowledgeShareWith(currentPlayerId, state);
            var weAreAResearcher = currentPlayerState.PlayerRole == PlayerRole.Researcher;
            var cityCardsWeHaveWithUncuredDiseases = currentPlayerCityCards.Where( c => citiesWithoutCures.Contains(c)).ToList();

            // if we are a researcher, then we can give any city card ( can go other way too, but this will be sufficient for bot )
            // TODO refactor this incredibly long piece of code ! This was just to understand how to do it, now tidy up !
            if (weAreAResearcher)
            {
               foreach (var playerWeCouldShareKnowledgeWith in playersWeCouldShareKnowledgeWith)
               {
                   var candidatePlayerState = state.PlayerStates[playerWeCouldShareKnowledgeWith];
                   var candidateBlackCount = candidatePlayerState.PlayerHand
                       .Count(c => c.PlayerCardType == PlayerCardType.City && ((City)c.Value).GetDefaultDisease() == Disease.Black);
                   var candidateBlueCount = candidatePlayerState.PlayerHand
                       .Count(c => c.PlayerCardType == PlayerCardType.City && ((City)c.Value).GetDefaultDisease() == Disease.Blue);
                   var candidateYellowCount = candidatePlayerState.PlayerHand
                       .Count(c => c.PlayerCardType == PlayerCardType.City && ((City)c.Value).GetDefaultDisease() == Disease.Yellow);
                   var candidateRedCount = candidatePlayerState.PlayerHand
                       .Count(c => c.PlayerCardType == PlayerCardType.City && ((City)c.Value).GetDefaultDisease() == Disease.Red);

                   var ourBlackCount = currentPlayerState.PlayerHand
                       .Count(c => c.PlayerCardType == PlayerCardType.City && ((City)c.Value).GetDefaultDisease() == Disease.Black);
                   var ourBlueCount = currentPlayerState.PlayerHand
                       .Count(c => c.PlayerCardType == PlayerCardType.City && ((City)c.Value).GetDefaultDisease() == Disease.Blue);
                   var ourYellowCount = currentPlayerState.PlayerHand
                       .Count(c => c.PlayerCardType == PlayerCardType.City && ((City)c.Value).GetDefaultDisease() == Disease.Yellow);
                   var ourRedCount = currentPlayerState.PlayerHand
                       .Count(c => c.PlayerCardType == PlayerCardType.City && ((City)c.Value).GetDefaultDisease() == Disease.Red);

                   if (diseaseStates[Disease.Black] == DiseaseState.NotCured && ourBlackCount > 0 && candidateBlackCount > ourBlackCount)
                   {
                       return new KnowledgeShare
                       {
                           Player1 = currentPlayerId,
                           Player2 = playerWeCouldShareKnowledgeWith,
                           CityCardToGive = currentPlayerCityCards.First(c => c.GetDefaultDisease() == Disease.Black)
                       };
                   }

                   if (diseaseStates[Disease.Blue] == DiseaseState.NotCured && ourBlueCount > 0 && candidateBlueCount > ourBlueCount)
                   {
                       return new KnowledgeShare
                       {
                           Player1 = currentPlayerId,
                           Player2 = playerWeCouldShareKnowledgeWith,
                           CityCardToGive = currentPlayerCityCards.First(c => c.GetDefaultDisease() == Disease.Blue)
                       };
                   }

                   if (diseaseStates[Disease.Yellow] == DiseaseState.NotCured && ourYellowCount > 0 && candidateYellowCount > ourYellowCount)
                   {
                       return new KnowledgeShare
                       {
                           Player1 = currentPlayerId,
                           Player2 = playerWeCouldShareKnowledgeWith,
                           CityCardToGive = currentPlayerCityCards.First(c => c.GetDefaultDisease() == Disease.Yellow)
                       };
                   }

                   if (diseaseStates[Disease.Red] == DiseaseState.NotCured && ourRedCount > 0 && candidateRedCount > ourRedCount)
                   {
                       return new KnowledgeShare
                       {
                           Player1 = currentPlayerId,
                           Player2 = playerWeCouldShareKnowledgeWith,
                           CityCardToGive = currentPlayerCityCards.First(c => c.GetDefaultDisease() == Disease.Red)
                       };
                   }
               }

            }

            // if the current city is for a cured or eradicated disease - don't knowledge share
            if (diseaseStates[disease] != DiseaseState.NotCured)
            {
                return null;
            }
            
            var playerToShareKnowledgeWith = playersWeCouldShareKnowledgeWith?.FirstOrDefault();

            if (playerToShareKnowledgeWith == null)
            {
                return null;
            }

            // TODO do not return a knowledgeShare unless it improves gameState
            // Otherwise we may just be knowledge sharing the same card back and forth !
            // maybe introduce some randomness to knowledge share as well
            // Similar to researcher above, only move city cards to a player who already has more of that colour than us
            
            return new KnowledgeShare
            {
                Player1 = currentPlayerId,
                Player2 = playerToShareKnowledgeWith.Value,
                CityCardToGive = state.PlayerStates[currentPlayerId].Location
            };
        }
        
        private List<int> PlayersWeCouldKnowledgeShareWith(int currentPlayerId, IPandemicState state)
        {
            var currentPlayerState = state.PlayerStates[currentPlayerId];
            var currentPlayerLocation = currentPlayerState.Location;
            var currentPlayerRole = currentPlayerState.PlayerRole;
            var currentPlayerCityCards = currentPlayerState.PlayerHand
                .Where(c => c.PlayerCardType == PlayerCardType.City).ToList();

            // If there are no other players in the same location as this player, then no knowledge sharing is possible
            var otherPlayersInSameCity = state.PlayerStates
                .Where(p => p.Key != currentPlayerId && p.Value.Location == currentPlayerLocation).ToList();
            if (!otherPlayersInSameCity.Any())
            {
                return new List<int>();
            }

            var weCanShareWithPlayerIds = new List<int>();
            // If a researcher is in this city, and has at least one city card, then knowledge sharing is possible
            if ( (currentPlayerRole == PlayerRole.Researcher && currentPlayerCityCards.Any())
                 || (otherPlayersInSameCity.Any(p => p.Value.PlayerRole == PlayerRole.Researcher 
                                                     && p.Value.PlayerHand.Any(c => c.PlayerCardType == PlayerCardType.City))))
            {
                weCanShareWithPlayerIds.AddRange(otherPlayersInSameCity.Select(p => p.Key).ToList());
            }

            // If another player in this city, has the city card of this city, then knowledge sharing is possible
            weCanShareWithPlayerIds.AddRange(otherPlayersInSameCity
                .Where(c => c.Value.PlayerHand.Any( ph => ph.PlayerCardType == PlayerCardType.City && (City)ph.Value == currentPlayerLocation))
                .Select(p => p.Key).ToList());
         
            return weCanShareWithPlayerIds;
        }
    }
}
