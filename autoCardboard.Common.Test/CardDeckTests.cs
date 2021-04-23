using System.Linq;
using Xunit;

namespace autoCardboard.Common.Test
{
    public class Testss
    {
        private CardDeck<Card> _cardDeck;

        private void Setup()
        {
            _cardDeck = new CardDeck<Card>();
            for (var card = 1; card <= 24; card++)
            {
                _cardDeck.AddCard(new Card { Value = card });
            }
        }

        [Fact]
        public void RevealCardsDoesntRemoveFromDeck()
        {
            Setup();
            var revealedCards = _cardDeck.Reveal(3);
            Assert.Equal(24, _cardDeck.CardCount);
        }

        [Fact]
        public void DrawCardsDoesRemoveFromDeck()
        {
            Setup();
            var cards = _cardDeck.Draw(3);
            Assert.Equal(21, _cardDeck.CardCount);
        }

        [Fact]
        public void CanSplitCardsInto3EqualPiles()
        {
            Setup();
            var cardPiles = _cardDeck.Divide(3).ToList();
            Assert.True(cardPiles[0].CardCount == 8 && cardPiles[1].CardCount == 8 && cardPiles[2].CardCount == 8);
        }

        [Fact]
        public void DrawBottomGetsCorrectCard()
        {
            Setup();
            var card = _cardDeck.DrawBottom();
            Assert.Equal(24, card.Value);
        }

        [Fact]
        public void DrawTopGetsCorrectCard()
        {
            Setup();
            var card = _cardDeck.DrawTop();
            Assert.Equal(1,card.Value);
        }
        
        [Fact]
        public void CanAddCardToTop()
        {
            Setup();
            var card = new Card { Value = 42 };
            _cardDeck.AddCard(card, CardDeckPosition.Top);
            var drawnCard = _cardDeck.DrawTop();
            Assert.Equal(42,drawnCard.Value);
        }

        [Fact]
        public void CanAddCardToBottom()
        {
            Setup();
            var card = new Card { Value = 42 };
            _cardDeck.AddCard(card, CardDeckPosition.Bottom);
            var drawnCard = _cardDeck.DrawBottom();
            Assert.Equal(42,drawnCard.Value);
        }

        [Fact]
        public void DivideDeckEmptiesOriginalDeck()
        {
            Setup();
            var piles = _cardDeck.Divide(9);
            Assert.Equal(0, _cardDeck.CardCount);
        }

        [Fact]
        public void DivideDeckCreatesCorrectNumberOfPiles()
        {
            Setup();
            var piles = _cardDeck.Divide(9);
            Assert.Empty(piles);
        }

        [Fact]
        public void DivideDeckIntoMorePilesThanCardsWorks()
        {
            Setup();
            var piles = _cardDeck.Divide(100);
            Assert.Equal(100, piles.Count());
        }

    }
}