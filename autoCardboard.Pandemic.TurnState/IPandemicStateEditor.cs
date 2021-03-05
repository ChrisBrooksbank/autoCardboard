using System.Collections.Generic;
using autoCardboard.Common;
using autoCardboard.Pandemic.State;

namespace autoCardboard.Pandemic.TurnState
{
    public interface IPandemicStateEditor
    {
        void Clear(IPandemicState state, int pandemicCardCount = 4);
        void Setup(IPandemicState state, IEnumerable<IPlayer<IPandemicTurn>> players, int pandemicCardCount = 4);
        void SetupPlayerDeck(IPandemicState state);
        void Epidemic(IPandemicState state);
        void InfectCities(IPandemicState state);
        void AddDiseaseCubes(IPandemicState state, City city, int count = 1);
        void AddDiseaseCube(IPandemicState state, Disease disease, City city, List<City> ignoreCities = null);
        void ApplyTurn(IPandemicState state, IPandemicTurn turn);
        void ApplyPlayerAction(IPandemicState state, PlayerAction playerAction);
        void RemoveUnknownStateForPlayer(IPandemicState state, int playerId);
    }
}
