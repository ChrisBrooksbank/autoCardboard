using System;
using System.Collections.Generic;
using autoCardboard.Common.Domain.Interfaces;

namespace autoCardboard.Common.Domain
{
    public class CardDeck: ICardDeck
    {
        private static Random r = new Random();
        protected List<ICard> cards;

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
            // TODO check bounds, remove drawn cards from this deck
            var drawnCards = cards.GetRange(0, count);
            return drawnCards;
        }
    }
}
