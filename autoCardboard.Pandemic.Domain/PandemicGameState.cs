using autoCardboard.Common.Domain;
using System.Collections.Generic;

namespace autoCardboard.Pandemic.Domain
{
    public class PandemicGameState: GameState
    {
        public PandemicBoard Map { get; set; }
        public int InfectionRateMarker { get; set; }
        public int[] InfectionRateTrack { get; set; }
        public int EpidemicCardCount { get; set; }

        public InfectionDeck InfectionDeck { get; set; }
        public PlayerDeck PlayerDeck { get; set; }
        public PlayerDeck PlayerDiscardPile { get; set; }

        public Dictionary<Disease,DiseaseState> DiscoveredCures { get; set; }

        public Dictionary<Disease,int> DiseaseCubeStock { get; set; }

        public Dictionary<int, PandemicPlayerState> PlayerStates { get; set; }


        public PandemicGameState()
        {
            Map = new PandemicBoard();
            InfectionRateMarker = 0;
            InfectionRateTrack = new int[] {2,2,2,3,3,4,4};
            EpidemicCardCount = 6;
            InfectionDeck = new InfectionDeck();
            PlayerDeck = new PlayerDeck();
            PlayerDiscardPile = new PlayerDeck();
            DiscoveredCures = new Dictionary<Disease, DiseaseState>();
            PlayerStates = new Dictionary<int,PandemicPlayerState>();
        }
    }
}
