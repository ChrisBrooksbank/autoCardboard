using System;
using autoCardboard.Common;
using autoCardboard.ForSale;
using autoCardboard.Infrastructure;
using autoCardboard.Pandemic;
using Microsoft.Extensions.DependencyInjection;

namespace autoCardboard.DependencyInjection
{
    public class ServiceProviderFactory
    {
        public static IServiceProvider GetServiceProvider()
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton<ICardboardLogger, CardboardLogger>()
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
