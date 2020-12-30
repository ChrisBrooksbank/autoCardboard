using autoCardboard.Common.Domain.Interfaces;
using System;
using System.Collections.Generic;

namespace autoCardboard.ForSale.Domain
{
    // TODO implement especially TakeTurn
    public class ForSalePlayer : IPlayer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string,object> State { get; set; }

        public Dictionary<string, object> TakeTurn(Dictionary<string, object> gameState)
        {
            throw new NotImplementedException();
        }
    }
}
