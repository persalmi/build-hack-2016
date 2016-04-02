using System;
using System.Threading.Tasks;
using SnapFeud.WebApi.Models;

namespace SnapFeud.WebApi.Controllers
{
    public interface IGameController
    {
        Task<Game> CreateGame(string userName);
        Task<Game> GetGame(Guid gameId);
        Task<Game> SubmitAnswer(Guid gameId);
    }
}