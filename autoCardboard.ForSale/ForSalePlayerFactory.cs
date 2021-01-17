using autoCardboard.Common;
using System.Collections.Generic;

namespace autoCardboard.ForSale
{
    public class ForSalePlayerFactory : IPlayerFactory<IForSaleGameTurn>
    {
        public IEnumerable<IPlayer<IForSaleGameTurn>> CreatePlayers(int playerCount)
        {
            var players = new List<IPlayer<IForSaleGameTurn>>();
            for (int player = 1; player <= playerCount; player++)
            {
                var newPlayer = new ForSalePlayer
                {
                    Id = player,
                    Name = player.ToString()
                };
                players.Add(newPlayer); ;
            }

            return players;
        }
    }
}
