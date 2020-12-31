using System.Collections.Generic;

namespace autoCardboard.Common.Domain.Interfaces
{
    public interface IGame<TGameState,TGameTurn>
        where TGameState : IGameState
        where TGameTurn : IGameTurn
    {
        TGameState State { get; set; }
        IEnumerable<IPlayer<IGameTurn>> Players { get; set; }

        void Setup(IEnumerable<IPlayer<IGameTurn>> players);
        void Play();
    }
}
