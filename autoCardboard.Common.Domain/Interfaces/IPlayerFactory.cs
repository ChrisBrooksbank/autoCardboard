using System.Collections.Generic;

namespace autoCardboard.Common.Domain.Interfaces
{
    public interface IPlayerFactory<TGameTurn> where TGameTurn: IGameTurn
    {
        IEnumerable<IPlayer<TGameTurn>> CreatePlayers(int playerCount);
    }
}
