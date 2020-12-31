using autoCardboard.Common.Domain.Interfaces;
using System.Collections.Generic;

namespace autoCardboard.ForSale.Domain
{
    public class ForSalePlayerState
    {
        public List<ICard> PropertyCards { get; set; }
        public int CoinsBid { get; set; }
        public int CoinBalance { get; set; }
    }
}
