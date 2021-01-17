using System;
using System.Linq;
using autoCardboard.Common.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace autoCardboard.Common.Domain.Games
{
    public class GameFactory
    {
        public static IGame<TGameState, TGameTurn> CreateGame<TGameState, TGameTurn>(IServiceProvider serviceProvider, int playerCount)
            where TGameState : IGameState where TGameTurn : IGameTurn
        {
            var game = serviceProvider.GetService<IGame<TGameState, TGameTurn>>();
            var playerFactory = serviceProvider.GetService<IPlayerFactory<TGameTurn>>();
            var players = playerFactory.CreatePlayers(playerCount).ToList();
            game.Players = players;
            return game;
        }
    }
}
