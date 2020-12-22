namespace autoCardboard.Common.Domain.Interfaces
{
    public interface IPlayerBot
    {
        int Id { get; set; }
        string Name { get; set; }

        IGameState TakeTurn(IGameState gameState);
    }
}
