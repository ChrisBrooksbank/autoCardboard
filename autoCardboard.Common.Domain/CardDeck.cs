using System;
using System.Collections.Generic;
using System.Linq;
using autoCardboard.Common.Domain.Interfaces;

namespace autoCardboard.Common.Domain
{
    [Serializable]
    public class CardDeck<TCardType>: ICardDeck<TCardType> where TCardType: ICard
    {
        private static Random r = new Random();
        protected List<TCardType> cards = new List<TCardType>();

        public bool Empty => !cards.Any();
        public int CardCount => cards.Count;
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

        public IEnumerable<TCardType> Draw(int count)
        {
            count = count > cards.Count ? cards.Count : count;
            var drawnCards = cards.GetRange(0, count);
            cards.RemoveRange(0,count);
            return drawnCards;
        }

        public IEnumerable<TCardType> Reveal(int count)
        {
            count = count > cards.Count ? cards.Count : count;
            var revealedCards = cards.GetRange(0, count);
            return revealedCards;
        }

        public void AddCard(TCardType card)
        {
            cards.Add(card);
        }

        public void AddCards(IEnumerable<TCardType> cards)
        {
            foreach(var card in cards)
            {
                AddCard(card);
            }
        }

        public TCardType DrawTop()
        {
            if (cards.Count == 0)
            {
                return default(TCardType);
            }

            var card = cards[0];
            cards.Remove(card);
            return card;
        }

        public TCardType RevealTop()
        {
            return cards.Count == 0 ? default(TCardType) : cards[0];
        }

        public IEnumerable<CardDeck<TCardType>> Divide(int count)
        {
            var piles = new List<CardDeck<TCardType>>();

            var pileNumber = 1;
            for(pileNumber = 1; pileNumber <=count;pileNumber++)
            {
                piles.Add(new CardDeck<TCardType>());
            }

            pileNumber = 1;
            foreach (var card in cards)
            {
                piles[pileNumber++ - 1].AddCard(card);

                if (pileNumber > count)
                {
                    pileNumber = 1;
                }
            }

            cards.Clear();

            return piles;
        }

        // TODO bug we get repeated pandemic cards
        public void Add(IEnumerable<CardDeck<TCardType>> decks)
        {
            var piles = decks.ToList();
            var pileCount = piles.Count();

            for (var pileNumber = pileCount; pileCount > 0; pileCount--)
            {
                Add(piles[pileNumber-1]);
            }
        }

        public void Add(CardDeck<TCardType> deck)
        {
            foreach (var card in deck.cards)
            {
                cards.Add(card);
            }

        }
    }
}
