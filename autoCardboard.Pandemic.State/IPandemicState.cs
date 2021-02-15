using System.Collections.Generic;
using autoCardboard.Common;

namespace autoCardboard.Pandemic.State
{
    public interface IPandemicState : IGameState
    {
        bool IsGameOver { get; set; }
        int TurnsPlayed { get; set; }
        PlayerDeck PlayerDeck { get; set; }
        PlayerDeck PlayerDiscardPile { get; set; }
        InfectionDeck InfectionDeck { get; set; }
        CardDeck<Card> InfectionDiscardPile { get; set; }

        int OutbreakCount { get; set; }
        int PandemicCardCount { get; set; }

        Dictionary<int, PandemicPlayerState> PlayerStates { get; set; }
        List<MapNode> Cities { get; set; }
        int InfectionRateMarker { get; set; }
        int[] InfectionRateTrack { get; set; }
        Dictionary<Disease, DiseaseState> DiscoveredCures { get; set; }
        Dictionary<Disease, int> DiseaseCubeReserve { get; set; }
        int ResearchStationStock { get; set; }
        bool OneQuietNight { get; set; }
    }
}
