using autoCardboard.Common.Domain.Interfaces;
using System.Collections.Generic;

namespace autoCardboard.Pandemic.Domain
{
    public class PlayerFactory : IPlayerFactory<PandemicGameTurn>
    {
        public IEnumerable<IPlayer<PandemicGameTurn>> CreatePlayers(int playerCount)
        {
            throw new System.NotImplementedException();
        }
    }
}
