using autoCardboard.Common.Domain.Interfaces;
using System.Collections.Generic;

namespace autoCardboard.ForSale.Domain.Interfaces
{
    public class PlayerState: IPlayerState
    {
        public IEnumerable<ICard> PropertyCards { get; set; }
        public int OneThousandDollarCoinCount { get; set; }
        public int TwoThousandDollarCoinCount { get; set; }
    }
}
