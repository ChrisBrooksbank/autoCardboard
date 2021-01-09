﻿using System.Linq;
using autoCardboard.Pandemic.Domain;
using NUnit.Framework;

namespace autoCardboard.Pandemic.Test
{
    class PandemicBoardTests
    {
        private PandemicGameState _gameState;

        [SetUp]
        public void Setup()
        {
            _gameState = new PandemicGameState();
            _gameState.Clear();
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
            _gameState.AddDiseaseCube(Disease.Blue, City.Chicago);
            _gameState.AddDiseaseCube(Disease.Blue, City.Chicago);
            _gameState.AddDiseaseCube(Disease.Blue, City.Chicago);
            _gameState.AddDiseaseCube(Disease.Blue, City.Chicago);
            Assert.AreEqual(_gameState.OutbreakCount, 1);
        }

        [Test]
        public void Adding2Blue2BlackDiseasesDoesntIncreaseOutbreakCount()
        {
            _gameState.AddDiseaseCube(Disease.Blue, City.Chicago);
            _gameState.AddDiseaseCube(Disease.Blue, City.Chicago);
            _gameState.AddDiseaseCube(Disease.Black, City.Chicago);
            _gameState.AddDiseaseCube(Disease.Black, City.Chicago);
            Assert.AreEqual(_gameState.OutbreakCount, 0);
        }

        [Test]
        public void OutBreakInMontrealSpreadsToNewYork()
        {
            _gameState.AddDiseaseCube(Disease.Blue, City.Montreal);
            _gameState.AddDiseaseCube(Disease.Blue, City.Montreal);
            _gameState.AddDiseaseCube(Disease.Blue, City.Montreal);
            _gameState.AddDiseaseCube(Disease.Blue, City.Montreal);

            var montrealNode = _gameState.Cities.Single(c => c.City == City.Chicago);
            var NewYork = _gameState.Cities.Single(c => c.City == City.NewYork);
            Assert.AreEqual(NewYork.DiseaseCubes[Disease.Blue], 1);
        }
        
        [Test]
        public void AddingDiseaseDecrementsDiseaseCubeStock()
        {
            _gameState.AddDiseaseCube(Disease.Blue, City.Montreal);
            Assert.AreEqual(_gameState.DiseaseCubeStock[Disease.Blue], 23);
        }

        [Test]
        public void TreatingDiseaseIncrementsDiseaseCubeStock()
        {
            _gameState.AddDiseaseCube(Disease.Blue, City.Montreal);
            _gameState.TreatDisease(Disease.Blue, City.Montreal);
            Assert.AreEqual(_gameState.DiseaseCubeStock[Disease.Blue], 24);
        }

        [Test]
        public void DoubleOutbreakSetsDiseaseCubeStockOk()
        {
            _gameState.AddDiseaseCube(Disease.Blue, City.Montreal);
            _gameState.AddDiseaseCube(Disease.Blue, City.Montreal);
            _gameState.AddDiseaseCube(Disease.Blue, City.Montreal);

            _gameState.AddDiseaseCube(Disease.Blue, City.NewYork);
            _gameState.AddDiseaseCube(Disease.Blue, City.NewYork);
            _gameState.AddDiseaseCube(Disease.Blue, City.NewYork);

            _gameState.AddDiseaseCube(Disease.Blue, City.Montreal);

            Assert.AreEqual(_gameState.DiseaseCubeStock[Disease.Blue], 13);
        }

        [Test]
        public void DoubleOutbreakSetsAddsDiseaseToCorrectCities()
        {
            _gameState.AddDiseaseCube(Disease.Blue, City.Montreal);
            _gameState.AddDiseaseCube(Disease.Blue, City.Montreal);
            _gameState.AddDiseaseCube(Disease.Blue, City.Montreal);

            _gameState.AddDiseaseCube(Disease.Blue, City.NewYork);
            _gameState.AddDiseaseCube(Disease.Blue, City.NewYork);
            _gameState.AddDiseaseCube(Disease.Blue, City.NewYork);

            _gameState.AddDiseaseCube(Disease.Blue, City.Montreal);

            var citiesWithBlueDisease = _gameState.Cities.Where(c => c.DiseaseCubes[Disease.Blue] > 0)
                .OrderBy(c => c.City).Select(c=> c.City.ToString()).ToList();
            var citiesWithBlueDiseaseCsv = string.Join(',', citiesWithBlueDisease);
            Assert.AreEqual(citiesWithBlueDiseaseCsv, "Chicago,London,Madrid,Montreal,NewYork,Washington");
        }

        [Test]
        public void InitialInfectionDepletesDiseaseStockpile()
        {
            _gameState.Setup();

            var diseaseCubesInStockpile = _gameState.DiseaseCubeStock[Disease.Blue]
                                          + _gameState.DiseaseCubeStock[Disease.Black]
                                          + _gameState.DiseaseCubeStock[Disease.Yellow]
                                          + _gameState.DiseaseCubeStock[Disease.Red];
            Assert.AreEqual(diseaseCubesInStockpile, 78);
        }

        [Test]
        public void InitialInfectionInfectsCities()
        {
            _gameState.Setup();

            var infectedCities = _gameState.Cities.Where(c => c.DiseaseCubeCount > 0);

            Assert.AreEqual(infectedCities.Count(), 9);
        }

    }
}
