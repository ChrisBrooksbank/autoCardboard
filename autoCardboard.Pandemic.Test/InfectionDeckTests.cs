using System.Linq;
using autoCardboard.Pandemic.Domain;
using NUnit.Framework;

namespace autoCardboard.Pandemic.Test
{
    public class InfectionDeckTests
    {
        private PandemicBoard _board;

        [SetUp]
        public void Setup()
        {
            _board = new PandemicBoard();
            _board.Clear();
        }
        
        [Test]
        public void CheckEpidemicRefillsInfectionDeck()
        {
            var drawnInfectionCards = _board.InfectionDeck.Draw(3);
            _board.InfectionDiscardPile.AddCards(drawnInfectionCards);

            _board.Epidemic();
            Assert.AreEqual(_board.InfectionDeck.CardCount, 48);
        }

        [Test]
        public void CheckEpidemicEmptiesInfectionDiscardDeck()
        {
            var drawnInfectionCards = _board.InfectionDeck.Draw(3);
            _board.InfectionDiscardPile.AddCards(drawnInfectionCards);

            _board.Epidemic();
            Assert.AreEqual( _board.InfectionDiscardPile.CardCount, 0);
        }

        [Test]
        public void CheckEpidemicAddsThreeCubesToOneCity()
        {
            var drawnInfectionCards = _board.InfectionDeck.Draw(3);
            _board.InfectionDiscardPile.AddCards(drawnInfectionCards);

            _board.Epidemic();

            var infectedCity = _board.Cities.Single(c => c.DiseaseCubeCount > 0);

            Assert.AreEqual( infectedCity.DiseaseCubeCount, 3);
        }

        [Test]
        public void CheckEpidemicIncrementsInfectionRateMarker()
        {
            var drawnInfectionCards = _board.InfectionDeck.Draw(3);
            _board.InfectionDiscardPile.AddCards(drawnInfectionCards);

            _board.Epidemic();
            Assert.AreEqual( _board.InfectionRateMarker, 1);
        }

    }
}
