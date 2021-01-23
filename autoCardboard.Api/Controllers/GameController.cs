using System.Threading.Tasks;
using autoCardboard.Common;
using autoCardboard.DependencyInjection;
using autoCardboard.Messaging;
using autoCardboard.Pandemic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace autoCardboard.Api.Controllers
{
    [ApiController]
    public class GameController : ControllerBase
    {
        [HttpGet]
        [Route("Pandemic")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task Pandemic()
        {
            var messenger = new GameMessenger();
            messenger.SendMessageASync("AutoCardboard", "Started Pandemic");

            var serviceProvider = ServiceProviderFactory.GetServiceProvider();
            var playerConfiguration = new PlayerConfiguration { PlayerCount = 2 };
            var pandemicGame = GameFactory.CreateGame<IPandemicState, IPandemicTurn>(serviceProvider, playerConfiguration);
            pandemicGame.Play();
        }
    }
}
