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
            for (var n = cards.Count - 1; n > 0; --n)
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

        public void AddCard(TCardType card, CardDeckPosition position = CardDeckPosition.Bottom)
        {
            if (position == CardDeckPosition.Bottom)
            {
                cards.Add(card);
            }
            else
            {
              cards.Insert(0, card);
            }
        }

        public void AddCards(IEnumerable<TCardType> cards, CardDeckPosition position = CardDeckPosition.Bottom)
        {
            foreach(var card in cards)
            {
                AddCard(card, position);
            }
        }

        public TCardType DrawBottom()
        {
            if (cards.Count == 0)
            {
                return default(TCardType);
            }

            var card = cards[cards.Count-1];
            cards.Remove(card);
            return card;
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

        public IEnumerable<CardDeck<TCardType>> Divide(int pileCount)
        {
            var piles = new List<CardDeck<TCardType>>();

            int pileNumber;
            for(pileNumber = 1; pileNumber <=pileCount;pileNumber++)
            {
                piles.Add(new CardDeck<TCardType>());
            }

            pileNumber = 1;
            foreach (var card in cards)
            {
                piles[pileNumber++ - 1].AddCard(card);

                if (pileNumber > pileCount)
                {
                    pileNumber = 1;
                }
            }

            cards.Clear();

            return piles;
        }

        public void Add(IEnumerable<CardDeck<TCardType>> decks)
        {
            decks = decks.Reverse().ToList();

            foreach (var deck in decks)
            {
                Add(deck);
            }
        }

        public void Add(CardDeck<TCardType> deck)
        {
            foreach (var card in deck.cards)
            {
                cards.Add(card);
            }
        }

        public void AddCardDeck(CardDeck<TCardType> cardDeck, CardDeckPosition position = CardDeckPosition.Bottom)
        {
            AddCards(cardDeck.cards, position);
            cardDeck.cards.Clear();
        }
    }
}
