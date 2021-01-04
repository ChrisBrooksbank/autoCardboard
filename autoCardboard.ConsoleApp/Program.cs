using System.Linq;
using autoCardboard.ForSale.Domain;
using autoCardboard.Pandemic.Domain;

namespace autoCardboard.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var pandemicMap = new PandemicMap();

            var game = new ForSaleGame();
            var playerFactory = new PlayerFactory();
            var players = playerFactory.CreatePlayers(5).ToList();

            for(var gameNumber = 1; gameNumber < 2; gameNumber++)
            {
                game.Play(players);
            }
        }

    }
}
