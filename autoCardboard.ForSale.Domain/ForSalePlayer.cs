using autoCardboard.Common.Domain.Interfaces;
using System;
using System.Collections.Generic;

namespace autoCardboard.ForSale.Domain
{
    public class ForSalePlayer : IPlayer<ForSaleGameTurn>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public void GetTurn(ForSaleGameTurn turn)
        {
            // Simply bid the minimum amount if we can afford it, else pass
            var playerState = turn.State.PlayerStates[Id];
            var minimumNextBid = turn.State.CurrentBid + 1;

            if (minimumNextBid <= playerState.CoinBalance)
            {
                turn.Bid(minimumNextBid);
            }
            else
            {
                turn.Pass();
            }
        }

    }
}
