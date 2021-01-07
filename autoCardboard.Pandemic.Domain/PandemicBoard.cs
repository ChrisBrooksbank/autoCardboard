using autoCardboard.Common.Domain.Cards;
using System;
using System.Collections.Generic;
using System.Linq;

namespace autoCardboard.Pandemic.Domain
{
    [Serializable]
    public class PandemicBoard
    {
        private int _outbreakCount;
        public int OutbreakCount => _outbreakCount;

        public List<MapNode> Cities { get; set; }

        private Dictionary<Disease, int> _diseaseCubeStock;

        public Dictionary<Disease, int> DiseaseCubeStock => _diseaseCubeStock;

        public InfectionDeck InfectionDeck { get; set; }
        public CardDeck<Card> InfectionDiscardPile { get; set; }
        public PlayerDeck PlayerDeck { get; set; }
        public PlayerDeck PlayerDiscardPile { get; set; }
        public int InfectionRateMarker { get; set; }
        public int[] InfectionRateTrack { get; set; }
        public int EpidemicCardCount { get; set; }

        public Dictionary<Disease,DiseaseState> DiscoveredCures { get; set; }

        public Boolean IsGameOver { get; set; }

        public IEnumerable<PandemicPlayerCard> DrawPlayerCards()
        {
            if (PlayerDeck.CardCount < 2)
            {
                Console.WriteLine("Game over. Empty playerdeck");
                IsGameOver = true;
                return new List<PandemicPlayerCard>();
            }

            return PlayerDeck.Draw(2);
        }

        public void Epidemic()
        {
            var bottomCard = InfectionDeck.DrawBottom();
            Console.WriteLine($"Epidemic in {(City)bottomCard.Value}");
            InfectionDiscardPile.AddCard(bottomCard);
            AddDiseaseCubes((City)bottomCard.Value,3);

            // Intensify
            InfectionDiscardPile.Shuffle();
            InfectionDeck.AddCardDeck(InfectionDiscardPile,CardDeckPosition.Top);

            InfectionRateMarker++;
        }

        public void Infect()
        {
            var infectionRate = InfectionRateTrack[InfectionRateMarker];
            var infectionCards = InfectionDeck.Draw(infectionRate).ToList();

            foreach (var infectionCard in infectionCards)
            {
                var city = (City) infectionCard.Value;
                var disease = city.GetDefaultDisease();
                AddDiseaseCube(disease, city);
            }

            InfectionDiscardPile.AddCards(infectionCards);
        }


        public void Clear(int pandemicCardCount = 6)
        {
            IsGameOver = false;
            Cities = new List<MapNode>();

            InfectionDeck = new InfectionDeck();
            InfectionDiscardPile = new CardDeck<Card>();
            PlayerDeck = new PlayerDeck();
            PlayerDeck.Setup(pandemicCardCount);

            PlayerDiscardPile = new PlayerDeck();

            DiscoveredCures = new Dictionary<Disease, DiseaseState>();
            InfectionRateMarker = 0;
            InfectionRateTrack = new int[] {2,2,2,3,3,4,4};
            EpidemicCardCount = pandemicCardCount;

            var nodeFactory = new MapNodeFactory();

            var cities = Enum.GetValues(typeof(City));
            foreach (var city in cities)
            {
                Cities.Add(nodeFactory.CreateMapNode((City)city));
            }

            _diseaseCubeStock = new Dictionary<Disease, int>
            {
                {Disease.Blue, 24},
                {Disease.Black, 24},
                {Disease.Red, 24 },
                {Disease.Yellow, 24}
            };
        }

        public void Setup(int pandemicCardCount = 6)
        {
            Clear();
            PerformInitialInfections();
        }
        
        private void PerformInitialInfections()
        {
            Console.WriteLine("Starting initial infections");

            var infectionCards = InfectionDeck.Draw(3).ToList();
            InfectionDiscardPile.AddCards(infectionCards);
            foreach (var infectionCard in infectionCards)
            { 
                AddDiseaseCubes((City)infectionCard.Value,3);
            }

            infectionCards = InfectionDeck.Draw(3).ToList();
            InfectionDiscardPile.AddCards(infectionCards);
            foreach (var infectionCard in infectionCards)
            { 
                AddDiseaseCubes((City)infectionCard.Value,2);
            }

            infectionCards = InfectionDeck.Draw(3).ToList();
            InfectionDiscardPile.AddCards(infectionCards);
            foreach (var infectionCard in infectionCards)
            { 
                AddDiseaseCubes((City)infectionCard.Value,1);
            }

            Console.WriteLine("Finished initial infections");
        }

        // TODO what if disease is cured, you then remove all
        // what if player role allows removal of all cubes
        public void TreatDisease(Disease disease, City city)
        {
            var node = Cities.Single(c => c.City == city);

            if (disease == 0)
            {
                return;
            }

            node.DiseaseCubes[disease]--;
            _diseaseCubeStock[disease]++;
        }

        public void AddDiseaseCubes(City city, int count = 1)
        {
            var disease = city.GetDefaultDisease();

            for (var i = 0; i < count; i++)
            {
                AddDiseaseCube(disease, city);
            }
        }

        public void AddDiseaseCube(Disease disease, City city, List<City> ignoreCities = null)
        {
            Console.WriteLine($"Adding {disease} to {city}");

            ignoreCities = ignoreCities ?? new List<City>();

            var node = Cities.Single(n => n.City == city);

            if (node.DiseaseCubes[disease] < 3)
            {
                if (_diseaseCubeStock[disease] == 0)
                {
                    Console.WriteLine($"Game over. No cubes left for {disease}");
                    IsGameOver = true;
                    return;
                }

                node.DiseaseCubes[disease] += 1;
                _diseaseCubeStock[disease]--;

                return;
            }

            Console.WriteLine($"Outbreak in {node.City}");
            _outbreakCount++;

            if (_outbreakCount > 7)
            {
                Console.WriteLine($"Game over. Too many outbreaks");
                IsGameOver = true;
                return;
            }

            ignoreCities.Add(city);
            foreach (var connectedCity in node.ConnectedCities.Where(c => !ignoreCities.Contains(c)))
            {
                AddDiseaseCube(disease,connectedCity, ignoreCities);
            }

            Console.WriteLine($"Finished adding {disease} to {city}");
        }
        
    }
}
