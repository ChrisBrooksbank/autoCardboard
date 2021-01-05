using System.Collections.Generic;

namespace autoCardboard.Common.Domain.Interfaces
{
    public interface ICardDeck<TCardType> where TCardType: ICard
    {
        void Shuffle();
        void AddCard(TCardType card);
        void AddCards(IEnumerable<TCardType> cards);
        TCardType DrawTop();
        IEnumerable<TCardType> Draw(int count);
        TCardType RevealTop();
        IEnumerable<TCardType> Reveal(int count);
        IEnumerable<CardDeck<TCardType>> Divide(int pileCount);
        void Add(IEnumerable<CardDeck<TCardType>> decks);
        void Add(CardDeck<TCardType> deck);
    }
}
