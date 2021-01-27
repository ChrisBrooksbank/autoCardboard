using autoCardboard.Common;
using autoCardboard.Infrastructure;
using System.Collections.Generic;
using autoCardboard.Messaging;

namespace autoCardboard.Pandemic
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
                var newPlayer = new PandemicPlayer(_log,_pandemicStateEditor, new MessageSender())
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
