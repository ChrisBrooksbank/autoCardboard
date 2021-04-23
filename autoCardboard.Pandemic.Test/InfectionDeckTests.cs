using System.Collections.Generic;
using System.Linq;
using autoCardBoard.Pandemic.Bots;
using autoCardboard.Pandemic.State;
using autoCardboard.Pandemic.TurnState;

namespace autoCardboard.Pandemic.Test
{
    public class InfectionDeckTests
    {
        private IPandemicState _gameState;
        private IPandemicStateEditor _stateEditor;

        public void Setup()
        {
            _gameState = new PandemicState();
            _stateEditor = new PandemicStateEditor(new PandemicActionValidator());
            _stateEditor.Clear(_gameState);
            var players = new List<PandemicBotStandard>();
            _stateEditor.Setup(_gameState, players);
        }
        
        public void CheckEpidemicRefillsInfectionDeck()
        {
            var drawnInfectionCards = _gameState.InfectionDeck.Draw(3);
            _gameState.InfectionDiscardPile.AddCards(drawnInfectionCards);

            _stateEditor.Epidemic(_gameState);
            // Assert.AreEqual(_gameState.InfectionDeck.CardCount, 48);
        }

        public void CheckEpidemicEmptiesInfectionDiscardDeck()
        {
            var drawnInfectionCards = _gameState.InfectionDeck.Draw(3);
            _gameState.InfectionDiscardPile.AddCards(drawnInfectionCards);

            _stateEditor.Epidemic(_gameState);
            // Assert.AreEqual(_gameState.InfectionDiscardPile.CardCount, 0);
        }

        public void CheckEpidemicAddsThreeCubesToOneCity()
        {
            _stateEditor.Clear(_gameState);
            var drawnInfectionCards = _gameState.InfectionDeck.Draw(3);
            _gameState.InfectionDiscardPile.AddCards(drawnInfectionCards);

            _stateEditor.Epidemic(_gameState);

            var infectedCity = _gameState.Cities.Single(c => c.DiseaseCubeCount > 0);

            // Assert.AreEqual( infectedCity.DiseaseCubeCount, 3);
        }

        public void CheckEpidemicIncrementsInfectionRateMarker()
        {
            var drawnInfectionCards = _gameState.InfectionDeck.Draw(3);
            _gameState.InfectionDiscardPile.AddCards(drawnInfectionCards);

            _stateEditor.Epidemic(_gameState);
            // Assert.AreEqual(_gameState.InfectionRateMarker, 1);
        }

    }
}
