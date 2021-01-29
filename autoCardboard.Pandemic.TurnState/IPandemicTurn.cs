using autoCardboard.Common;
using autoCardboard.Pandemic.State;
using System.Collections.Generic;

namespace autoCardboard.Pandemic.TurnState
{
    public interface IPandemicTurn : IGameTurn
    {
        int CurrentPlayerId { get; set; }
        IEnumerable<PlayerAction> ActionsTaken { get; }
        IPandemicState State { get; set; }

        void DriveOrFerry(City toConnectedCity);
        void TreatDisease(Disease disease);
        void BuildResearchStation(City city);
    }
}
