using System.Collections.Generic;

namespace SnapFeud.WebApi.Models
{
    public class Player
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<Game> Games { get; set; }

        public int HighScore { get; set; }

    }
}