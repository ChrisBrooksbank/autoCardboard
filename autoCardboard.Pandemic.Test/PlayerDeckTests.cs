using autoCardboard.Pandemic.State;
using autoCardboard.Pandemic.TurnState;
using System;
using Xunit;

namespace autoCardboard.Pandemic.Test
{
    public class PlayerDeckTests
    {
        private PlayerDeck deckWith6Epidemics;
        private PlayerDeck deckWith5Epidemics;

        private void Setup()
        {
            var pandemicStateEditor = new PandemicStateEditor(new PandemicActionValidator());
            var statewith6Epidemics = new PandemicState();
            statewith6Epidemics.PandemicCardCount = 6;
            pandemicStateEditor.SetupPlayerDeck(statewith6Epidemics);
            deckWith6Epidemics = statewith6Epidemics.PlayerDeck;

            var statewith5Epidemics = new PandemicState();
            statewith5Epidemics.PandemicCardCount = 5;
            pandemicStateEditor.SetupPlayerDeck(statewith5Epidemics);
            deckWith5Epidemics = statewith5Epidemics.PlayerDeck;
        }

        [Fact]
        public void IsExpectedDeckSizeWith6Epidemics()
        {
            Setup();
            var cityCount = Enum.GetValues(typeof(City)).Length;
            var eventCount = Enum.GetValues(typeof(EventCard)).Length;

            Assert.Equal(deckWith6Epidemics.CardCount, 6 + cityCount + eventCount);
        }

        [Fact]
        public void IsExpectedDeckSizeWith5Epidemics()
        {
            Setup();
            var cityCount = Enum.GetValues(typeof(City)).Length;
            var eventCount = Enum.GetValues(typeof(EventCard)).Length;

            Assert.Equal(deckWith5Epidemics.CardCount, 5 + cityCount + eventCount);
        }



    }
}