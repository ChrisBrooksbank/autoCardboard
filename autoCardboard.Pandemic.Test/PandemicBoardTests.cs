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
            _pandemicBoard.Clear();
            _pandemicBoard.AddDiseaseCube(Disease.Blue, City.Chicago);
            _pandemicBoard.AddDiseaseCube(Disease.Blue, City.Chicago);
            _pandemicBoard.AddDiseaseCube(Disease.Blue, City.Chicago);
            _pandemicBoard.AddDiseaseCube(Disease.Blue, City.Chicago);
            Assert.AreEqual(_pandemicBoard.OutbreakCount, 1);
        }

        [Test]
        public void Adding2Blue2BlackDiseasesDoesntIncreaseOutbreakCount()
        {
            _pandemicBoard.Clear();
            _pandemicBoard.AddDiseaseCube(Disease.Blue, City.Chicago);
            _pandemicBoard.AddDiseaseCube(Disease.Blue, City.Chicago);
            _pandemicBoard.AddDiseaseCube(Disease.Black, City.Chicago);
            _pandemicBoard.AddDiseaseCube(Disease.Black, City.Chicago);
            Assert.AreEqual(_pandemicBoard.OutbreakCount, 0);
        }

        [Test]
        public void OutBreakInMontrealSpreadsToNewYork()
        {
            _pandemicBoard.Clear();
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
            _pandemicBoard.Clear();
            _pandemicBoard.AddDiseaseCube(Disease.Blue, City.Montreal);
            Assert.AreEqual(_pandemicBoard.DiseaseCubeStock[Disease.Blue], 23);
        }

        [Test]
        public void TreatingDiseaseIncrementsDiseaseCubeStock()
        {
            _pandemicBoard.Clear();
            _pandemicBoard.AddDiseaseCube(Disease.Blue, City.Montreal);
            _pandemicBoard.TreatDisease(Disease.Blue, City.Montreal);
            Assert.AreEqual(_pandemicBoard.DiseaseCubeStock[Disease.Blue], 24);
        }

        [Test]
        public void DoubleOutbreakSetsDiseaseCubeStockOk()
        {
            _pandemicBoard.Clear();
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
            _pandemicBoard.Clear();
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


    }
}
