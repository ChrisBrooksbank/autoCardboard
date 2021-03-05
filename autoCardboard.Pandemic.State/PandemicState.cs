using System;
using System.Collections.Generic;
using autoCardboard.Common;

namespace autoCardboard.Pandemic.State
{
    [Serializable]
    public class PandemicState: GameState, IPandemicState
    {
        public string Id { get; set; }
        public Boolean IsGameOver { get; set; }
        public string GameOverReason{ get; set; }
        public int ActionsPlayed { get; set; }
        public int OutbreakCount { get; set; }
        public Dictionary<Disease, int> DiseaseCubeReserve { get; set; }
        public int PandemicCardCount { get; set; }
        public int InfectionRateMarker { get; set; }
        public int ResearchStationStock { get; set; }
        public Dictionary<Disease, DiseaseState> DiscoveredCures { get; set; }
        public Dictionary<int, PandemicPlayerState> PlayerStates { get; set; }
        public InfectionDeck InfectionDeck { get; set; }
        public CardDeck<Card> InfectionDiscardPile { get; set; }
        public PlayerDeck PlayerDeck { get; set; }
        public PlayerDeck EventCardsQueue{ get; set; }
        public PlayerDeck PlayerDiscardPile { get; set; }
        public int[] InfectionRateTrack { get; set; }
        public List<MapNode> Cities { get; set; }
        public List<PlayerMeetingRequest> MeetingRequests { get; set; }
    }
}
