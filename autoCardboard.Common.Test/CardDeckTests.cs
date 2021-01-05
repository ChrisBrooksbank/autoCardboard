using System.Linq;
using autoCardboard.Common.Domain;
using NUnit.Framework;

namespace autoCardboard.Common.Test
{
    public class Tests
    {
        private CardDeck<Card> _cardDeck;

        [SetUp]
        public void Setup()
        {
            _cardDeck = new CardDeck<Card>();
            for (var card = 1; card <= 24; card++)
            {
                _cardDeck.AddCard(new Card{ Value = card });
            }
        }

        [Test]
        public void RevealCardsDoesntRemoveFromDeck()
        {

            var revealedCards = _cardDeck.Reveal(3);
            Assert.AreEqual(_cardDeck.CardCount, 24);
        }

        [Test]
        public void DrawCardsDoesRemoveFromDeck()
        {

            var cards = _cardDeck.Draw(3);
            Assert.AreEqual(_cardDeck.CardCount, 21);
        }

        [Test]
        public void CanSplitCardsInto3EqualPiles()
        {
            var cardPiles= _cardDeck.Divide(3).ToList();
            Assert.IsTrue(cardPiles[0].CardCount == 8 && cardPiles[1].CardCount == 8 && cardPiles[2].CardCount == 8);
        }
    }
}