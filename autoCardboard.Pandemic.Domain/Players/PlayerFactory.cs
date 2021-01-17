using autoCardboard.Common;
using System.Collections.Generic;

namespace autoCardboard.Pandemic.Domain
{
    public class PlayerFactory : IPlayerFactory<IPandemicTurn>
    {
        public IEnumerable<IPlayer<IPandemicTurn>> CreatePlayers(int playerCount)
        {
            List<IPlayer<IPandemicTurn>> players = new List<IPlayer<IPandemicTurn>>();
            for (int player = 1; player <= playerCount; player++)
            {
                var newPlayer = new PandemicPlayer()
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
