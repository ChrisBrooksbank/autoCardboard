using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace autoCardboard.GamesRoom.Hubs
{
    public class GamesHub : Hub
    {
        public async Task SendMessage(string topic, string payload)
        {
            await Clients.All.SendAsync("ReceiveMessage", topic, payload);
        }
    }
}