using autoCardboard.Common;
using autoCardboard.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace autoCardboard.Pandemic
{
    public class PandemicStateEditor: IPandemicStateEditor
    {
        private readonly ICardboardLogger _log;

        private int _currentPlayerId;

        public IPandemicState State { get; set; }

        public PandemicStateEditor(ICardboardLogger logger)
        {
            _log = logger;
        }

        public void Setup(IEnumerable<IPlayer<IPandemicTurn>> players, int pandemicCardCount = 6)
        {
            Clear();
            State.PandemicCardCount = pandemicCardCount;
            SetupPlayerStates(players);
            PerformInitialInfections();
        }

        public void Clear()
        {
            State.IsGameOver = false;
            State.Cities = new List<MapNode>();

            State.InfectionDeck = new InfectionDeck();
            State.InfectionDiscardPile = new CardDeck<Card>();
            State.PlayerDeck = new PlayerDeck();
            State.PandemicCardCount = 6;
            State.PlayerDeck.Setup(State.PandemicCardCount);

            State.PlayerDiscardPile = new PlayerDeck();

            State.DiscoveredCures = new Dictionary<Disease, DiseaseState>();
            State.InfectionRateMarker = 0;
            State.InfectionRateTrack = new int[] { 2, 2, 2, 3, 3, 4, 4 };

            var nodeFactory = new MapNodeFactory();

            var cities = Enum.GetValues(typeof(City));
            foreach (var city in cities)
            {
                State.Cities.Add(nodeFactory.CreateMapNode((City)city));
            }

            State.DiseaseCubeStock = new Dictionary<Disease, int>
            {
                {Disease.Blue, 24},
                {Disease.Black, 24},
                {Disease.Red, 24 },
                {Disease.Yellow, 24}
            };

            State.DiscoveredCures = new Dictionary<Disease, DiseaseState>
            {
                {Disease.Blue, DiseaseState.NotCured},
                {Disease.Black, DiseaseState.NotCured},
                {Disease.Red, DiseaseState.NotCured },
                {Disease.Yellow, DiseaseState.NotCured}
            };
        }

        public void TakeTurn(IPandemicTurn turn)
        {
            _currentPlayerId = turn.CurrentPlayerId;
            foreach (var action in turn.ActionsTaken)
            {
                TakeAction(action);
            }
        }

        // TODO complete and refactor somewhere more sensible
        private void OutputGameState()
        {
            //_log.Information("Game State : ");
        }


        private void TakeAction(PlayerActionWithCity action)
        {
            // TODO solid, inject IEnumerable<IPlayerActionHandler>
            switch(action.PlayerAction)
            {
                case PlayerStandardAction.TreatDisease:
                    TreatDisease(action.City, action.Disease);
                    break;
                case PlayerStandardAction.DriveOrFerry:
                    DriveOrFerry(action.City);
                    break;
            }

            OutputGameState();
        }

        private void DriveOrFerry(City city)
        {
            var node = State.Cities.Single(c => c.City == city);
            var playerState = State.PlayerStates[_currentPlayerId];
            playerState.Location = city;
        }

        private void TreatDisease(City city, Disease disease)
        {
            var diseaseState = State.DiscoveredCures[disease];
            var currentPlayerRole = State.PlayerStates[_currentPlayerId].PlayerRole;

            var node = State.Cities.Single(c => c.City == city);

            if (disease == 0)
            {
                return;
            }

            if (diseaseState == DiseaseState.Cured || currentPlayerRole == PlayerRole.Medic)
            {
                State.DiseaseCubeStock[disease] += node.DiseaseCubes[disease];
                node.DiseaseCubes[disease] = 0;
            }
            else
            {
                node.DiseaseCubes[disease]--;
                State.DiseaseCubeStock[disease]++;
            }
          
        }

        public void Setup()
        {
            throw new System.NotImplementedException();
        }

        public void Epidemic()
        {
            var bottomCard = State.InfectionDeck.DrawBottom();
            _log.Information($"Epidemic in {(City)bottomCard.Value}");
            State.InfectionDiscardPile.AddCard(bottomCard);
            AddDiseaseCubes((City)bottomCard.Value, 3);

            // Intensify
            State.InfectionDiscardPile.Shuffle();
            State.InfectionDeck.AddCardDeck(State.InfectionDiscardPile, CardDeckPosition.Top);

            State.InfectionRateMarker++;
        }

        public void InfectCities()
        {
            var infectionRate = State.InfectionRateTrack[State.InfectionRateMarker];
            var infectionCards = State.InfectionDeck.Draw(infectionRate).ToList();

            foreach (var infectionCard in infectionCards)
            {
                var city = (City)infectionCard.Value;
                var disease = city.GetDefaultDisease();
                AddDiseaseCube(disease, city);
            }

            State.InfectionDiscardPile.AddCards(infectionCards);
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
            var node = State.Cities.Single(n => n.City == city);


            // Dont place disease if quarantineSpecialist is here or in neighbouring city
            var quarantineSpecialist = GetPlayerStateByRole(PlayerRole.QuarantineSpecialist);
            if (quarantineSpecialist != null)
            {
                if (quarantineSpecialist.Location == city || State.Cities.Single(n => n.City == quarantineSpecialist.Location).ConnectedCities.Contains(city))
                {
                    _log.Information($"quarantineSpecialist in {quarantineSpecialist.Location} prevented {disease} in {city}");
                    return;
                }
            }

            _log.Information($"Adding {disease} to {city}");

            ignoreCities = ignoreCities ?? new List<City>();

            if (node.DiseaseCubes[disease] < 3)
            {
                if (State.DiseaseCubeStock[disease] == 0)
                {
                    _log.Information($"Game over. No cubes left for {disease}");
                    State.IsGameOver = true;
                    return;
                }

                node.DiseaseCubes[disease] += 1;
                State.DiseaseCubeStock[disease]--;

                return;
            }

            _log.Information($"Outbreak in {node.City}");
            State.OutbreakCount++;

            if (State.OutbreakCount > 7)
            {
                _log.Information($"Game over. Too many outbreaks");
                State.IsGameOver = true;
                return;
            }

            ignoreCities.Add(city);
            foreach (var connectedCity in node.ConnectedCities.Where(c => !ignoreCities.Contains(c)))
            {
                AddDiseaseCube(disease, connectedCity, ignoreCities);
            }

            _log.Information($"Finished adding {disease} to {city}");
        }

        private void SetupPlayerStates(IEnumerable<IPlayer<IPandemicTurn>> players)
        {
            var roleDeck = new RoleDeck();

            State.PlayerStates = new Dictionary<int, PandemicPlayerState>();
            foreach (var player in players)
            {
                State.PlayerStates[player.Id] = new PandemicPlayerState
                {
                    PlayerHand = new List<PandemicPlayerCard>(), // TODO start with 4 cards
                    Location = City.Atlanta,
                    PlayerRole = (PlayerRole)roleDeck.DrawTop().Value
                };
            }
        }

        private PandemicPlayerState GetPlayerStateByRole(PlayerRole playerRole)
        {
            foreach (var playerState in State.PlayerStates)
            {
                if (playerState.Value.PlayerRole == playerRole)
                {
                    return playerState.Value;
                }
            }
            return null;
        }

        private void PerformInitialInfections()
        {
            _log.Information("Starting initial infections");

            var infectionCards = State.InfectionDeck.Draw(3).ToList();
            State.InfectionDiscardPile.AddCards(infectionCards);
            foreach (var infectionCard in infectionCards)
            {
                AddDiseaseCubes((City)infectionCard.Value, 3);
            }

            infectionCards = State.InfectionDeck.Draw(3).ToList();
            State.InfectionDiscardPile.AddCards(infectionCards);
            foreach (var infectionCard in infectionCards)
            {
                AddDiseaseCubes((City)infectionCard.Value, 2);
            }

            infectionCards = State.InfectionDeck.Draw(3).ToList();
            State.InfectionDiscardPile.AddCards(infectionCards);
            foreach (var infectionCard in infectionCards)
            {
                AddDiseaseCubes((City)infectionCard.Value, 1);
            }

            _log.Information("Finished initial infections");
        }

        public IEnumerable<PandemicPlayerCard> DrawPlayerCards()
        {
            if (State.PlayerDeck.CardCount < 2)
            {
                _log.Information("Game over. Empty playerdeck");
                State.IsGameOver = true;
                return new List<PandemicPlayerCard>();
            }

            return State.PlayerDeck.Draw(2);
        }

    }
}
