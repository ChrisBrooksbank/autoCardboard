using System.Collections.Generic;
using autoCardboard.Common;
using autoCardboard.Infrastructure;
using autoCardboard.Messaging;
using autoCardBoard.Pandemic.Bots;
using autoCardboard.Pandemic.State;
using autoCardboard.Pandemic.TurnState;
using Microsoft.Extensions.Caching.Memory;

namespace autoCardboard.Pandemic.Game
{
    public class PandemicPlayerFactory : IPlayerFactory<IPandemicTurn>
    {
        private readonly ICardboardLogger _log;
        private readonly IMemoryCache _memoryCache;

        public PandemicPlayerFactory(ICardboardLogger log, IMemoryCache memoryCache)
        {
            _log = log;
            _memoryCache = memoryCache;
        }

        public IEnumerable<IPlayer<IPandemicTurn>> CreatePlayers(PlayerConfiguration playerConfiguration)
        {
            List<IPlayer<IPandemicTurn>> players = new List<IPlayer<IPandemicTurn>>();
            for (int player = 1; player <= playerConfiguration.PlayerCount; player++)
            {
                var newPlayer = new PandemicBotStandard(_log, new RouteHelper(_memoryCache, new MapNodeFactory()), new MessageSender(), new PlayerDeckHelper())
                {
                    Id = player,
                    Name = player.ToString()
                };
                players.Add(newPlayer); ;
            }

            return players;
        }
    }
}
