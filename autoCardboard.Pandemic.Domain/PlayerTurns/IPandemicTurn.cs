﻿using autoCardboard.Common;
using System.Collections.Generic;

namespace autoCardboard.Pandemic.Domain
{
    public interface IPandemicTurn : IGameTurn
    {
        int CurrentPlayerId { get; set; }
        IEnumerable<PlayerActionWithCity> ActionsTaken { get; }
    }
}
