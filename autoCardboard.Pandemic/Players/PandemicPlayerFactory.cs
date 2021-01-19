using autoCardboard.Common;
using autoCardboard.Infrastructure;
using System.Collections.Generic;

namespace autoCardboard.Pandemic
{
    public class PandemicPlayerFactory : IPlayerFactory<IPandemicTurn>
    {
        private readonly ICardboardLogger _log;

        public PandemicPlayerFactory(ICardboardLogger log)
        {
            _log = log;
        }

        public IEnumerable<IPlayer<IPandemicTurn>> CreatePlayers(PlayerConfiguration playerConfiguration)
        {
            List<IPlayer<IPandemicTurn>> players = new List<IPlayer<IPandemicTurn>>();
            for (int player = 1; player <= playerConfiguration.PlayerCount; player++)
            {
                var newPlayer = new PandemicPlayer(_log)
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
