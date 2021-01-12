using autoCardboard.Common.Domain.Interfaces;
using System.Collections.Generic;

namespace autoCardboard.Pandemic.Domain
{
    public class PlayerFactory : IPlayerFactory<PandemicTurn>
    {
        public IEnumerable<IPlayer<PandemicTurn>> CreatePlayers(int playerCount)
        {
            List<IPlayer<PandemicTurn>> players = new List<IPlayer<PandemicTurn>>();
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
