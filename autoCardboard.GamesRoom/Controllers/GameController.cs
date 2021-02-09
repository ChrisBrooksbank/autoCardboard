using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace autoCardboard.GamesRoom.Controllers
{
   [ApiController]  
    public class GameController : ControllerBase
    {
        private readonly IOptions<ApiConfig> _config;
        public GameController(IOptions<ApiConfig> config)
        {
            _config = config;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/json")]
        [Route("Play")]
        public async Task<IActionResult> Get(string game)
        {
            var client = new HttpClient();
            var response = await client.GetAsync($"{_config.Value.BaseUrl}/Play?game=" + game);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return new JsonResult(responseBody);
        }

    }
}
