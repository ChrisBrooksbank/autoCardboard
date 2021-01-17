using autoCardboard.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace autoCardboard.ForSale
{
    [Serializable]
    public class ForSalePlayerState
    {
        public List<ICard> PropertyCards { get; set; }
        public List<ICard> ChequeCards { get; set; }

        public int CoinsBid { get; set; }
        public ICard PropertySelling{ get; set; }

        public int CoinBalance { get; set; }

        public int TotalScore => CoinBalance + ChequeCards.Sum(c => c.Value);
    }
}
