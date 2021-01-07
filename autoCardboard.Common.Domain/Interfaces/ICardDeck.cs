using System.Collections.Generic;
using autoCardboard.Common.Domain.Cards;

namespace autoCardboard.Common.Domain.Interfaces
{
    public interface ICardDeck<TCardType> where TCardType: ICard
    {
        void Shuffle();
        void AddCard(TCardType card, CardDeckPosition position = CardDeckPosition.Bottom);
        void AddCards(IEnumerable<TCardType> cards, CardDeckPosition position = CardDeckPosition.Bottom);
        void AddCardDeck(CardDeck<TCardType> cardDeck, CardDeckPosition position = CardDeckPosition.Bottom);
        TCardType DrawTop();
        TCardType DrawBottom();
        IEnumerable<TCardType> Draw(int count);
        TCardType RevealTop();
        IEnumerable<TCardType> Reveal(int count);
        IEnumerable<CardDeck<TCardType>> Divide(int pileCount);
        void Add(IEnumerable<CardDeck<TCardType>> decks);
    }
}
