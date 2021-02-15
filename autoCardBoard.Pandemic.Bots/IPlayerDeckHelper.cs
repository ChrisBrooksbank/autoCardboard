using System.Collections.Generic;
using autoCardboard.Pandemic.State;

namespace autoCardBoard.Pandemic.Bots
{
    public interface IPlayerDeckHelper
    {
        List<PandemicPlayerCard> GetCardsToDiscardToCure(IPandemicState state, Disease disease, PlayerRole playerRole, IEnumerable<PandemicPlayerCard> cards);
        PandemicPlayerCard GetWeakCard(IPandemicState state, PlayerRole playerRole, IEnumerable<PandemicPlayerCard> cards);
        IEnumerable<Disease> GetDiseasesCanCure(PlayerRole playerRole, IEnumerable<PandemicPlayerCard> cards);
        Dictionary<Disease, List<PandemicPlayerCard>> GetCityCardsByColour(IEnumerable<PandemicPlayerCard> cards);
    }
}
