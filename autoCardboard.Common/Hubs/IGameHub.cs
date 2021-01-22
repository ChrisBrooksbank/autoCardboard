using System.Threading.Tasks;

namespace autoCardboard.Common.Hubs
{
    public interface IGameHub
    {
        Task SendGameState(IGameState gameState);
    }
}
