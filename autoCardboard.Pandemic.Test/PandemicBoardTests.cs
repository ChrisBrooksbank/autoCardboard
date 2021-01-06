using System.Linq;
using autoCardboard.Pandemic.Domain;
using NUnit.Framework;

namespace autoCardboard.Pandemic.Test
{
    class PandemicBoardTests
    {
        private PandemicBoard _pandemicBoard;

        [SetUp]
        public void Setup()
        {
            _pandemicBoard = new PandemicBoard();
            _pandemicBoard.Clear();
        }

        [Test]
        public void ChicagoHasCorrectConnectionsCount()
        {
            var chicagoNode = _pandemicBoard.Cities.Single(n => n.City == City.Chicago);
            Assert.AreEqual(chicagoNode.ConnectedCities.Count(), 5);
        }

        [Test]
        public void AddingFourBlueDiseasesIncreasesOutbreakCount()
        {
            _pandemicBoard.AddDiseaseCube(Disease.Blue, City.Chicago);
            _pandemicBoard.AddDiseaseCube(Disease.Blue, City.Chicago);
            _pandemicBoard.AddDiseaseCube(Disease.Blue, City.Chicago);
            _pandemicBoard.AddDiseaseCube(Disease.Blue, City.Chicago);
            Assert.AreEqual(_pandemicBoard.OutbreakCount, 1);
        }

        [Test]
        public void Adding2Blue2BlackDiseasesDoesntIncreaseOutbreakCount()
        {
            _pandemicBoard.AddDiseaseCube(Disease.Blue, City.Chicago);
            _pandemicBoard.AddDiseaseCube(Disease.Blue, City.Chicago);
            _pandemicBoard.AddDiseaseCube(Disease.Black, City.Chicago);
            _pandemicBoard.AddDiseaseCube(Disease.Black, City.Chicago);
            Assert.AreEqual(_pandemicBoard.OutbreakCount, 0);
        }

        [Test]
        public void OutBreakInMontrealSpreadsToNewYork()
        {
            _pandemicBoard.AddDiseaseCube(Disease.Blue, City.Montreal);
            _pandemicBoard.AddDiseaseCube(Disease.Blue, City.Montreal);
            _pandemicBoard.AddDiseaseCube(Disease.Blue, City.Montreal);
            _pandemicBoard.AddDiseaseCube(Disease.Blue, City.Montreal);

            var montrealNode = _pandemicBoard.Cities.Single(c => c.City == City.Chicago);
            var NewYork =  _pandemicBoard.Cities.Single(c => c.City == City.NewYork);
            Assert.AreEqual(NewYork.DiseaseCubes[Disease.Blue], 1);
        }
        
        [Test]
        public void AddingDiseaseDecrementsDiseaseCubeStock()
        {
            _pandemicBoard.AddDiseaseCube(Disease.Blue, City.Montreal);
            Assert.AreEqual(_pandemicBoard.DiseaseCubeStock[Disease.Blue], 23);
        }

        [Test]
        public void TreatingDiseaseIncrementsDiseaseCubeStock()
        {
            _pandemicBoard.AddDiseaseCube(Disease.Blue, City.Montreal);
            _pandemicBoard.TreatDisease(Disease.Blue, City.Montreal);
            Assert.AreEqual(_pandemicBoard.DiseaseCubeStock[Disease.Blue], 24);
        }

        [Test]
        public void DoubleOutbreakSetsDiseaseCubeStockOk()
        {
            _pandemicBoard.AddDiseaseCube(Disease.Blue, City.Montreal);
            _pandemicBoard.AddDiseaseCube(Disease.Blue, City.Montreal);
            _pandemicBoard.AddDiseaseCube(Disease.Blue, City.Montreal);

            _pandemicBoard.AddDiseaseCube(Disease.Blue, City.NewYork);
            _pandemicBoard.AddDiseaseCube(Disease.Blue, City.NewYork);
            _pandemicBoard.AddDiseaseCube(Disease.Blue, City.NewYork);

            _pandemicBoard.AddDiseaseCube(Disease.Blue, City.Montreal);

            Assert.AreEqual(_pandemicBoard.DiseaseCubeStock[Disease.Blue], 13);
        }

        [Test]
        public void DoubleOutbreakSetsAddsDiseaseToCorrectCities()
        {
            _pandemicBoard.AddDiseaseCube(Disease.Blue, City.Montreal);
            _pandemicBoard.AddDiseaseCube(Disease.Blue, City.Montreal);
            _pandemicBoard.AddDiseaseCube(Disease.Blue, City.Montreal);

            _pandemicBoard.AddDiseaseCube(Disease.Blue, City.NewYork);
            _pandemicBoard.AddDiseaseCube(Disease.Blue, City.NewYork);
            _pandemicBoard.AddDiseaseCube(Disease.Blue, City.NewYork);

            _pandemicBoard.AddDiseaseCube(Disease.Blue, City.Montreal);

            var citiesWithBlueDisease = _pandemicBoard.Cities.Where(c => c.DiseaseCubes[Disease.Blue] > 0)
                .OrderBy(c => c.City).Select(c=> c.City.ToString()).ToList();
            var citiesWithBlueDiseaseCsv = string.Join(',', citiesWithBlueDisease);
            Assert.AreEqual(citiesWithBlueDiseaseCsv, "Chicago,London,Madrid,Montreal,NewYork,Washington");
        }

        [Test]
        public void InitialInfectionDepletesDiseaseStockpile()
        {
            _pandemicBoard.Setup();

            var diseaseCubesInStockpile = _pandemicBoard.DiseaseCubeStock[Disease.Blue]
                                          + _pandemicBoard.DiseaseCubeStock[Disease.Black]
                                          + _pandemicBoard.DiseaseCubeStock[Disease.Yellow]
                                          + _pandemicBoard.DiseaseCubeStock[Disease.Red];
            Assert.AreEqual(diseaseCubesInStockpile, 78);
        }

        [Test]
        public void InitialInfectionInfectsCities()
        {
            _pandemicBoard.Setup();

            var infectedCities = _pandemicBoard.Cities.Where(c => c.DiseaseCubeCount > 0);

            Assert.AreEqual(infectedCities.Count(), 9);
        }

    }
}
