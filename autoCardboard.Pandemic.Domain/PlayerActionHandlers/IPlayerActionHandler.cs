namespace autoCardboard.Pandemic.Domain
{
    public interface IPlayerActionHandler
    {
        bool HandleAction(int playerId, PlayerActionWithCity action, IPandemicGameState gameState);
    }
}
