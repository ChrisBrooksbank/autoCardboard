using autoCardboard.Common.Domain.Interfaces;
using System.Collections.Generic;

namespace autoCardboard.Common.Domain
{
    public abstract class Game : IGame
    {
        public Dictionary<string,object> State { get; set; }
        public IEnumerable<IPlayer> Players { get; set; }

        abstract public void Play();

        abstract public void Setup(IEnumerable<IPlayer> players);
    }
}
