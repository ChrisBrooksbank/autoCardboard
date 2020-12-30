using autoCardboard.Common.Domain.Interfaces;
using System;
using System.Collections.Generic;

namespace autoCardboard.ForSale.Domain
{
    public class ForSalePlayer : IPlayer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string,object> State { get; set; }

        public Dictionary<string, object> TakeTurn(Dictionary<string, object> gameState)
        {
            var propertiesBiddingOn = gameState["PropertyCardsOnTable"] as IEnumerable<ICard>;
            var currentBidByAnyPlayer = (int)gameState["CurrentBid"];
            var coinBalance = (int)State["CoinBalance"];
            var coinsBid = (int)State["CoinsBid"];

            var minimumNextBid = currentBidByAnyPlayer + 1;

            if (minimumNextBid <= coinBalance)
            {
                var newBid = minimumNextBid;
                var newCoinBalance = coinBalance + coinsBid - newBid;
                State["CoinBalance"] = newCoinBalance;
                State["CoinsBid"] = newBid;
                State["LastAction"] = "bid";
                gameState["CurrentBid"] = newBid;
                Console.WriteLine($"Player {Id} bids {newBid} reducing their coin balance to {newCoinBalance}");
            }
            else
            {
                State["LastAction"] = "pass";
                Console.WriteLine($"Player {Id} passes");
            }

            return gameState;
        }
    }
}
