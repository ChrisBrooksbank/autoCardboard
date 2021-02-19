using System.Collections.Generic;
using autoCardboard.Pandemic.State;

namespace autoCardBoard.Pandemic.Bots
{
    public interface IResearchStationHelper
    {
        bool CouldBuildResearchStation(IPandemicState state, City currentLocation, PlayerRole playerRole, List<PandemicPlayerCard> playerHand);
        bool ShouldBuildResearchStation(IPandemicState state, City currentLocation, PlayerRole playerRole, List<PandemicPlayerCard> playerHand);
    }
}
