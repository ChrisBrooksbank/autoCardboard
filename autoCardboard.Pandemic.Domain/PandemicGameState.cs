using autoCardboard.Common.Domain;
using System.Collections.Generic;

namespace autoCardboard.Pandemic.Domain
{
    public class PandemicGameState: GameState
    {
        public PandemicMap Map { get; set; }
        public int OutbreakCount { get; set; }
        public int InfectionRateMarker { get; set; }
        public int[] InfectionRateTrack { get; set; }
        public int EpidemicCardCount { get; set; }

        public CardDeck<Card> InfectionDeck { get; set; }
        public CardDeck<PandemicPlayerCard> PlayerDeck { get; set; }
        public CardDeck<PandemicPlayerCard> PlayerDiscardPile { get; set; }

        public Dictionary<Disease,DiseaseState> DiscoveredCures { get; set; }

        public Dictionary<Disease,int> DiseaseCubeStock { get; set; }

        public Dictionary<int, PandemicPlayerState> PlayerStates { get; set; }


        public PandemicGameState()
        {
            Map = new PandemicMap();
            OutbreakCount = 0;
            InfectionRateMarker = 0;
            InfectionRateTrack = new int[] {2,2,2,3,3,4,4};
            EpidemicCardCount = 6;
            InfectionDeck = new CardDeck<Card>();
            PlayerDeck = new CardDeck<PandemicPlayerCard>();
            PlayerDiscardPile = new CardDeck<PandemicPlayerCard>();
            DiscoveredCures = new Dictionary<Disease, DiseaseState>();
            DiseaseCubeStock = new Dictionary<Disease, int>();
            PlayerStates = new Dictionary<int,PandemicPlayerState>();
        }
    }
}
