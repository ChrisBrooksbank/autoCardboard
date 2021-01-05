using System.Linq;
using autoCardboard.ForSale.Domain;
using autoCardboard.Pandemic.Domain;

namespace autoCardboard.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var pandemicGame = new PandemicGame();
            pandemicGame.Play(null);
          
            var game = new ForSaleGame();
            var playerFactory = new ForSale.Domain.PlayerFactory();
            var players = playerFactory.CreatePlayers(5).ToList();

            for(var gameNumber = 1; gameNumber < 12; gameNumber++)
            {
                game.Play(players);
            }
        }

    }
}
