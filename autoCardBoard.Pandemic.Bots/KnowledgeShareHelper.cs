using System.Linq;
using System.Transactions;
using autoCardboard.Pandemic.State;

namespace autoCardBoard.Pandemic.Bots
{
    public class KnowledgeShareHelper: IKnowledgeShareHelper
    {
        public bool CanKnowledgeShare(int currentPlayerId, IPandemicState state)
        {
            var currentPlayerState = state.PlayerStates[currentPlayerId];
            var currentPlayerLocation = currentPlayerState.Location;
            var currentPlayerRole = currentPlayerState.PlayerRole;
            var currentPlayerCityCards = currentPlayerState.PlayerHand.Where(c => c.PlayerCardType == PlayerCardType.City);

            // If there are no other players in the same location as this player, then no knowledge sharing is possible
            var otherPlayersInSameCity = state.PlayerStates
                .Where(p => p.Key != currentPlayerId && p.Value.Location == currentPlayerLocation).ToList();
            if (!otherPlayersInSameCity.Any())
            {
                return false;
            }

            // If a researcher is in this city, and has at least one city card, then knowledge sharing is possible
            if ( (currentPlayerRole == PlayerRole.Researcher && currentPlayerCityCards.Any())
                || (otherPlayersInSameCity.Any(p => p.Value.PlayerRole == PlayerRole.Researcher && p.Value.PlayerHand.Any(c => c.PlayerCardType == PlayerCardType.City))))
            {
                return true;
            }

            // If any player in this city, has the city card of this city, then knowledge sharing is possible
            if (currentPlayerCityCards.Any(c => (City)c.Value == currentPlayerLocation))
            {
                return true;
            }
            if (otherPlayersInSameCity.Any(p => 
                p.Value.PlayerHand.Any(c => c.PlayerCardType == PlayerCardType.City && (City)c.Value == currentPlayerLocation)))
            {
                return true;
            }

            return false;
        }

        // TODO
        public KnowledgeShare GetSuggestedKnowledgeShare(int currentPlayerId, IPandemicState state)
        {
            return null;
        }
    }
}
