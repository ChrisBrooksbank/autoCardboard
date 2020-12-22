using System.Collections.Generic;

namespace autoCardboard.Common.Domain.Interfaces
{
    public interface IGame
    {
        void Initialise(IEnumerable<IPlayerBot> players, IGameMonitor gameMonitor);
        void Play();
    }
}
