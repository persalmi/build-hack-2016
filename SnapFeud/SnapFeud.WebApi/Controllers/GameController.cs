using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using SnapFeud.WebApi.Models;

namespace SnapFeud.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    public class GameController : Controller
    {
        private readonly SnapFeudContext snapFeudContext;
        private Random random = new Random();

        public GameController(SnapFeudContext snapFeudContext)
        {
            this.snapFeudContext = snapFeudContext;
        }

        [HttpGet("{userName}")]
        public async Task<Guid> CreateGame(string userName)
        {
            var player = await snapFeudContext.Players.FirstOrDefaultAsync(x => x.Name == userName);
            if (player == null)
            {
                player = new Player { Name = userName };
                snapFeudContext.Players.Add(player);
            }

            var game = new Game
            {
                Player = player,
                Id = Guid.NewGuid(),
                Score = 0
            };

            await NextChallenge(game);

            snapFeudContext.Games.Add(game);

            await snapFeudContext.SaveChangesAsync();

            return game.Id;
        }

        [HttpGet("{gameId}")]
        public async Task<Game> GetGame(Guid gameId)
        {
            return await snapFeudContext.Games.Include(g => g.CurrentChallenge).FirstOrDefaultAsync(x => x.Id == gameId);
        }

        [HttpPost("{gameId}")]
        public async Task<string> SubmitAnswer(Guid gameId)
        {
            if (!Request.ContentLength.HasValue)
            {
                return "No photo";
            }

            try
            {
                byte[] photo = new byte[Request.ContentLength.Value];
                await Request.Body.ReadAsync(photo, 0, photo.Length);

                var analysis = await AnalyzePhoto(photo);
                var game =
                    await
                        snapFeudContext.Games.Include(g => g.CurrentChallenge).FirstOrDefaultAsync(x => x.Id == gameId);

                var expectedTags = game.CurrentChallenge.Tag.Split(',');
                var isMatch = expectedTags.All(x => analysis.Tags.Any(y => y.Name == x));
                if (isMatch)
                {
                    //var bestMatch = analysis.Tags.Where(x => expectedTags.Contains(x.Name)).Max(x => x.Confidence);
                    game.Score += 500; //(int)bestMatch * 1000;

                    await NextChallenge(game);
                }

                await snapFeudContext.SaveChangesAsync();
                return isMatch ? "Correct answer" : "Wrong answer";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private async Task NextChallenge(Game game)
        {
            var challenges = await snapFeudContext.Challenges.ToListAsync();
            var nextChallenge = random.Next(0, challenges.Count);
            var newChallenge = challenges.Count == 0 ? null : challenges[nextChallenge];
            game.CurrentChallenge = newChallenge;
            game.ChallengeExpireTime = DateTime.UtcNow.AddMinutes(1);
        }

        private async Task<AnalysisResult> AnalyzePhoto(byte[] photo)
        {

            VisionServiceClient visionServiceClient = new VisionServiceClient("");
            using (var stream = new MemoryStream(photo))
            {
                VisualFeature[] visualFeatures = new VisualFeature[]
                {
                        VisualFeature.Adult, VisualFeature.Categories, VisualFeature.Color, VisualFeature.Description,
                        VisualFeature.Faces, VisualFeature.ImageType, VisualFeature.Tags
                };

                return await visionServiceClient.AnalyzeImageAsync(stream, visualFeatures);
            }
        }
    }
}
