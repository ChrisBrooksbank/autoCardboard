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

        private void Setup()
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
            Setup();
            var drawnInfectionCards = _gameState.InfectionDeck.Draw(3);
            _gameState.InfectionDiscardPile.AddCards(drawnInfectionCards);

            _stateEditor.Epidemic(_gameState);
            Assert.Equal(48, _gameState.InfectionDeck.CardCount);
        }

        [Fact]
        public void CheckEpidemicEmptiesInfectionDiscardDeck()
        {
            Setup();
            var drawnInfectionCards = _gameState.InfectionDeck.Draw(3);
            _gameState.InfectionDiscardPile.AddCards(drawnInfectionCards);

            _stateEditor.Epidemic(_gameState);
            Assert.Equal(0, _gameState.InfectionDiscardPile.CardCount);
        }

        [Fact]
        public void CheckEpidemicAddsThreeCubesToOneCity()
        {
            Setup();
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
            Setup();
            var drawnInfectionCards = _gameState.InfectionDeck.Draw(3);
            _gameState.InfectionDiscardPile.AddCards(drawnInfectionCards);

            _stateEditor.Epidemic(_gameState);
            Assert.Equal(1, _gameState.InfectionRateMarker);
        }

    }
}
