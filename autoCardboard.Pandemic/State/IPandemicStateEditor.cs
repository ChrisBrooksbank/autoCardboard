﻿using autoCardboard.Common;
using System.Collections.Generic;

namespace autoCardboard.Pandemic
{
    public interface IPandemicStateEditor
    {
        void Clear(IPandemicState state, int pandemicCardCount = 6);
        void Setup(IPandemicState state, IEnumerable<IPlayer<IPandemicTurn>> players, int pandemicCardCount = 6);
        void SetupPlayerDeck(IPandemicState state);
        void Epidemic(IPandemicState state);
        void InfectCities(IPandemicState state);
        void AddDiseaseCubes(IPandemicState state, City city, int count = 1);
        void AddDiseaseCube(IPandemicState state, Disease disease, City city, List<City> ignoreCities = null);
        void TakeTurn(IPandemicState state, IPandemicTurn turn);
        void TakePlayerAction(IPandemicState state, PlayerAction playerAction);
    }
}
