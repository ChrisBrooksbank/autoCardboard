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
            var game = new ForSaleGame();
            var playerFactory = new PlayerFactory();
            var players = playerFactory.CreatePlayers(5);
            game.Setup(players);
            game.Play();
        }

    }
}
