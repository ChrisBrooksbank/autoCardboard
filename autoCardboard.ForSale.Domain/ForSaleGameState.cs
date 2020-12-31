using autoCardboard.Common.Domain;
using autoCardboard.Common.Domain.Interfaces;
using System.Collections.Generic;

namespace autoCardboard.ForSale.Domain
{
    public class ForSaleGameState: IGameState
    {
        public CardDeck PropertyDeck { get; set; }
        public IEnumerable<ICard> PropertyCardsOnTable { get; set; }
        public int CurrentBid { get; set; }
    }
}
