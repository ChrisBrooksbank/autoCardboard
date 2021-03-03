using autoCardboard.Common;
using autoCardboard.Pandemic.State;
using System.Collections.Generic;

namespace autoCardboard.Pandemic.TurnState
{
    public interface IPandemicTurn : IGameTurn
    {
        int CurrentPlayerId { get; set; }
        PandemicTurnType TurnType { get; set; }
        PlayerAction ActionTaken { get; }
        IEnumerable<PlayerEventPlayed> EventCardsPlayed{ get; set; }
        IPandemicState State { get; set; }
        IEnumerable<PandemicPlayerCard> CardsToDiscard{ get; set; }

        void DriveOrFerry(City toConnectedCity);
        void ShuttleFlight(City anyCityAlsoWithResearchStation);
        void TreatDisease(Disease disease);
        void BuildResearchStation(City city);
        void DiscoverCure(Disease disease, IEnumerable<PandemicPlayerCard> cardsToDiscard);
        void DirectFlight(City city);
        void CharterFlight(City anyCityAsDestination);
        void PlayEventCard(EventCard eventCard, City? city = null);
        void PlayEventCard(EventCard eventCard, int playerId, City? city = null);
    }
}
