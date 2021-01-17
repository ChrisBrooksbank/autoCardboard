namespace autoCardboard.Pandemic
{
    public interface IPlayerActionHandler
    {
        bool HandleAction(int playerId, PlayerActionWithCity action, IPandemicGameState gameState);
    }
}
