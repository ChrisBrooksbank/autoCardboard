using System.Threading.Tasks;

namespace autoCardboard.Common.Hubs
{
    public interface IGameHub
    {
        Task SendStatusMessage(string message);
        Task SendGameState(IGameState gameState);
    }
}
