﻿using autoCardboard.Common.Domain;
using autoCardboard.Common.Domain.Interfaces;
using System.Collections.Generic;

namespace autoCardboard.ForSale.Domain
{
    public class PlayerFactory : IPlayerFactory<ForSaleGameTurn>
    {
        public IEnumerable<IPlayer<ForSaleGameTurn>> CreatePlayers(int playerCount)
        {
            List<IPlayer<ForSaleGameTurn>> players = new List<IPlayer<ForSaleGameTurn>>();
            for (int player = 1; player <= playerCount; player++)
            {
                var newPlayer = new ForSalePlayer
                {
                    Id = player,
                    Name = player.ToString()
                    // TODO
                    //State = new Dictionary<string, object>
                    //{
                    //    ["PropertyCards"] = new List<ICard>(),
                    //    ["CoinBalance"] = 16,
                    //    ["CoinsBid"] = 0,
                    //    ["LastAction"] = ""
                    //}
                };
                players.Add(newPlayer); ;
            }

            return players;
        }
    }
}
