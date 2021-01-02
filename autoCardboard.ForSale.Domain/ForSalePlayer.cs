using autoCardboard.Common.Domain.Interfaces;
using System.Linq;

namespace autoCardboard.ForSale.Domain
{
    public class ForSalePlayer : IPlayer<ForSaleGameTurn>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Choose a turn based on turn.State and make it
        /// any changes made to turn.State are ignored
        /// </summary>
        /// <param name="turn"></param>
        public void GetTurn(ForSaleGameTurn turn)
        {
            // Simply bid the minimum amount if we can afford it, else pass
            var playerState = turn.State.PlayerStates[Id];
            var currentMaxBid = turn.State.PlayerStates.Max(p => p.Value.CoinsBid);
            var minimumNextBid = currentMaxBid + 1;

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
