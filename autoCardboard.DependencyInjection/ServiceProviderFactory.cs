using System;
using autoCardboard.Common;
using autoCardboard.Common.Hubs;
using autoCardboard.ForSale;
using autoCardboard.Infrastructure;
using autoCardboard.Messaging;
using autoCardboard.Pandemic;
using autoCardboard.Pandemic.State;
using Microsoft.Extensions.DependencyInjection;

namespace autoCardboard.DependencyInjection
{
    public class ServiceProviderFactory
    {
        public static IServiceProvider GetServiceProvider()
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton<ICardboardLogger, CardboardLogger>()
                .AddSingleton<IGameHub, GameHub>()
                .AddSingleton<IMessageSender,MessageSender>()
                .AddScoped<IForSaleGameState, ForSaleGameState>()
                .AddScoped<IGame<IForSaleGameState, IForSaleGameTurn>, ForSaleGame>()
                .AddScoped<IPlayerFactory<IForSaleGameTurn>, ForSalePlayerFactory>()
                .AddScoped<IPandemicStateEditor, PandemicStateEditor>()

                .AddScoped<IPandemicState, PandemicState>()
                .AddScoped<IGame<IPandemicState, IPandemicTurn>, PandemicGame>()
                .AddScoped<IPlayerFactory<IPandemicTurn>, Pandemic.PandemicPlayerFactory>()
                .AddScoped<IPandemicTurnValidator, PandemicTurnValidator>()

                .BuildServiceProvider();

            return serviceProvider;
        }
    }
}
