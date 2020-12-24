using autoCardboard.Common.Domain.Interfaces;
using autoCardboard.ForSale.Domain.Interfaces;
using System.Collections.Generic;

namespace autoCardboard.ForSale.Domain
{
    public class ForSaleGameState
    {
        public IEnumerable<ICard> PropertyDrawDeck { get; set; }
        public IEnumerable<ICard> PropertyDiscardDeck { get; set; }
        public IEnumerable<ICard> PropertyBidDeck { get; set; }

        public IEnumerable<IPlayer> Players { get; set; }

    }
}
