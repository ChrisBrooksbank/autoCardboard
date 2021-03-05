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
            var playersWeCouldShareKnowledgeWith = PlayersWeCouldKnowledgeShareWith(currentPlayerId, state);

            var playerToShareKnowledgeWith = playersWeCouldShareKnowledgeWith?.FirstOrDefault();
            if (playerToShareKnowledgeWith == null)
            {
                return null;
            }

            // TODO only suggest the share if it benefits the players !!!


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
            // If a researcher is in this city, and has at least one city card, then knowledge sharing is possible, he can give ( not take )
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
