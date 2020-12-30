﻿using autoCardboard.Common.Domain.Interfaces;
using System.Collections.Generic;

namespace autoCardboard.Common.Domain
{
    public class Player : IPlayer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string,object> State { get; set; }

        public Dictionary<string, object> TakeTurn(Dictionary<string, object> gameState)
        {
            throw new System.NotImplementedException();
        }
    }
}
