using autoCardboard.Pandemic.State;
using autoCardboard.Pandemic.TurnState;
using autoCardBoard.Pandemic.Bots;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace autoCardboard.Pandemic.Test
{
    public class PandemicStateTests
    {
        private IPandemicState _gameState;
        private PandemicStateEditor _stateEditor;

        private void Setup()
        {
            _gameState = new PandemicState();
            _stateEditor = new PandemicStateEditor(new PandemicActionValidator());
            _stateEditor.Clear(_gameState);
        }

        [Fact]
        public void ChicagoHasCorrectConnectionsCount()
        {
            Setup();
            var chicagoNode = _gameState.Cities.Single(n => n.City == City.Chicago);
            Assert.Equal(5, chicagoNode.ConnectedCities.Count());
        }

        [Fact]
        public void AddingFourBlueDiseasesIncreasesOutbreakCount()
        {
            Setup();
            _stateEditor.AddDiseaseCube(_gameState, Disease.Blue, City.Chicago);
            _stateEditor.AddDiseaseCube(_gameState, Disease.Blue, City.Chicago);
            _stateEditor.AddDiseaseCube(_gameState, Disease.Blue, City.Chicago);
            _stateEditor.AddDiseaseCube(_gameState, Disease.Blue, City.Chicago);
            Assert.Equal(1, _gameState.OutbreakCount);
        }

        [Fact]
        public void Adding2Blue2BlackDiseasesDoesntIncreaseOutbreakCount()
        {
            Setup();
            _stateEditor.AddDiseaseCube(_gameState, Disease.Blue, City.Chicago);
            _stateEditor.AddDiseaseCube(_gameState, Disease.Blue, City.Chicago);
            _stateEditor.AddDiseaseCube(_gameState, Disease.Black, City.Chicago);
            _stateEditor.AddDiseaseCube(_gameState, Disease.Black, City.Chicago);
            Assert.Equal(0, _gameState.OutbreakCount);
        }

        [Fact]
        public void OutBreakInMontrealSpreadsToNewYork()
        {
            Setup();
            _stateEditor.AddDiseaseCube(_gameState, Disease.Blue, City.Montreal);
            _stateEditor.AddDiseaseCube(_gameState, Disease.Blue, City.Montreal);
            _stateEditor.AddDiseaseCube(_gameState, Disease.Blue, City.Montreal);
            _stateEditor.AddDiseaseCube(_gameState, Disease.Blue, City.Montreal);

            var montrealNode = _gameState.Cities.Single(c => c.City == City.Chicago);
            var NewYork = _gameState.Cities.Single(c => c.City == City.NewYork);
            Assert.Equal(1, NewYork.DiseaseCubes[Disease.Blue]);
        }

        [Fact]
        public void AddingDiseaseDecrementsDiseaseCubeStock()
        {
            Setup();
            _stateEditor.AddDiseaseCube(_gameState, Disease.Blue, City.Montreal);
            Assert.Equal(23, _gameState.DiseaseCubeReserve[Disease.Blue]);
        }

        [Fact]
        public void DoubleOutbreakSetsDiseaseCubeStockOk()
        {
            Setup();
            _stateEditor.AddDiseaseCube(_gameState, Disease.Blue, City.Montreal);
            _stateEditor.AddDiseaseCube(_gameState, Disease.Blue, City.Montreal);
            _stateEditor.AddDiseaseCube(_gameState, Disease.Blue, City.Montreal);

            _stateEditor.AddDiseaseCube(_gameState, Disease.Blue, City.NewYork);
            _stateEditor.AddDiseaseCube(_gameState, Disease.Blue, City.NewYork);
            _stateEditor.AddDiseaseCube(_gameState, Disease.Blue, City.NewYork);

            _stateEditor.AddDiseaseCube(_gameState, Disease.Blue, City.Montreal);

            Assert.Equal(13, _gameState.DiseaseCubeReserve[Disease.Blue]);
        }

        [Fact]
        public void DoubleOutbreakSetsAddsDiseaseToCorrectCities()
        {
            Setup();
            _stateEditor.AddDiseaseCube(_gameState, Disease.Blue, City.Montreal);
            _stateEditor.AddDiseaseCube(_gameState, Disease.Blue, City.Montreal);
            _stateEditor.AddDiseaseCube(_gameState, Disease.Blue, City.Montreal);

            _stateEditor.AddDiseaseCube(_gameState, Disease.Blue, City.NewYork);
            _stateEditor.AddDiseaseCube(_gameState, Disease.Blue, City.NewYork);
            _stateEditor.AddDiseaseCube(_gameState, Disease.Blue, City.NewYork);

            _stateEditor.AddDiseaseCube(_gameState, Disease.Blue, City.Montreal);

            var citiesWithBlueDisease = _gameState.Cities.Where(c => c.DiseaseCubes[Disease.Blue] > 0)
                .OrderBy(c => c.City).Select(c => c.City.ToString()).ToList();
            var citiesWithBlueDiseaseCsv = string.Join(',', citiesWithBlueDisease);
            Assert.Equal("Chicago,London,Madrid,Montreal,NewYork,Washington", citiesWithBlueDiseaseCsv);
        }

        [Fact]
        public void InitialInfectionDepletesDiseaseStockpile()
        {
            Setup();
            var players = new List<PandemicBotStandard>();
            _stateEditor.Setup(_gameState, players);

            var diseaseCubesInStockpile = _gameState.DiseaseCubeReserve[Disease.Blue]
                                          + _gameState.DiseaseCubeReserve[Disease.Black]
                                          + _gameState.DiseaseCubeReserve[Disease.Yellow]
                                          + _gameState.DiseaseCubeReserve[Disease.Red];
            Assert.Equal(78, diseaseCubesInStockpile);
        }

        [Fact]
        public void InitialInfectionInfectsCities()
        {
            Setup();
            var players = new List<PandemicBotStandard>();
            _stateEditor.Setup(_gameState, players);

            var infectedCities = _gameState.Cities.Where(c => c.DiseaseCubeCount > 0);

            Assert.Equal(9, infectedCities.Count());
        }

    }
}
