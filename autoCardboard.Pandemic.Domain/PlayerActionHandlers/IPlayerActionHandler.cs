using autoCardboard.Pandemic.Domain.State;

namespace autoCardboard.Pandemic.Domain.PlayerActionHandlers
{
    public interface IPlayerActionHandler
    {
        bool HandleAction(int playerId, PlayerActionWithCity action, IPandemicGameState gameState);
    }
}
