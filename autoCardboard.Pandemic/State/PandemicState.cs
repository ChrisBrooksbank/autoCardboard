using System;
using autoCardboard.Common;
using System.Collections.Generic;

namespace autoCardboard.Pandemic
{
    [Serializable]
    public class PandemicState: GameState, IPandemicState
    {
        public Dictionary<int, PandemicPlayerState> PlayerStates { get; set; }

        public int OutbreakCount { get; set; }

        public List<MapNode> Cities { get; set; }

        public Dictionary<Disease, int> DiseaseCubeStock { get; set; }
        public int ResearchStationStock { get; set; }

        public InfectionDeck InfectionDeck { get; set; }
        public CardDeck<Card> InfectionDiscardPile { get; set; }
        public PlayerDeck PlayerDeck { get; set; }
        public PlayerDeck PlayerDiscardPile { get; set; }
        public int InfectionRateMarker { get; set; }
        public int[] InfectionRateTrack { get; set; }

        public int PandemicCardCount { get; set; }

        public Dictionary<Disease, DiseaseState> DiscoveredCures { get; set; }

        public Boolean IsGameOver { get; set; }
    }
}
