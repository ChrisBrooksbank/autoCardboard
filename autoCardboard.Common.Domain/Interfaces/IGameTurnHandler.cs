namespace autoCardboard.Common.Domain.Interfaces
{
    // modify the IGameState in accordance with the IGameTurn ( turn was selected by a IPlayer )
    public interface IGameTurnHandler<TGameState,TGameTurn>
        where TGameState : IGameState
        where TGameTurn : IGameTurn
    {
        void TakeTurn(TGameState state, TGameTurn turn);
    }
}
