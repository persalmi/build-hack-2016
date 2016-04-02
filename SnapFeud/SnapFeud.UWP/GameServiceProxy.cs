using System;
using System.Net.Http;
using System.Threading.Tasks;
using SnapFeud.WebApi.Controllers;
using SnapFeud.WebApi.Models;

namespace SnapFeud.UWP
{
    class GameServiceProxy
    {
        private readonly Uri baseUri;
        private HttpClient client;

        public GameServiceProxy(Uri baseUri)
        {
            this.baseUri = baseUri;
            client = new HttpClient();
        }

        public async Task<Game> CreateGame(string userName)
        {
            Uri geturi = new Uri($"{baseUri}api/game/creategame/{userName}");
            HttpResponseMessage responseGet = await client.GetAsync(geturi);
            string response = await responseGet.Content.ReadAsStringAsync();

            return Newtonsoft.Json.JsonConvert.DeserializeObject<Game>(response);
        }

        public async Task<Game> GetGame(Guid gameId)
        {
            var geturi = new Uri($"{baseUri}api/game/getgame/{gameId}");
            var responseGet = await client.GetAsync(geturi);
            var response = await responseGet.Content.ReadAsStringAsync();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Game>(response);
        }

        public async Task<Game> SubmitAnswer(Guid gameId, byte[] photo)
        {
            var postUri = new Uri($"{baseUri}api/game/submitanswer/{gameId}");
            var responsePost = await client.PostAsync(postUri, new ByteArrayContent(photo));
            var response = await responsePost.Content.ReadAsStringAsync();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Game>(response);
        }
    }
}
