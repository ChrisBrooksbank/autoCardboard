﻿using autoCardboard.Common;
using System.Collections.Generic;

namespace autoCardboard.Pandemic
{
    public class PandemicPlayerFactory : IPlayerFactory<IPandemicTurn>
    {
        public IEnumerable<IPlayer<IPandemicTurn>> CreatePlayers(PlayerConfiguration playerConfiguration)
        {
            List<IPlayer<IPandemicTurn>> players = new List<IPlayer<IPandemicTurn>>();
            for (int player = 1; player <= playerConfiguration.PlayerCount; player++)
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
