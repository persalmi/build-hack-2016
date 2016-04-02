using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using SnapFeud.WebApi.Models;

namespace SnapFeud.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    public class GameController : Controller
    {
        [HttpGet("{userName}")]
        public Game CreateGame(string userName)
        {
            return null;
        }

        [HttpGet]
        public Game GetGame(Guid gameId)
        {
            return null;
        }

        [HttpPost("{gameId}/{userName}")]
        public async Task<Game> SubmitAnswer(Guid gameId)
        {
            if (!Request.ContentLength.HasValue)
            {
                return null;
            }

            byte[] photo = new byte[Request.ContentLength.Value];
            await Request.Body.ReadAsync(photo, 0, photo.Length);

            return null;
        }
    }
}
