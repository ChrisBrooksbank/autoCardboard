using System;
using autoCardboard.Common;
using autoCardboard.Common.Hubs;
using autoCardboard.ForSale;
using autoCardboard.Infrastructure;
using autoCardboard.Messaging;
using autoCardBoard.Pandemic.Bots;
using autoCardboard.Pandemic.Game;
using autoCardboard.Pandemic.State;
using autoCardboard.Pandemic.TurnState;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace autoCardboard.DependencyInjection
{
    public class ServiceProviderFactory
    {
        public static IServiceProvider GetServiceProvider()
        {
            var memoryCache = new MemoryCache(new MemoryCacheOptions());

            var serviceProvider = new ServiceCollection()
                .AddSingleton<ICardboardLogger, CardboardLogger>()
                .AddSingleton<IGameHub, GameHub>()
                .AddSingleton<IMessageSender,MessageSender>()
                .AddSingleton<IMemoryCache>(memoryCache)
                .AddSingleton<IRouteHelper, RouteHelper>()
                .AddSingleton<IEventCardHelper, EventCardHelper>()
                .AddSingleton<IHandManagementHelper, HandManagementHelper>()
                .AddSingleton<IResearchStationHelper, ResearchStationHelper>()
                .AddSingleton<IKnowledgeShareHelper, KnowledgeShareHelper>()
                .AddSingleton<IMapNodeFactory, MapNodeFactory>()
                .AddScoped<IForSaleGameState, ForSaleGameState>()
                .AddScoped<IGame<IForSaleGameState, IForSaleGameTurn>, ForSaleGame>()
                .AddScoped<IPlayerFactory<IForSaleGameTurn>, ForSalePlayerFactory>()
                .AddScoped<IPandemicStateEditor, PandemicStateEditor>()

                .AddScoped<IPandemicState, PandemicState>()
                .AddScoped<IGame<IPandemicState, IPandemicTurn>, PandemicGame>()
                .AddScoped<IPlayerFactory<IPandemicTurn>, PandemicPlayerFactory>()
                .AddScoped<IPandemicActionValidator, PandemicActionValidator>()

                .BuildServiceProvider();

            return serviceProvider;
        }
    }
}
