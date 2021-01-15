using System.Linq;
using autoCardboard.Pandemic.Domain.State;
using NUnit.Framework;

namespace autoCardboard.Pandemic.Test
{
    public class InfectionDeckTests
    {
        private IPandemicGameState _gameState;

        [SetUp]
        public void Setup()
        {
            _gameState = new PandemicGameState();
            _gameState.Clear();
        }
        
        [Test]
        public void CheckEpidemicRefillsInfectionDeck()
        {
            var drawnInfectionCards = _gameState.InfectionDeck.Draw(3);
            _gameState.InfectionDiscardPile.AddCards(drawnInfectionCards);

            _gameState.Epidemic();
            Assert.AreEqual(_gameState.InfectionDeck.CardCount, 48);
        }

        [Test]
        public void CheckEpidemicEmptiesInfectionDiscardDeck()
        {
            var drawnInfectionCards = _gameState.InfectionDeck.Draw(3);
            _gameState.InfectionDiscardPile.AddCards(drawnInfectionCards);

            _gameState.Epidemic();
            Assert.AreEqual(_gameState.InfectionDiscardPile.CardCount, 0);
        }

        [Test]
        public void CheckEpidemicAddsThreeCubesToOneCity()
        {
            var drawnInfectionCards = _gameState.InfectionDeck.Draw(3);
            _gameState.InfectionDiscardPile.AddCards(drawnInfectionCards);

            _gameState.Epidemic();

            var infectedCity = _gameState.Cities.Single(c => c.DiseaseCubeCount > 0);

            Assert.AreEqual( infectedCity.DiseaseCubeCount, 3);
        }

        [Test]
        public void CheckEpidemicIncrementsInfectionRateMarker()
        {
            var drawnInfectionCards = _gameState.InfectionDeck.Draw(3);
            _gameState.InfectionDiscardPile.AddCards(drawnInfectionCards);

            _gameState.Epidemic();
            Assert.AreEqual(_gameState.InfectionRateMarker, 1);
        }

    }
}
