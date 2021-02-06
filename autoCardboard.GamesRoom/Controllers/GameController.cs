using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace autoCardboard.GamesRoom.Controllers
{
   [ApiController]  
    public class GameController : ControllerBase
    {

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/json")]
        [Route("Play")]
        public async Task<IActionResult> Get(string game)
        {
            var client = new HttpClient();
            var response = await client.GetAsync("https://localhost:44387/Play?game=" + game);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return new JsonResult(responseBody);
        }

    }
}
