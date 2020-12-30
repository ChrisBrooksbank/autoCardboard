using System.Collections.Generic;

namespace autoCardboard.Common.Domain.Interfaces
{
    public interface IGame
    {
        Dictionary<string,object> State { get; set; }
        IEnumerable<IPlayer> Players { get; set; }

        void Setup(IEnumerable<IPlayer> players);
        void Play();
    }
}
