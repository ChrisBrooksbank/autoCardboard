using autoCardboard.Common;
using System.Collections.Generic;
using autoCardboard.Pandemic.State;

namespace autoCardboard.Pandemic
{
    public interface IPandemicTurn : IGameTurn
    {
        int CurrentPlayerId { get; set; }
        IEnumerable<PlayerAction> ActionsTaken { get; }
        IPandemicState State { get; set; }

        void DriveOrFerry(City toConnectedCity);
        void TreatDisease(Disease disease);
    }
}
