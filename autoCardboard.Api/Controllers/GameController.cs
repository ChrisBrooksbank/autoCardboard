using System;
using autoCardboard.Common;
using autoCardboard.DependencyInjection;
using autoCardboard.ForSale;
using autoCardboard.Infrastructure;
using autoCardboard.Messaging;
using autoCardboard.Pandemic;
using autoCardboard.Pandemic.Game;
using autoCardboard.Pandemic.State;
using autoCardboard.Pandemic.TurnState;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace autoCardboard.Api.Controllers
{

    [ApiController]
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
        [Route("GetGames")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/json")]
        public IActionResult GetGames()
        {
            var games = new { games = new string[] {Game.ForSale.ToString(), Game.Pandemic.ToString()} };
            return new JsonResult(games);
        }

        [HttpGet]
        [Route("GetNewGame")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/json")]
        public JsonResult GetNewGame(Game game)
        {
            LogMessage($"Getting new game {game}");
            var playerConfiguration = new PlayerConfiguration { PlayerCount = 2 };
            IGameState gameState = null;

            switch (game)
            {
                case Game.Pandemic:
                    var pandemicGame = GameFactory.CreateGame<IPandemicState, IPandemicTurn>(_serviceProvider, playerConfiguration) as PandemicGame;
                    pandemicGame.Setup(pandemicGame.Players);
                    gameState = pandemicGame.State;
                    break;
                case Game.ForSale:
                    var forSaleGame = GameFactory.CreateGame<IForSaleGameState, IForSaleGameTurn>(_serviceProvider, playerConfiguration);
                    gameState = forSaleGame.State;
                    break;
            }

            LogMessage($"Got new game {game}");
            return new JsonResult(gameState);
        }

        [HttpGet]
        [Route("Play")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/json")]
        public JsonResult Play(Game game)
        {
            LogMessage( $"Playing game {game}");
            var serviceProvider = ServiceProviderFactory.GetServiceProvider();
            var playerConfiguration = new PlayerConfiguration { PlayerCount = 2 };
            IGameState gameState = null;

            switch (game)
            {
                case Game.Pandemic:
                    gameState = GameFactory.CreateGame<IPandemicState, IPandemicTurn>(serviceProvider, playerConfiguration).Play();
                    break;
                case Game.ForSale:
                    gameState = GameFactory.CreateGame<IForSaleGameState, IForSaleGameTurn>(serviceProvider, playerConfiguration).Play();
                    break;
            }

            LogMessage($"Finished playing game {game}");
            return new JsonResult(gameState);;
        }

        private void LogMessage(string message, string topic = "AutoCardboard")
        {
            _logger.Information(message);
            _messageSender.SendMessageASync(topic, message);
        }

    }
}
