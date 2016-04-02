using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using SnapFeud.WebApi.Models;

namespace SnapFeud.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    public class GameController : Controller
    {
        private readonly SnapFeudContext snapFeudContext;

        public GameController(SnapFeudContext snapFeudContext)
        {
            this.snapFeudContext = snapFeudContext;
        }

        [HttpGet("{userName}")]
        public async Task<Game> CreateGame(string userName)
        {
            var player = await snapFeudContext.Players.FirstOrDefaultAsync(x => x.Name == userName);
            if (player == null)
            {
                player = new Player { Name = userName };
                snapFeudContext.Players.Add(player);
            }

            var random = new Random();
            var challenges = await snapFeudContext.Challenges.ToListAsync();
            if (challenges.Count == 0)
            {
                return null;
            }

            var nextChallenge = random.Next(0, challenges.Count);
            var newChallenge = challenges[nextChallenge];

            var game = new Game
            {
                ChallengeExpireTime = DateTime.UtcNow.AddMinutes(1),
                Player = player,
                Id = Guid.NewGuid(),
                CurrentChallenge = newChallenge,
                Score = 0
            };

            snapFeudContext.Games.Add(game);

            await snapFeudContext.SaveChangesAsync();

            return game;
        }

        [HttpGet("{gameId}")]
        public async Task<Game> GetGame(Guid gameId)
        {
            return await snapFeudContext.Games.FirstOrDefaultAsync(x => x.Id == gameId);
        }

        [HttpPost("{gameId}")]
        public async Task<Game> SubmitAnswer(Guid gameId)
        {
            if (!Request.ContentLength.HasValue)
            {
                return null;
            }

            byte[] photo = new byte[Request.ContentLength.Value];
            await Request.Body.ReadAsync(photo, 0, photo.Length);

            var game = await snapFeudContext.Games.FirstOrDefaultAsync(x => x.Id == gameId);
            game.Score += 10;
            await snapFeudContext.SaveChangesAsync();
            return game;
        }
    }
}
