using System.Collections.Generic;

namespace autoCardboard.Common.Domain.Interfaces
{
    public interface ICardShuffler
    {
        IEnumerable<ICard> Shuffle(IEnumerable<ICard> cards);
    }
}
