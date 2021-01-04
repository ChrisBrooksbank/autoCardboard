using System.Linq;
using autoCardboard.ForSale.Domain;
using autoCardboard.Pandemic.Domain;

namespace autoCardboard.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = new ForSaleGame();
            var playerFactory = new ForSale.Domain.PlayerFactory();
            var players = playerFactory.CreatePlayers(5).ToList();

            for(var gameNumber = 1; gameNumber < 200; gameNumber++)
            {
                game.Play(players);
            }
        }

    }
}
