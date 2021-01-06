using System;
using System.Linq;
using autoCardboard.Common.Domain;

namespace autoCardboard.Pandemic.Domain
{
    [Serializable]
    public class PlayerDeck: CardDeck<PandemicPlayerCard>
    {
        public void Setup(int pandemicCardCount)
        {
            // Add city cards
            foreach (var city in Enum.GetValues(typeof(City)))
            {
                AddCard(new PandemicPlayerCard{ Value = (int)city, Name = city.ToString(), PlayerCardType = PlayerCardType.City});
            }

            // Add event cards
            foreach (var eventCard in Enum.GetValues(typeof(EventCard)))
            {
                AddCard(new PandemicPlayerCard{ Value = (int)eventCard, Name = eventCard.ToString(), PlayerCardType = PlayerCardType.Event});
            }

            Shuffle();

            // TODO each player will want four cards, they will want them before Epidemic get shuffled in

            // Divide shuffled deck up into piles, add a Epidemic card to each pile, shuffle each pile, reassemble deck
            var playerDeckCardPiles = Divide(pandemicCardCount).ToList();
            foreach (var cardDeck in playerDeckCardPiles)
            {
                cardDeck.AddCard( new PandemicPlayerCard{ PlayerCardType = PlayerCardType.Epidemic, Name = "Epidemic", Value = 0});
                cardDeck.Shuffle();
            }

            cards.Clear();
            Add(playerDeckCardPiles);
        }
      
    }
}
