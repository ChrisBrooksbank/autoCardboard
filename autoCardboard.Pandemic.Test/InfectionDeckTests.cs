using System.Collections.Generic;
using System.Linq;
using autoCardboard.Infrastructure;
using NUnit.Framework;

namespace autoCardboard.Pandemic.Test
{
    public class InfectionDeckTests
    {
        private IPandemicState _gameState;
        private IPandemicStateEditor _stateEditor;

        [SetUp]
        public void Setup()
        {
            _gameState = new PandemicState();
            _stateEditor = new PandemicStateEditor(new CardboardLogger());
            _stateEditor.State = _gameState;
            _stateEditor.Clear();
            var players = new List<PandemicPlayer>();
            _stateEditor.Setup(players);
        }
        
        [Test]
        public void CheckEpidemicRefillsInfectionDeck()
        {
            var drawnInfectionCards = _gameState.InfectionDeck.Draw(3);
            _gameState.InfectionDiscardPile.AddCards(drawnInfectionCards);

            _stateEditor.Epidemic();
            Assert.AreEqual(_gameState.InfectionDeck.CardCount, 48);
        }

        [Test]
        public void CheckEpidemicEmptiesInfectionDiscardDeck()
        {
            var drawnInfectionCards = _gameState.InfectionDeck.Draw(3);
            _gameState.InfectionDiscardPile.AddCards(drawnInfectionCards);

            _stateEditor.Epidemic();
            Assert.AreEqual(_gameState.InfectionDiscardPile.CardCount, 0);
        }

        [Test]
        public void CheckEpidemicAddsThreeCubesToOneCity()
        {
            _stateEditor.Clear();
            var drawnInfectionCards = _gameState.InfectionDeck.Draw(3);
            _gameState.InfectionDiscardPile.AddCards(drawnInfectionCards);

            _stateEditor.Epidemic();

            var infectedCity = _gameState.Cities.Single(c => c.DiseaseCubeCount > 0);

            Assert.AreEqual( infectedCity.DiseaseCubeCount, 3);
        }

        [Test]
        public void CheckEpidemicIncrementsInfectionRateMarker()
        {
            var drawnInfectionCards = _gameState.InfectionDeck.Draw(3);
            _gameState.InfectionDiscardPile.AddCards(drawnInfectionCards);

            _stateEditor.Epidemic();
            Assert.AreEqual(_gameState.InfectionRateMarker, 1);
        }

    }
}
