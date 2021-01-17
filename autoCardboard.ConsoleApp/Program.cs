using autoCardboard.Infrastructure;
using autoCardboard.Common;
using autoCardboard.ForSale;
using autoCardboard.Pandemic;

namespace autoCardboard.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = ServiceProviderFactory.GetServiceProvider();
            GameFactory.CreateGame<IPandemicGameState, IPandemicTurn>(serviceProvider, 2).Play();
            GameFactory.CreateGame<IForSaleGameState, IForSaleGameTurn>(serviceProvider, 2).Play();
        }
    }
}
