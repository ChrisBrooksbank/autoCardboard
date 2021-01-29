using System.Collections.Generic;
using System.Linq;
using autoCardboard.Infrastructure;
using autoCardboard.Messaging;
using autoCardBoard.Pandemic.Bots;
using autoCardboard.Pandemic.State;
using autoCardboard.Pandemic.TurnState;
using NUnit.Framework;

namespace autoCardboard.Pandemic.Test
{
    class PandemicStateTests
    {
        private IPandemicState _gameState;
        private IPandemicStateEditor _stateEditor;

        [SetUp]
        public void Setup()
        {
            _gameState = new PandemicState();
            _stateEditor = new PandemicStateEditor(new CardboardLogger(), new MessageSender());
            _stateEditor.Clear(_gameState);
        }

        [Test]
        public void ChicagoHasCorrectConnectionsCount()
        {
            var chicagoNode = _gameState.Cities.Single(n => n.City == City.Chicago);
            Assert.AreEqual(chicagoNode.ConnectedCities.Count(), 5);
        }

        [Test]
        public void AddingFourBlueDiseasesIncreasesOutbreakCount()
        {
            _stateEditor.AddDiseaseCube(_gameState, Disease.Blue, City.Chicago);
            _stateEditor.AddDiseaseCube(_gameState,Disease.Blue, City.Chicago);
            _stateEditor.AddDiseaseCube(_gameState,Disease.Blue, City.Chicago);
            _stateEditor.AddDiseaseCube(_gameState,Disease.Blue, City.Chicago);
            Assert.AreEqual(_gameState.OutbreakCount, 1);
        }

        [Test]
        public void Adding2Blue2BlackDiseasesDoesntIncreaseOutbreakCount()
        {
            _stateEditor.AddDiseaseCube(_gameState,Disease.Blue, City.Chicago);
            _stateEditor.AddDiseaseCube(_gameState,Disease.Blue, City.Chicago);
            _stateEditor.AddDiseaseCube(_gameState,Disease.Black, City.Chicago);
            _stateEditor.AddDiseaseCube(_gameState,Disease.Black, City.Chicago);
            Assert.AreEqual(_gameState.OutbreakCount, 0);
        }

        [Test]
        public void OutBreakInMontrealSpreadsToNewYork()
        {
            _stateEditor.AddDiseaseCube(_gameState,Disease.Blue, City.Montreal);
            _stateEditor.AddDiseaseCube(_gameState,Disease.Blue, City.Montreal);
            _stateEditor.AddDiseaseCube(_gameState,Disease.Blue, City.Montreal);
            _stateEditor.AddDiseaseCube(_gameState,Disease.Blue, City.Montreal);

            var montrealNode = _gameState.Cities.Single(c => c.City == City.Chicago);
            var NewYork = _gameState.Cities.Single(c => c.City == City.NewYork);
            Assert.AreEqual(NewYork.DiseaseCubes[Disease.Blue], 1);
        }
        
        [Test]
        public void AddingDiseaseDecrementsDiseaseCubeStock()
        {
            _stateEditor.AddDiseaseCube(_gameState,Disease.Blue, City.Montreal);
            Assert.AreEqual(_gameState.DiseaseCubeReserve[Disease.Blue], 23);
        }

        [Test]
        public void DoubleOutbreakSetsDiseaseCubeStockOk()
        {
            _stateEditor.AddDiseaseCube(_gameState,Disease.Blue, City.Montreal);
            _stateEditor.AddDiseaseCube(_gameState,Disease.Blue, City.Montreal);
            _stateEditor.AddDiseaseCube(_gameState,Disease.Blue, City.Montreal);

            _stateEditor.AddDiseaseCube(_gameState,Disease.Blue, City.NewYork);
            _stateEditor.AddDiseaseCube(_gameState,Disease.Blue, City.NewYork);
            _stateEditor.AddDiseaseCube(_gameState,Disease.Blue, City.NewYork);

            _stateEditor.AddDiseaseCube(_gameState,Disease.Blue, City.Montreal);

            Assert.AreEqual(_gameState.DiseaseCubeReserve[Disease.Blue], 13);
        }

        [Test]
        public void DoubleOutbreakSetsAddsDiseaseToCorrectCities()
        {
            _stateEditor.AddDiseaseCube(_gameState,Disease.Blue, City.Montreal);
            _stateEditor.AddDiseaseCube(_gameState,Disease.Blue, City.Montreal);
            _stateEditor.AddDiseaseCube(_gameState,Disease.Blue, City.Montreal);

            _stateEditor.AddDiseaseCube(_gameState,Disease.Blue, City.NewYork);
            _stateEditor.AddDiseaseCube(_gameState,Disease.Blue, City.NewYork);
            _stateEditor.AddDiseaseCube(_gameState,Disease.Blue, City.NewYork);

            _stateEditor.AddDiseaseCube(_gameState,Disease.Blue, City.Montreal);

            var citiesWithBlueDisease = _gameState.Cities.Where(c => c.DiseaseCubes[Disease.Blue] > 0)
                .OrderBy(c => c.City).Select(c=> c.City.ToString()).ToList();
            var citiesWithBlueDiseaseCsv = string.Join(',', citiesWithBlueDisease);
            Assert.AreEqual(citiesWithBlueDiseaseCsv, "Chicago,London,Madrid,Montreal,NewYork,Washington");
        }

        [Test]
        public void InitialInfectionDepletesDiseaseStockpile()
        {
            var players = new List<PandemicBotStandard>();
            _stateEditor.Setup(_gameState,players);

            var diseaseCubesInStockpile = _gameState.DiseaseCubeReserve[Disease.Blue]
                                          + _gameState.DiseaseCubeReserve[Disease.Black]
                                          + _gameState.DiseaseCubeReserve[Disease.Yellow]
                                          + _gameState.DiseaseCubeReserve[Disease.Red];
            Assert.AreEqual(diseaseCubesInStockpile, 78);
        }

        [Test]
        public void InitialInfectionInfectsCities()
        {
            var players = new List<PandemicBotStandard>();
            _stateEditor.Setup(_gameState,players);

            var infectedCities = _gameState.Cities.Where(c => c.DiseaseCubeCount > 0);

            Assert.AreEqual(infectedCities.Count(), 9);
        }

    }
}
