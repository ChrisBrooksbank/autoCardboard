using autoCardboard.Common.Domain.Cards;
using autoCardboard.Common.Domain.Interfaces;
using autoCardboard.Pandemic.Domain.PlayerTurns;
using System;
using System.Collections.Generic;

namespace autoCardboard.Pandemic.Domain.State
{
    public interface IPandemicGameState : IGameState
    {
        Boolean IsGameOver { get; set; }
        PlayerDeck PlayerDeck { get; set; }
        PlayerDeck PlayerDiscardPile { get; set; }
        InfectionDeck InfectionDeck { get; set; }
        CardDeck<Card> InfectionDiscardPile { get; set; }

        Dictionary<int, PandemicPlayerState> PlayerStates { get; set; }
        List<MapNode> Cities { get; set; }
        int InfectionRateMarker { get; set; }
        int[] InfectionRateTrack { get; set; }
        int EpidemicCardCount { get; set; }
        Dictionary<Disease, DiseaseState> DiscoveredCures { get; set; }
        Dictionary<Disease, int> DiseaseCubeStock { get; }
        int OutbreakCount { get; }

        void AddDiseaseCubes(City city, int count = 1);
        void AddDiseaseCube(Disease disease, City city, List<City> ignoreCities = null);

    void Clear(int pandemicCardCount = 6);
        void Epidemic();
        void InfectCities();
     
        void Setup(IEnumerable<IPlayer<IPandemicTurn>> players, int pandemicCardCount = 6);
    }
}
