using System;
using System.Linq;
using autoCardboard.Common.Domain;

namespace autoCardboard.Pandemic.Domain
{
    public class PlayerDeck: CardDeck<PandemicPlayerCard>
    {
        public void Setup(int pandemicCardCount)
        {
            foreach (var city in Enum.GetValues(typeof(City)))
            {
                AddCard(new PandemicPlayerCard{ Value = (int)city, Name = city.ToString(), PlayerCardType = PlayerCardType.City});
            }
            foreach (var eventCard in Enum.GetValues(typeof(EventCard)))
            {
                AddCard(new PandemicPlayerCard{ Value = (int)eventCard, Name = eventCard.ToString(), PlayerCardType = PlayerCardType.Event});
            }
            Shuffle();
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
