using System.Collections.Generic;

namespace autoCardboard.Common
{
    public interface IPlayerFactory<TGameTurn> where TGameTurn: IGameTurn
    {
        IEnumerable<IPlayer<TGameTurn>> CreatePlayers(PlayerConfiguration playerConfiguration);
    }
}
