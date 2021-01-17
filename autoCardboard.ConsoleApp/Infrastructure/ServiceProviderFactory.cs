using System;
using autoCardboard.Common.Domain.Interfaces;
using autoCardboard.ForSale.Domain;
using autoCardboard.Pandemic.Domain;
using autoCardboard.Pandemic.Domain.PlayerTurns;
using autoCardboard.Pandemic.Domain.State;
using Microsoft.Extensions.DependencyInjection;
using PlayerFactory = autoCardboard.ForSale.Domain.PlayerFactory;

namespace autoCardboard.ConsoleApp.Infrastructure
{
    public class ServiceProviderFactory
    {
        public static IServiceProvider GetServiceProvider()
        {
            var serviceProvider = new ServiceCollection()
                .AddScoped<IForSaleGameState, ForSaleGameState>()
                .AddScoped<IGame<IForSaleGameState, IForSaleGameTurn>, ForSaleGame>()
                .AddScoped<IPlayerFactory<IForSaleGameTurn>,PlayerFactory>()

                .AddScoped<IPandemicGameState, PandemicGameState>()
                .AddScoped<IGame<IPandemicGameState, IPandemicTurn>, PandemicGame>()
                .AddScoped<IPlayerFactory<IPandemicTurn>, Pandemic.Domain.PlayerFactory>()
                .AddScoped<IPandemicTurnValidator, PandemicTurnValidator>()

                .BuildServiceProvider();

            return serviceProvider;
        }
    }
}
