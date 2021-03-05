using System;
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
            var city = currentPlayerState.Location;
            var playersWeCouldShareKnowledgeWith = PlayersWeCouldKnowledgeShareWith(currentPlayerId, state);

            // if we are a researcher, then we can give any city card ( can go other way too, but this will be sufficient for bot )
            if (currentPlayerState.PlayerRole == PlayerRole.Researcher)
            {
                var suggestedKnowledgeShare = GetSuggestedKnowledgeShareForResearcher(currentPlayerId, state,
                    playersWeCouldShareKnowledgeWith,
                    currentPlayerState, diseaseStates, currentPlayerCityCards);
                return suggestedKnowledgeShare;
            }

            // if the current city is for a cured or eradicated disease - don't knowledge share
            if (diseaseStates[city.GetDefaultDisease()] != DiseaseState.NotCured)
            {
                return null;
            }
            
            var playerToShareKnowledgeWith = playersWeCouldShareKnowledgeWith?.FirstOrDefault();

            if (playerToShareKnowledgeWith == null)
            {
                return null;
            }

            // do not return a knowledgeShare unless it improves gameState
            // Otherwise we may just be knowledge sharing the same card back and forth !
            var disease = city.GetDefaultDisease();
            if (diseaseStates[disease] != DiseaseState.NotCured)
            {
                return null;
            }

            var candidatePlayer = state.PlayerStates[playerToShareKnowledgeWith.Value];
            var ourDiseaseCount = currentPlayerState.PlayerHand
                .Count(c => c.PlayerCardType == PlayerCardType.City && ((City) c.Value).GetDefaultDisease() == disease);
            var candidateDiseaseCount = candidatePlayer.PlayerHand
                .Count(c => c.PlayerCardType == PlayerCardType.City && ((City) c.Value).GetDefaultDisease() == disease);

            if (ourDiseaseCount == 0 || candidateDiseaseCount <= ourDiseaseCount)
            {
                return null;
            }

            return new KnowledgeShare
            {
                Player1 = currentPlayerId,
                Player2 = playerToShareKnowledgeWith.Value,
                CityCardToGive = state.PlayerStates[currentPlayerId].Location
            };
        }

        private KnowledgeShare GetSuggestedKnowledgeShareForResearcher(int currentPlayerId, IPandemicState state,
            List<int> playersWeCouldShareKnowledgeWith, PandemicPlayerState currentPlayerState, Dictionary<Disease, DiseaseState> diseaseStates,
            List<City> currentPlayerCityCards)
        {
            foreach (var playerWeCouldShareKnowledgeWith in playersWeCouldShareKnowledgeWith)
            {
                var candidatePlayerState = state.PlayerStates[playerWeCouldShareKnowledgeWith];

                foreach (var disease in (Disease[]) Enum.GetValues(typeof(Disease)))
                {
                    var ourDiseaseCount = currentPlayerState.PlayerHand
                        .Count(c => c.PlayerCardType == PlayerCardType.City && ((City) c.Value).GetDefaultDisease() == disease);
                    var candidateDiseaseCount = candidatePlayerState.PlayerHand
                        .Count(c => c.PlayerCardType == PlayerCardType.City && ((City) c.Value).GetDefaultDisease() == disease);

                    if (diseaseStates[disease] == DiseaseState.NotCured && ourDiseaseCount > 0 &&
                        candidateDiseaseCount > ourDiseaseCount)
                    {
                        {
                            return new KnowledgeShare
                            {
                                Player1 = currentPlayerId,
                                Player2 = playerWeCouldShareKnowledgeWith,
                                CityCardToGive = currentPlayerCityCards.First(c => c.GetDefaultDisease() == disease)
                            };
                        }
                    }
                }
            }

            return null;
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
