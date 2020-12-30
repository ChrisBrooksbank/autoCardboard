using System.Collections.Generic;

namespace autoCardboard.Common.Domain.Interfaces
{
    public interface IPlayerFactory
    {
        IEnumerable<IPlayer> CreatePlayers(int playerCount);
    }
}
