using System.Collections.Generic;
using autoCardboard.Pandemic.State;

namespace autoCardBoard.Pandemic.Bots
{
    public interface IResearchStationHelper
    {
        bool CouldBuildResearchStation(List<MapNode> mapNodes, City currentLocation, PlayerRole playerRole, List<PandemicPlayerCard> playerHand);
        bool ShouldBuildResearchStation(List<MapNode> mapNodes, City currentLocation, PlayerRole playerRole, List<PandemicPlayerCard> playerHand);
    }
}
