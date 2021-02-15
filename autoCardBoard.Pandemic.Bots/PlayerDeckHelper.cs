using autoCardboard.Pandemic.State;
using System.Collections.Generic;
using System.Linq;
using autoCardboard.Infrastructure.Exceptions;

namespace autoCardBoard.Pandemic.Bots
{
    public class PlayerDeckHelper : IPlayerDeckHelper
    {
        public Dictionary<Disease, List<PandemicPlayerCard>> GetCityCardsByColour(IEnumerable<PandemicPlayerCard> cards)
        {
            var cityCardsByColour = new Dictionary<Disease, List<PandemicPlayerCard>>();
            if (cards == null)
            {
                return cityCardsByColour;
            }

            cityCardsByColour[Disease.Blue] = new List<PandemicPlayerCard>();
            cityCardsByColour[Disease.Red] = new List<PandemicPlayerCard>();
            cityCardsByColour[Disease.Black] = new List<PandemicPlayerCard>();
            cityCardsByColour[Disease.Yellow] = new List<PandemicPlayerCard>();

            foreach (var cityCard in cards.Where(c => c.PlayerCardType == PlayerCardType.City))
            {
                var city = (City) cityCard.Value;
                cityCardsByColour[city.GetDefaultDisease()].Add(cityCard);
            }

            return cityCardsByColour;
        }

        public IEnumerable<Disease> GetDiseasesCanCure(PlayerRole playerRole, IEnumerable<PandemicPlayerCard> cards)
        {
            var cardsNeededToCure = playerRole == PlayerRole.Scientist ? 4 : 5;
            return  GetCityCardsByColour(cards)
                .Where(cbd => cbd.Value.Count >= cardsNeededToCure)
                .Select(cbd => cbd.Key);
        }

        public PandemicPlayerCard GetWeakCard(IPandemicState state, PlayerRole playerRole, IEnumerable<PandemicPlayerCard> cards)
        {
            var cardsByColour = GetCityCardsByColour(cards);
            var playerCards = cards.ToList();

            // If we have a event card, we consider this weak
            // Mainly because the code doesnt yet support the playing of these
            // that logic will change !
            var eventCard = playerCards.FirstOrDefault( c => c.PlayerCardType == PlayerCardType.Event);
            if (eventCard != null)
            {
                return eventCard;
            }

            // If we have a city card for a cured or eradicated disease then we can return that as weak
            var diseaseStates = state.DiscoveredCures;
            foreach (var diseaseState in diseaseStates)
            {
                if (diseaseState.Value == DiseaseState.Cured || diseaseState.Value == DiseaseState.Eradicated)
                {
                    // if we have a card relating to a cured or eradicated disease, then is a weak card
                    // ( we are not taking into account, if card is useful to facilitate flights here )
                    if (cardsByColour[diseaseState.Key].Count > 0)
                    {
                        return cardsByColour[diseaseState.Key][0];
                    }
                }
            }

            // If we have a colour, where we have at least one city card, but its less than the max of another colour, thats weak
            var maxCount = cardsByColour.Max( c => c.Value.Count);
            foreach (var cardByColour in cardsByColour)
            {
                if (cardByColour.Value.Count > 0 && cardByColour.Value.Count < maxCount)
                {
                    return cardByColour.Value[0];
                }
            }

            // If we have got here, we simply discard the first card
            return playerCards[0];
        }

        public List<PandemicPlayerCard> GetCardsToDiscardToCure(IPandemicState state, Disease disease, PlayerRole playerRole, IEnumerable<PandemicPlayerCard> cards)
        {
            var cardCountToDiscard = playerRole == PlayerRole.Scientist ? 4 : 5;
            var candidateDiscards = GetCityCardsByColour(cards)[disease];

            if (cardCountToDiscard > candidateDiscards.Count)
            {
                throw new CardboardException($"Cant find enough cards to discard to cure disease {disease}");
            }
            
            return candidateDiscards.Take(cardCountToDiscard).ToList();
        }
    }
}
