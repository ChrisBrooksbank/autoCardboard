using System.Collections.Generic;
using autoCardboard.Common.Domain;
using autoCardboard.Common.Domain.Interfaces;
using autoCardboard.ForSale.Domain;
using autoCardboard.ForSale.Domain.Interfaces;

namespace autoCardboard.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var forSaleGame = SetupForSaleGame(5);
            forSaleGame.Play();
        }

        static IGame SetupForSaleGame(int playerCount)
        {
            var game = new ForSaleGame();

            // TODO move this logic to a player factory, in ForSale namespace
            List<IPlayer> players = new List<IPlayer>();
            for (int player = 1; player <= playerCount; player++)
            {
                players.Add(new ForSalePlayer
                {
                    Id = player,
                    Name = player.ToString(),
                    State = new Dictionary<string, object>
                    {
                      ["PropertyCards"] = new List<Card>(),
                      ["ThousandDollarCoinCount"] = playerCount < 5 ? 16 : 12
                    }
                }); ;
            }

            game.Setup(players);
            return game;
        }
    }
}
