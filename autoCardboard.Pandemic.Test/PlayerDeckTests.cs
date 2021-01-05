using System;
using System.Linq;
using autoCardboard.Pandemic.Domain;
using NUnit.Framework;

namespace autoCardboard.Pandemic.Test
{
    public class PlayerDeckTests
    {
        private PlayerDeck deckWith6Epidemics;
        private PlayerDeck deckWith5Epidemics;

        [SetUp]
        public void Setup()
        {
            deckWith6Epidemics = new PlayerDeck();
            deckWith6Epidemics.Setup(6);

            deckWith5Epidemics = new PlayerDeck();
            deckWith5Epidemics.Setup(5);
        }

        [Test]
        public void IsExpectedDeckSizeWith6Epidemics()
        {
            var cityCount = Enum.GetValues(typeof(City)).Length;
            var eventCount =Enum.GetValues(typeof(EventCard)).Length;

            Assert.AreEqual(deckWith6Epidemics.CardCount, 6 + cityCount + eventCount);
        }

        [Test]
        public void IsExpectedDeckSizeWith5Epidemics()
        {
            var cityCount = Enum.GetValues(typeof(City)).Length;
            var eventCount= Enum.GetValues(typeof(EventCard)).Length;

            Assert.AreEqual(deckWith5Epidemics.CardCount, 5 + cityCount + eventCount);
        }

     
      
    }
}