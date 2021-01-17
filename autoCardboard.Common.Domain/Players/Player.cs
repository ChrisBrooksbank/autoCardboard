using autoCardboard.Common.Domain.Interfaces;

namespace autoCardboard.Common.Domain.Players
{
    public abstract class Player<TGameTurn> : IPlayer<TGameTurn>
        where TGameTurn: IGameTurn
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public abstract void GetTurn(TGameTurn turn);
    }
}
