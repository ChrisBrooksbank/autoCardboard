using System.Collections.Generic;
using autoCardboard.Common;

namespace autoCardboard.ForSale
{
    public interface IForSaleGameState: IGameState
    {
        CardDeck<Card> PropertyDeck { get; set; }
        CardDeck<Card> ChequeDeck { get; set; }
        IEnumerable<ICard> PropertyCardsOnTable { get; set; }
        IEnumerable<ICard> ChequesOnTable { get; set; }

        Dictionary<int,ForSalePlayerState> PlayerStates { get; set; }
    }
}
