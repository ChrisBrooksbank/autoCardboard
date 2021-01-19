using autoCardboard.Common;
using System.Collections.Generic;

namespace autoCardboard.Pandemic
{
    public interface IPandemicTurn : IGameTurn
    {
        int CurrentPlayerId { get; set; }
        IEnumerable<PlayerActionWithCity> ActionsTaken { get; }
        IPandemicGameState State { get; set; }

        void DriveOrFerry(City toConnectedCity);
        void TreatDisease(Disease disease);
    }
}
