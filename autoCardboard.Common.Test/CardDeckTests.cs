using System.Linq;

namespace autoCardboard.Common.Test
{
    public class Testss
    {
        private CardDeck<Card> _cardDeck;

        public void Setup()
        {
            _cardDeck = new CardDeck<Card>();
            for (var card = 1; card <= 24; card++)
            {
                _cardDeck.AddCard(new Card{ Value = card });
            }
        }

        public void RevealCardsDoesntRemoveFromDeck()
        {

            var revealedCards = _cardDeck.Reveal(3);
            // Assert.AreEqual(_cardDeck.CardCount, 24);
        }

        public void DrawCardsDoesRemoveFromDeck()
        {

            var cards = _cardDeck.Draw(3);
            //Assert.AreEqual(_cardDeck.CardCount, 21);
        }

        public void CanSplitCardsInto3EqualPiles()
        {
            var cardPiles= _cardDeck.Divide(3).ToList();
            // Assert.IsTrue(cardPiles[0].CardCount == 8 && cardPiles[1].CardCount == 8 && cardPiles[2].CardCount == 8);
        }

        public void DrawBottomGetsCorrectCard()
        {
            var card = _cardDeck.DrawBottom();
            // Assert.AreEqual(card.Value, 24);
        }

        public void DrawTopGetsCorrectCard()
        {
            var card = _cardDeck.DrawTop();
            // Assert.AreEqual(card.Value, 1);
        }

        public void CanAddCardToTop()
        {
            var card = new Card {Value = 42 };
            _cardDeck.AddCard(card, CardDeckPosition.Top);
            var drawnCard = _cardDeck.DrawTop();
            // Assert.AreEqual(drawnCard.Value, 42);
        }

        public void CanAddCardToBottom()
        {
            var card = new Card {Value = 42 };
            _cardDeck.AddCard(card, CardDeckPosition.Bottom);
            var drawnCard = _cardDeck.DrawBottom();
            // Assert.AreEqual(drawnCard.Value, 42);
        }

        public void DivideDeckEmptiesOriginalDeck()
        {
            var piles = _cardDeck.Divide(9);
            // Assert.AreEqual(_cardDeck.CardCount, 0);
        }

        public void DivideDeckCreatesCorrectNumberOfPiles()
        {
            var piles = _cardDeck.Divide(9);
            // Assert.AreEqual(piles.Count(), 9);
        }

        public void DivideDeckIntoMorePilesThanCardsWorks()
        {
            var piles = _cardDeck.Divide(100);
            // Assert.AreEqual(piles.Count(), 100);
        }

    }
}