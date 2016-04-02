using System;
using System.Collections.Generic;

namespace SnapFeud.WebApi.Models
{
    public class Game
    {
        public Guid Id { get; set; }

        public Challenge CurrentChallenge { get; set; }

        public DateTime ChallengeExpireTime { get; set; }

        public Player Player { get; set; }

        public int Score { get; set; }
    }
}