using System;
using System.Collections.Generic;
using System.Linq;
using autoCardboard.Common.Domain.Interfaces;

namespace autoCardboard.Common.Domain
{
    [Serializable]
    public class CardDeck: ICardDeck
    {
        private static Random r = new Random();
        protected List<ICard> cards = new List<ICard>();

        public bool Empty => !cards.Any();

        public void Shuffle()
        {
            for (int n = cards.Count - 1; n > 0; --n)
            {
                var k = r.Next(n+1);
                var temp = cards[n];
                cards[n] = cards[k];
                cards[k] = temp;
            }
        }

        public IEnumerable<ICard> Draw(int count)
        {
            count = count > cards.Count ? cards.Count : count;
            var drawnCards = cards.GetRange(0, count);
            cards.RemoveRange(0,count);
            return drawnCards;
        }

        public IEnumerable<ICard> Reveal(int count)
        {
            count = count > cards.Count ? cards.Count : count;
            var revealedCards = cards.GetRange(0, count);
            return revealedCards;
        }

        public void AddCard(ICard card)
        {
            cards.Add(card);
        }

        public void AddCards(IEnumerable<ICard> cards)
        {
            foreach(var card in cards)
            {
                AddCard(card);
            }
                   }
    }
}
