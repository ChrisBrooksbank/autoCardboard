using Microsoft.AspNetCore.Mvc;
using System;
using autoCardboard.Common;
using autoCardboard.Infrastructure;
using autoCardboard.Messaging;
using autoCardboard.DependencyInjection;
using autoCardboard.Pandemic.Game;
using autoCardboard.Pandemic.State;
using autoCardboard.Pandemic.TurnState;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace autoCardboard.GamesRoom.Controllers
{
   [ApiController]
   [Route("[controller]")]
    public class GameController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ICardboardLogger _logger;
        private readonly IMessageSender _messageSender;

        public GameController()
        {
            _serviceProvider = ServiceProviderFactory.GetServiceProvider();
            _logger = _serviceProvider.GetService<ICardboardLogger>();
            _messageSender = _serviceProvider.GetService<IMessageSender>();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/json")]
        public IActionResult Get()
        {
            var playerConfiguration = new PlayerConfiguration { PlayerCount = 2 };
            var pandemicGame = GameFactory.CreateGame<IPandemicState, IPandemicTurn>(_serviceProvider, playerConfiguration) as PandemicGame;
            pandemicGame.Setup(pandemicGame.Players);
            pandemicGame.Play();
            return new JsonResult(pandemicGame.State);
        }

    }
}
