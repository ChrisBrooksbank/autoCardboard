using System.Linq;
using autoCardboard.ForSale.Domain;
using autoCardboard.Pandemic.Domain;
using forSalePlayerFactory = autoCardboard.ForSale.Domain.PlayerFactory;
using pandemicPlayerFactory = autoCardboard.Pandemic.Domain.PlayerFactory;

namespace autoCardboard.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var pandemicGame = new PandemicGame();
            var pandemicPlayerFactory = new pandemicPlayerFactory();
            var pandemicPlayers = pandemicPlayerFactory.CreatePlayers(2).ToList();
            pandemicGame.Play(pandemicPlayers);
          
            var game = new ForSaleGame();
            var forSalePlayerFactory = new forSalePlayerFactory();
            var players = forSalePlayerFactory.CreatePlayers(5).ToList();
            game.Play(players);
        }

    }
}
