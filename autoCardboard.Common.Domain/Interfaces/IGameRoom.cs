using System.Collections.Generic;

namespace autoCardboard.Common.Domain.Interfaces
{
    public interface IGamesRoom
    {
        IEnumerable<IGame> Games { get; set; }
    }
}
