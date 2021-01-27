using autoCardboard.Common;
using System.Collections.Generic;

namespace autoCardboard.Pandemic
{
    public interface IPandemicStateEditor
    {
        IPandemicState State { get; set; }

        void Clear(int pandemicCardCount = 6);
        void Setup(IEnumerable<IPlayer<IPandemicTurn>> players, int pandemicCardCount = 6);
        void SetupPlayerDeck();
        void Epidemic();
        void InfectCities();
        void AddDiseaseCubes(City city, int count = 1);
        void AddDiseaseCube(Disease disease, City city, List<City> ignoreCities = null);
        void TakeTurn(IPandemicTurn turn);
    }
}
