using System;
using autoCardboard.Common;
using autoCardboard.DependencyInjection;
using autoCardboard.ForSale;
using autoCardboard.Infrastructure;
using autoCardboard.Messaging;
using autoCardboard.Pandemic.Game;
using autoCardboard.Pandemic.State;
using autoCardboard.Pandemic.TurnState;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace autoCardboard.Api.Controllers
{

    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly MessageSenderConfiguration _messageSenderconfiguration;

        public GameController(IOptions<MessageSenderConfiguration> configuration)
        {
            _messageSenderconfiguration = configuration.Value;
            _serviceProvider = ServiceProviderFactory.GetServiceProvider(_messageSenderconfiguration);
        }

        [HttpGet]
        [Route("GetGames")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/json")]
        public IActionResult GetGames()
        {
            var games = new { games = new string[] {Game.ForSale.ToString(), Game.Pandemic.ToString()} };
            return new JsonResult(games);
        }

        [HttpGet]
        [Route("Play")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/json")]
        public JsonResult Play(Game game, int playCount = 1)
        {
            var serviceProvider = ServiceProviderFactory.GetServiceProvider(_messageSenderconfiguration);
            var playerConfiguration = new PlayerConfiguration { PlayerCount = 2 };
            IGameState gameState = null;

            for (var gamesPlayed = 0; gamesPlayed < playCount; gamesPlayed++)
            {
                switch (game)
                {
                    case Game.Pandemic:
                        gameState = GameFactory.CreateGame<IPandemicState, IPandemicTurn>(serviceProvider, playerConfiguration).Play();
                        break;
                    case Game.ForSale:
                        gameState = GameFactory.CreateGame<IForSaleGameState, IForSaleGameTurn>(serviceProvider, playerConfiguration).Play();
                        break;
                }
            }

            return new JsonResult(gameState);;
        }
    }
}
