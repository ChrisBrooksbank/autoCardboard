using System.Linq;
using autoCardboard.Common.Domain.Interfaces;
using autoCardboard.ForSale.Domain;
using autoCardboard.Pandemic.Domain;
using autoCardboard.Pandemic.Domain.Game;
using autoCardboard.Pandemic.Domain.PlayerTurns;
using autoCardboard.Pandemic.Domain.State;
using Microsoft.Extensions.DependencyInjection;
using forSalePlayerFactory = autoCardboard.ForSale.Domain.PlayerFactory;
using pandemicPlayerFactory = autoCardboard.Pandemic.Domain.PlayerFactory;

namespace autoCardboard.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddScoped<IPandemicGameState, PandemicGameState>()
                .AddScoped<IGame<IPandemicGameState, IPandemicTurn>, PandemicGame>()
                .AddScoped<IPlayerFactory<IPandemicTurn>, pandemicPlayerFactory>()
                .BuildServiceProvider();

            var pandemicGame = serviceProvider.GetService<IGame<IPandemicGameState, IPandemicTurn>>();
            var pandemicPlayerFactory = serviceProvider.GetService<IPlayerFactory<IPandemicTurn>>();

            var pandemicPlayers = pandemicPlayerFactory.CreatePlayers(2).ToList();
            pandemicGame.Play(pandemicPlayers);
          
            // TODO use DI
            var game = new ForSaleGame();
            var forSalePlayerFactory = new forSalePlayerFactory();
            var players = forSalePlayerFactory.CreatePlayers(5).ToList();
            game.Play(players);
        }

    }
}
