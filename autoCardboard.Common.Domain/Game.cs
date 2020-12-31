using autoCardboard.Common.Domain.Interfaces;
using System.Collections.Generic;

namespace autoCardboard.Common.Domain
{
    public abstract class Game<TGameState, TGameTurn>
        where TGameState : IGameState
        where TGameTurn : IGameTurn
    {
        public TGameState State { get; set; }
        public IEnumerable<IPlayer> Players { get; set; }

        abstract public void Play();

        abstract public void Setup(IEnumerable<IPlayer> players);
    }
}
