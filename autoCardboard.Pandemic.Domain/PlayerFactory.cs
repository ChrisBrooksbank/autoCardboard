using autoCardboard.Common.Domain.Interfaces;
using System.Collections.Generic;

namespace autoCardboard.Pandemic.Domain
{
    public class PlayerFactory : IPlayerFactory<PandemicGameTurn>
    {
        public IEnumerable<IPlayer<PandemicGameTurn>> CreatePlayers(int playerCount)
        {
            List<IPlayer<PandemicGameTurn>> players = new List<IPlayer<PandemicGameTurn>>();
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
