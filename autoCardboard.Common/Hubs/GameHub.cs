using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace autoCardboard.Common.Hubs
{
    public class GameHub: Hub,  IGameHub
    {
        public async Task SendStatusMessage(string message)
        {
            if (Clients != null)
            {
                await Clients.All.SendAsync("ReceiveGameStatusMessage", message);
            }
        }

        public async Task SendGameState(IGameState gameState)
        {
            if (Clients != null)
            {
                await Clients.All.SendAsync("ReceiveGameState", gameState);
            }
        }
    }
}
