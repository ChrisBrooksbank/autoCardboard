namespace autoCardboard.Common.Domain.Interfaces
{
    /// <summary>
    /// A player is a piece of code which :
    /// given a IGameTurn, containing visible (immutable) gameState and methods corresponding to legal moves in this game
    /// Is responsible for deciding what to do this turn, and to signal that by calling the methods on the IGameTurn.
    /// </summary>
    /// <typeparam name="TGameTurn"></typeparam>
    public interface IPlayer<TGameTurn> where TGameTurn: IGameTurn
    {
        int Id { get; set; }
        string Name { get; set; }

        void GetTurn(TGameTurn turn);
    }
}
