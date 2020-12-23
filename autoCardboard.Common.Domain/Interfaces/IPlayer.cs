namespace autoCardboard.Common.Domain.Interfaces
{
    public interface IPlayer
    {
        int Id { get; set; }
        string Name { get; set; }

        IGameState TakeTurn(IGameState gameState);
    }
}
