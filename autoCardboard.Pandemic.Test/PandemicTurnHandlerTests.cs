using autoCardboard.Pandemic.Domain;
using NUnit.Framework;
using System.Linq;
using pandemicPlayerFactory = autoCardboard.Pandemic.Domain.PlayerFactory;

namespace autoCardboard.Pandemic.Test
{
    class PandemicTurnHandlerTests
    {
        private PandemicGameState _gameState;
        private PandemicTurnHandler _turnHandler;
        private PandemicTurn _turn;

        [SetUp]
        public void Setup()
        {
            _gameState = new PandemicGameState();
            _gameState.Clear();

            var pandemicPlayerFactory = new pandemicPlayerFactory();
            var pandemicPlayers = pandemicPlayerFactory.CreatePlayers(2).ToList();
            _gameState.Setup(pandemicPlayers);

            _turnHandler = new PandemicTurnHandler();

            _turn = new PandemicTurn();
            _turn.CurrentPlayerId = pandemicPlayers[0].Id;
            _turn.State = _gameState;
        }

        [Test]
        public void TreatingDiseaseIncrementsDiseaseCubeStock()
        {
            _gameState.Clear();
            _gameState.AddDiseaseCube(Disease.Blue, City.Atlanta);
            _turn.TreatDisease(Disease.Blue);
            _turnHandler.TakeTurn(_gameState, _turn);

            Assert.AreEqual(24, _gameState.DiseaseCubeStock[Disease.Blue]);
        }

    }
}
