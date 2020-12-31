using System.Collections.Generic;

namespace autoCardboard.Common.Domain.Interfaces
{
    public interface IPlayer<TGameTurn> where TGameTurn: IGameTurn
    {
        int Id { get; set; }
        string Name { get; set; }

        void TakeTurn(TGameTurn turn);
    }
}
