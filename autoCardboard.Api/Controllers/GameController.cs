using autoCardboard.Common;
using autoCardboard.DependencyInjection;
using autoCardboard.ForSale;
using autoCardboard.Messaging;
using autoCardboard.Pandemic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace autoCardboard.Api.Controllers
{
    [ApiController]
    public class GameController : ControllerBase
    {
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
        public IActionResult GetNewGame(Game game)
        {
            // TODO
            return null;
        }

        [HttpGet]
        [Route("PlayNextRound")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/json")]
        public IActionResult PlayNextRound(Game game, IGameState gameState)
        {
            // TODO
            return null;
        }

        [HttpGet]
        [Route("Play")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/json")]
        public string Play(Game game)
        {
            var messenger = new MessageSender();
            messenger.SendMessageASync("AutoCardboard", $"Play {game}");
            
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

            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            string json = JsonConvert.SerializeObject(gameState,settings);

            return json;
        }

    }
}
