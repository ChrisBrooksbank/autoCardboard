using autoCardboard.Pandemic.State;
using autoCardboard.Pandemic.TurnState;
using autoCardBoard.Pandemic.Bots;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace autoCardboard.Pandemic.Test
{
    public class InfectionDeckTests
    {
        private IPandemicState _gameState;
        private IPandemicStateEditor _stateEditor;

        public InfectionDeckTests()
        {
            _gameState = new PandemicState();
            _stateEditor = new PandemicStateEditor(new PandemicActionValidator());
            _stateEditor.Clear(_gameState);
            var players = new List<PandemicBotStandard>();
            _stateEditor.Setup(_gameState, players);
        }

        [Fact]
        public void CheckEpidemicRefillsInfectionDeck()
        {
            var drawnInfectionCards = _gameState.InfectionDeck.Draw(3);
            _gameState.InfectionDiscardPile.AddCards(drawnInfectionCards);

            _stateEditor.Epidemic(_gameState);
            Assert.Equal(48, _gameState.InfectionDeck.CardCount);
        }

        [Fact]
        public void CheckEpidemicEmptiesInfectionDiscardDeck()
        {
            var drawnInfectionCards = _gameState.InfectionDeck.Draw(3);
            _gameState.InfectionDiscardPile.AddCards(drawnInfectionCards);

            _stateEditor.Epidemic(_gameState);
            Assert.Equal(0, _gameState.InfectionDiscardPile.CardCount);
        }

        [Fact]
        public void CheckEpidemicAddsThreeCubesToOneCity()
        {
            _stateEditor.Clear(_gameState);
            var drawnInfectionCards = _gameState.InfectionDeck.Draw(3);
            _gameState.InfectionDiscardPile.AddCards(drawnInfectionCards);

            _stateEditor.Epidemic(_gameState);

            var infectedCity = _gameState.Cities.Single(c => c.DiseaseCubeCount > 0);

            Assert.Equal( 3, infectedCity.DiseaseCubeCount);
        }

        [Fact]
        public void CheckEpidemicIncrementsInfectionRateMarker()
        {
            var drawnInfectionCards = _gameState.InfectionDeck.Draw(3);
            _gameState.InfectionDiscardPile.AddCards(drawnInfectionCards);

            _stateEditor.Epidemic(_gameState);
            Assert.Equal(1, _gameState.InfectionRateMarker);
        }

    }
}
