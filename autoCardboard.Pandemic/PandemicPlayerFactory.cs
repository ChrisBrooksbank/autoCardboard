using System.Collections.Generic;
using autoCardboard.Common;
using autoCardboard.Infrastructure;
using autoCardboard.Messaging;
using autoCardBoard.Pandemic.Bots;
using autoCardboard.Pandemic.State;
using autoCardboard.Pandemic.TurnState;

namespace autoCardboard.Pandemic.Game
{
    public class PandemicPlayerFactory : IPlayerFactory<IPandemicTurn>
    {
        private readonly ICardboardLogger _log;
        private readonly IPandemicStateEditor _pandemicStateEditor;

        public PandemicPlayerFactory(ICardboardLogger log, IPandemicStateEditor pandemicStateEditor)
        {
            _log = log;
            _pandemicStateEditor = pandemicStateEditor;
        }

        public IEnumerable<IPlayer<IPandemicTurn>> CreatePlayers(PlayerConfiguration playerConfiguration)
        {
            List<IPlayer<IPandemicTurn>> players = new List<IPlayer<IPandemicTurn>>();
            for (int player = 1; player <= playerConfiguration.PlayerCount; player++)
            {
                var newPlayer = new PandemicPlayer(_log, new RouteHelper(new MapNodeFactory()), new MessageSender())
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
