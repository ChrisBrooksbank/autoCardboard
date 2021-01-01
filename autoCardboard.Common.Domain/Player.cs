using autoCardboard.Common.Domain.Interfaces;
using System.Collections.Generic;

namespace autoCardboard.Common.Domain
{
    public abstract class Player<TGameTurn> : IPlayer<TGameTurn>
        where TGameTurn: IGameTurn
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public abstract void GetTurn(TGameTurn turn);
    }
}
