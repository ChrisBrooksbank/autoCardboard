using autoCardboard.Common;
using autoCardboard.ForSale;
using autoCardboard.Pandemic;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace autoCardboard.Infrastructure
{
    public class ServiceProviderFactory
    {
        public static IServiceProvider GetServiceProvider()
        {
            var serviceProvider = new ServiceCollection()
                .AddScoped<IForSaleGameState, ForSaleGameState>()
                .AddScoped<IGame<IForSaleGameState, IForSaleGameTurn>, ForSaleGame>()
                .AddScoped<IPlayerFactory<IForSaleGameTurn>, ForSalePlayerFactory>()

                .AddScoped<IPandemicGameState, PandemicGameState>()
                .AddScoped<IGame<IPandemicGameState, IPandemicTurn>, PandemicGame>()
                .AddScoped<IPlayerFactory<IPandemicTurn>, Pandemic.PandemicPlayerFactory>()
                .AddScoped<IPandemicTurnValidator, PandemicTurnValidator>()

                .BuildServiceProvider();

            return serviceProvider;
        }
    }
}
