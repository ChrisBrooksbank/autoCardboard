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
        IPandemicState State { get; set; }
        IEnumerable<PandemicPlayerCard> CardsToDiscard{ get; set; }

        void DriveOrFerry(City toConnectedCity);
        void ShuttleFlight(City anyCityAlsoWithResearchStation);
        void TreatDisease(Disease disease);
        void BuildResearchStation(City city);
        void DiscoverCure(Disease disease, IEnumerable<PandemicPlayerCard> cardsToDiscard);
        void PlayOneQuietNight();
    }
}
