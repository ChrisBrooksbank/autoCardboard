using System;
using autoCardboard.Pandemic.State;
using autoCardboard.Pandemic.TurnState;

namespace autoCardboard.Pandemic.Test
{
    public class PlayerDeckTests
    {
        private PlayerDeck deckWith6Epidemics;
        private PlayerDeck deckWith5Epidemics;

        public void Setup()
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

        public void IsExpectedDeckSizeWith6Epidemics()
        {
            var cityCount = Enum.GetValues(typeof(City)).Length;
            var eventCount =Enum.GetValues(typeof(EventCard)).Length;

//            Assert.AreEqual(deckWith6Epidemics.CardCount, 6 + cityCount + eventCount);
        }

        public void IsExpectedDeckSizeWith5Epidemics()
        {
            var cityCount = Enum.GetValues(typeof(City)).Length;
            var eventCount= Enum.GetValues(typeof(EventCard)).Length;

  //          Assert.AreEqual(deckWith5Epidemics.CardCount, 5 + cityCount + eventCount);
        }

     
      
    }
}