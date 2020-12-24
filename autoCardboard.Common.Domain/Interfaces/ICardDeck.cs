using System.Collections.Generic;

namespace autoCardboard.Common.Domain.Interfaces
{
    public interface ICardDeck
    {
        void Shuffle();
        IEnumerable<ICard> Draw(int count);
        IEnumerable<ICard> Reveal(int count);
    }
}
