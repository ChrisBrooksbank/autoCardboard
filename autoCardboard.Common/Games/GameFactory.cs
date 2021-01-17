using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace autoCardboard.Common
{
    public class GameFactory
    {
        public static IGame<TGameState, TGameTurn> CreateGame<TGameState, TGameTurn>(IServiceProvider serviceProvider, PlayerConfiguration playerConfiguration)
            where TGameState : IGameState where TGameTurn : IGameTurn
        {
            var game = serviceProvider.GetService<IGame<TGameState, TGameTurn>>();
            var playerFactory = serviceProvider.GetService<IPlayerFactory<TGameTurn>>();
            var players = playerFactory.CreatePlayers(playerConfiguration).ToList();
            game.Players = players;
            return game;
        }
    }
}
