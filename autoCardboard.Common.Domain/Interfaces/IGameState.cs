namespace autoCardboard.Common.Domain.Interfaces
{
    public interface IGameState
    {
        T GetState<T>();
        T GetState<T>(IPlayer playerBot);
    }
}
