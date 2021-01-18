using System;
using autoCardboard.Common;
using System.Collections.Generic;
using System.Linq;
using autoCardboard.Infrastructure;

namespace autoCardboard.Pandemic
{
    [Serializable]
    /// Represents the game state of a game of Pandemic
    /// TODO consider if should move code out such as Epidemic(), InfectCities(), PerformInitialInfections(), AddDiseaseCubes() somewhere else... or leave here
    public class PandemicGameState: GameState, IPandemicGameState
    {
        [NonSerialized]
        private ICardboardLogger _logger;

        public Dictionary<int, PandemicPlayerState> PlayerStates { get; set; }

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

        public Dictionary<Disease, DiseaseState> DiscoveredCures { get; set; }

        public Boolean IsGameOver { get; set; }

        public PandemicGameState(ICardboardLogger logger)
        {
            _logger = logger;
            PlayerStates = new Dictionary<int, PandemicPlayerState>();
        }

        public IEnumerable<PandemicPlayerCard> DrawPlayerCards()
        {
            if (PlayerDeck.CardCount < 2)
            {
                _logger.Information("Game over. Empty playerdeck");
                IsGameOver = true;
                return new List<PandemicPlayerCard>();
            }

            return PlayerDeck.Draw(2);
        }

        public void Epidemic()
        {
            var bottomCard = InfectionDeck.DrawBottom();
            _logger.Information($"Epidemic in {(City)bottomCard.Value}");
            InfectionDiscardPile.AddCard(bottomCard);
            AddDiseaseCubes((City)bottomCard.Value, 3);

            // Intensify
            InfectionDiscardPile.Shuffle();
            InfectionDeck.AddCardDeck(InfectionDiscardPile, CardDeckPosition.Top);

            InfectionRateMarker++;
        }

        public void InfectCities()
        {
            var infectionRate = InfectionRateTrack[InfectionRateMarker];
            var infectionCards = InfectionDeck.Draw(infectionRate).ToList();

            foreach (var infectionCard in infectionCards)
            {
                var city = (City)infectionCard.Value;
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
            InfectionRateTrack = new int[] { 2, 2, 2, 3, 3, 4, 4 };
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

            DiscoveredCures = new Dictionary<Disease, DiseaseState>
            {
                {Disease.Blue, DiseaseState.NotCured},
                {Disease.Black, DiseaseState.NotCured},
                {Disease.Red, DiseaseState.NotCured },
                {Disease.Yellow, DiseaseState.NotCured}
            };
        }

        public void Setup(IEnumerable<IPlayer<IPandemicTurn>> players, int pandemicCardCount = 6)
        {
            Clear();
            SetupPlayerStates(players);
            PerformInitialInfections();
        }

        private void SetupPlayerStates(IEnumerable<IPlayer<IPandemicTurn>> players)
        {
            var roleDeck = new RoleDeck();

            PlayerStates = new Dictionary<int, PandemicPlayerState>();
            foreach (var player in players)
            {
                PlayerStates[player.Id] = new PandemicPlayerState
                {
                    PlayerHand = new List<PandemicPlayerCard>(), // TODO start with 4 cards
                    Location = City.Atlanta,
                    PlayerRole = (PlayerRole)roleDeck.DrawTop().Value
                };
            }
        }

        private void PerformInitialInfections()
        {
            _logger.Information("Starting initial infections");

            var infectionCards = InfectionDeck.Draw(3).ToList();
            InfectionDiscardPile.AddCards(infectionCards);
            foreach (var infectionCard in infectionCards)
            {
                AddDiseaseCubes((City)infectionCard.Value, 3);
            }

            infectionCards = InfectionDeck.Draw(3).ToList();
            InfectionDiscardPile.AddCards(infectionCards);
            foreach (var infectionCard in infectionCards)
            {
                AddDiseaseCubes((City)infectionCard.Value, 2);
            }

            infectionCards = InfectionDeck.Draw(3).ToList();
            InfectionDiscardPile.AddCards(infectionCards);
            foreach (var infectionCard in infectionCards)
            {
                AddDiseaseCubes((City)infectionCard.Value, 1);
            }

            _logger.Information("Finished initial infections");
        }

        public void AddDiseaseCubes(City city, int count = 1)
        {
            var disease = city.GetDefaultDisease();

            for (var i = 0; i < count; i++)
            {
                AddDiseaseCube(disease, city);
            }
        }

        private PandemicPlayerState GetPlayerStateByRole(PlayerRole playerRole)
        {
            foreach (var playerState in PlayerStates)
            {
                if (playerState.Value.PlayerRole == playerRole)
                {
                    return playerState.Value;
                }
            }
            return null;
        }

        public void AddDiseaseCube(Disease disease, City city, List<City> ignoreCities = null)
        {
            var node = Cities.Single(n => n.City == city);


            // Dont place disease if quarantineSpecialist is here or in neighbouring city
            var quarantineSpecialist = GetPlayerStateByRole(PlayerRole.QuarantineSpecialist);
            if (quarantineSpecialist != null)
            {
                if (quarantineSpecialist.Location == city || Cities.Single(n => n.City == quarantineSpecialist.Location).ConnectedCities.Contains(city))
                {
                    _logger.Information($"quarantineSpecialist in {quarantineSpecialist.Location} prevented {disease} in {city}");
                    return;
                }
            }

            _logger.Information($"Adding {disease} to {city}");

            ignoreCities = ignoreCities ?? new List<City>();

            if (node.DiseaseCubes[disease] < 3)
            {
                if (_diseaseCubeStock[disease] == 0)
                {
                    _logger.Information($"Game over. No cubes left for {disease}");
                    IsGameOver = true;
                    return;
                }

                node.DiseaseCubes[disease] += 1;
                _diseaseCubeStock[disease]--;

                return;
            }

            _logger.Information($"Outbreak in {node.City}");
            _outbreakCount++;

            if (_outbreakCount > 7)
            {
                _logger.Information($"Game over. Too many outbreaks");
                IsGameOver = true;
                return;
            }

            ignoreCities.Add(city);
            foreach (var connectedCity in node.ConnectedCities.Where(c => !ignoreCities.Contains(c)))
            {
                AddDiseaseCube(disease, connectedCity, ignoreCities);
            }

            _logger.Information($"Finished adding {disease} to {city}");
        }

    }
}
