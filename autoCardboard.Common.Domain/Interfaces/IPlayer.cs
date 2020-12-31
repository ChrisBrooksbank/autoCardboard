using System.Collections.Generic;

namespace autoCardboard.Common.Domain.Interfaces
{
    public interface IPlayer
    {
        int Id { get; set; }
        string Name { get; set; }

        // TODO add TakeTurnUrl property ?

        // TODO remove this, is part of IGameState
        Dictionary<string,object> State { get; set; }

        Dictionary<string, object> TakeTurn(Dictionary<string, object> gameState);
    }
}
