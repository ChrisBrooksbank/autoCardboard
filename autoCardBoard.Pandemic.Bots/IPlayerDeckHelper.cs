using System.Collections.Generic;
using autoCardboard.Pandemic.State;

namespace autoCardBoard.Pandemic.Bots
{
    public interface IPlayerDeckHelper
    {
        PandemicPlayerCard GetWeakCard(IPandemicState state, PlayerRole playerRole, IEnumerable<PandemicPlayerCard> cards);
        IEnumerable<Disease> GetDiseasesCanCure(PlayerRole playerRole, IEnumerable<PandemicPlayerCard> cards);
        Dictionary<Disease, List<PandemicPlayerCard>> GetCityCardsByColour(IEnumerable<PandemicPlayerCard> cards);
    }
}
