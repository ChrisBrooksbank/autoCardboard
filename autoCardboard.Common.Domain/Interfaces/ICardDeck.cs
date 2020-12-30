using System.Collections.Generic;

namespace autoCardboard.Common.Domain.Interfaces
{
    public interface ICardDeck
    {
        void Shuffle();
        void AddCard(ICard card);
        void AddCards(IEnumerable<ICard> cards);
        IEnumerable<ICard> Draw(int count);
        IEnumerable<ICard> Reveal(int count);
    }
}
