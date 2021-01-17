using autoCardboard.Common;
using autoCardboard.ConsoleApp.Infrastructure;
using autoCardboard.ForSale.Domain;
using autoCardboard.Pandemic.Domain;

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
