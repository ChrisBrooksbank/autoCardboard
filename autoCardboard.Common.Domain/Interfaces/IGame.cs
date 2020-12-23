using System.Collections.Generic;

namespace autoCardboard.Common.Domain.Interfaces
{
    public interface IGame
    {
        void Initialise(IEnumerable<IPlayer> players, IGameMonitor gameMonitor);
        void Play();
    }
}
