using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

namespace SnapFeud.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    public class GameController : Controller
    {
        private static GameState currentGameState;

        private static readonly Dictionary<string, UserState> users = new Dictionary<string, UserState>();

        private static readonly string[] challenges = {"Laptop", "Phone"};

        private static readonly Random random = new Random();

        public GameController()
        {
            if (currentGameState == null)
            {
                currentGameState = new GameState {GameId = Guid.NewGuid()};
                CreateNewChallenge(currentGameState);
            }
        }

        [HttpGet("{userName}")]
        public Guid JoinGame(string userName)
        {
            if (!users.ContainsKey(userName))
            {
                users[userName] = new UserState {UserName = userName};
            }

            users[userName].GameId = currentGameState.GameId;

            return currentGameState.GameId;
        }

        [HttpGet]
        public GameState GetGameState()
        {
            if (DateTime.UtcNow > currentGameState.ExpiryTime)
            {
                CreateNewChallenge(currentGameState);
            }

            return currentGameState;
        }

        [HttpPost("{gameId}/{userName}")]
        public async Task<bool> SubmitAnswer(string userName, Guid gameId)
        {
            byte[] photo = new byte[Request.ContentLength.Value];
            await Request.Body.ReadAsync(photo, 0, photo.Length);

            if (currentGameState == null || gameId != currentGameState.GameId || !users.ContainsKey(userName))
            {
                return false;
            }

            users[userName].Score += 1;

            return true;
        }

        [HttpGet("{gameId}")]
        public IEnumerable<UserState> GetScores(Guid gameId)
        {
            return users.Values.Where(x => x.GameId == gameId);
        }

        private void CreateNewChallenge(GameState gameState)
        {
            var nextChallenge = random.Next(0, challenges.Length);
            gameState.ExpiryTime = DateTime.UtcNow.AddMinutes(2);
            gameState.CurrentChallenge = challenges[nextChallenge];
        }
    }

    public class GameState
    {
        public string CurrentChallenge { get; set; }

        public DateTime ExpiryTime { get; set; }

        public Guid GameId { get; set; }
    }

    public class UserState
    {
        public string UserName { get; set; }

        public int Score { get; set; }

        public Guid GameId { get; set; }
    }
}
