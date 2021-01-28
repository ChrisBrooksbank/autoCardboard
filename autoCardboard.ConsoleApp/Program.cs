using autoCardboard.Common;
using autoCardboard.DependencyInjection;
using autoCardboard.ForSale;
using autoCardboard.Pandemic;
using autoCardboard.Pandemic.State;
using autoCardboard.Pandemic.TurnState;

namespace autoCardboard.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = ServiceProviderFactory.GetServiceProvider();
            var playerConfiguration = new PlayerConfiguration { PlayerCount = 2 };
            GameFactory.CreateGame<IPandemicState, IPandemicTurn>(serviceProvider, playerConfiguration).Play();
            GameFactory.CreateGame<IForSaleGameState, IForSaleGameTurn>(serviceProvider, playerConfiguration).Play();
        }
    }
}
