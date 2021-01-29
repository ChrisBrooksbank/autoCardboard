using System;
using System.Collections.Generic;
using System.Linq;
using autoCardboard.Common;
using autoCardboard.Infrastructure;
using autoCardboard.Messaging;
using autoCardboard.Pandemic.State;

namespace autoCardboard.Pandemic.TurnState
{
      public class PandemicStateEditor: IPandemicStateEditor
    {
        private readonly ICardboardLogger _log;

        private int _currentPlayerId;
        private IPandemicState _state;
        private IMessageSender _messageSender;

        public PandemicStateEditor(ICardboardLogger logger, IMessageSender messageSender)
        {
            _log = logger;
            _messageSender = messageSender;
        }

        public void Setup(IPandemicState state, IEnumerable<IPlayer<IPandemicTurn>> players, int pandemicCardCount = 6)
        {
            _state = state;
            Clear(_state);
            _state.PandemicCardCount = pandemicCardCount;
            SetupPlayerStates(_state, players);
            PerformInitialInfections(_state);
            _messageSender.SendMessageASync("AutoCardboard/Pandemic/StateEditor", "Setup");
        }

        public void Clear(IPandemicState state, int pandemicCardCount = 6)
        {
            _state = state;
            _state.TurnsPlayed = 0;
            var players = new List<IPlayer<IPandemicTurn>>();
            SetupPlayerStates(_state, players);
            _state.IsGameOver = false;
            _state.Cities = new List<MapNode>();

            _state.InfectionDeck = new InfectionDeck();
            _state.InfectionDiscardPile = new CardDeck<Card>();
            _state.PlayerDeck = new PlayerDeck();
            _state.PandemicCardCount = pandemicCardCount;
            _state.PlayerDiscardPile = new PlayerDeck();
            SetupPlayerDeck(_state);

            _state.DiscoveredCures = new Dictionary<Disease, DiseaseState>();
            _state.InfectionRateMarker = 0;
            _state.InfectionRateTrack = new int[] { 2, 2, 2, 3, 3, 4, 4 };

            var nodeFactory = new MapNodeFactory();

            var cities = Enum.GetValues(typeof(City));
            foreach (var city in cities)
            {
                _state.Cities.Add(nodeFactory.CreateMapNode((City)city));
            }

            _state.ResearchStationStock = 6;

            _state.DiseaseCubeReserve = new Dictionary<Disease, int>
            {
                {Disease.Blue, 24},
                {Disease.Black, 24},
                {Disease.Red, 24 },
                {Disease.Yellow, 24}
            };

            _state.DiscoveredCures = new Dictionary<Disease, DiseaseState>
            {
                {Disease.Blue, DiseaseState.NotCured},
                {Disease.Black, DiseaseState.NotCured},
                {Disease.Red, DiseaseState.NotCured },
                {Disease.Yellow, DiseaseState.NotCured}
            };

            _messageSender.SendMessageASync("AutoCardboard/Pandemic/StateEditor", "Cleared");
        }

        public void TakeTurn(IPandemicState state, IPandemicTurn turn)
        {
            _state = state;
            _currentPlayerId = turn.CurrentPlayerId;
            foreach (var action in turn.ActionsTaken)
            {
                TakePlayerAction(_state, action);
            }
            _state.TurnsPlayed++;
        }
        public void TakePlayerAction(IPandemicState state, PlayerAction action)
        {
            _state = state;
            _currentPlayerId = action.PlayerId;

            switch(action.PlayerActionType)
            {
                case PlayerActionType.TreatDisease:
                    TreatDisease(_state, action.City, action.Disease);
                    break;
                case PlayerActionType.DriveOrFerry:
                    DriveOrFerry(_state, action.City);
                    break;
                case PlayerActionType.BuildResearchStation:
                    BuildResearchStation(_state, action.City);
                    break;
                case PlayerActionType.CharterFlight:
                    CharterFlight(_state, action.City);
                    break;
                case PlayerActionType.DirectFlight:
                    DirectFlight(_state, action.City);
                    break;
                case PlayerActionType.ShuttleFlight:
                    ShuttleFlight(_state, action.City);
                    break;
                case PlayerActionType.DiscoverCure:
                    DiscoverCure(_state, action.Disease);
                    break;
                case PlayerActionType.ShareKnowledge:
                    ShareKnowledge();
                    break;
            }

        }

        // TODO
        private void ShareKnowledge()
        {
                        
        }

        private void DiscoverCure(IPandemicState state, Disease disease)
        {
            _state = state;
            _state.DiscoveredCures[disease] = !_state.Cities.Any(n => n.DiseaseCubes[disease] > 0) ? DiseaseState.Cured : DiseaseState.Eradicated;
        }

        private void ShuttleFlight(IPandemicState state, City city)
        {
            _state = state;
            var node = _state.Cities.Single(c => c.City == city);
            var playerState = _state.PlayerStates[_currentPlayerId];
            var startingCity = playerState.Location;
            playerState.Location = city;

            var cardToDiscard = playerState.PlayerHand.Single(c => c.PlayerCardType == PlayerCardType.City && (City)c.Value == startingCity);
            playerState.PlayerHand.Remove(cardToDiscard);
            _state.PlayerDiscardPile.AddCard(cardToDiscard);
        }

        private void DirectFlight(IPandemicState state, City city)
        {
            _state = state;
            var node = _state.Cities.Single(c => c.City == city);
            var playerState = _state.PlayerStates[_currentPlayerId];
            playerState.Location = city;

            var cardToDiscard = playerState.PlayerHand.Single(c => c.PlayerCardType == PlayerCardType.City && (City)c.Value == city);
            playerState.PlayerHand.Remove(cardToDiscard);
            _state.PlayerDiscardPile.AddCard(cardToDiscard);
        }

        private void CharterFlight(IPandemicState state, City city)
        {
            _state = state;
            var node = _state.Cities.Single(c => c.City == city);
            var playerState = _state.PlayerStates[_currentPlayerId];
            var startingCity = playerState.Location;
            playerState.Location = city;

            var cardToDiscard = playerState.PlayerHand.Single(c => c.PlayerCardType == PlayerCardType.City && (City)c.Value == startingCity);
            playerState.PlayerHand.Remove(cardToDiscard);
            _state.PlayerDiscardPile.AddCard(cardToDiscard);
        }

        private void BuildResearchStation(IPandemicState state, City city)
        {
            _state = state;
            var playerState = _state.PlayerStates[_currentPlayerId];

            var node = _state.Cities.Single(c => c.City == city);
            node.HasResearchStation = true;
            _state.ResearchStationStock--;

            if (playerState.PlayerRole != PlayerRole.OperationsExpert)
            {
                var cardToDiscard = playerState.PlayerHand.Single(c => c.PlayerCardType == PlayerCardType.City && (City)c.Value == city);
                playerState.PlayerHand.Remove(cardToDiscard);
                _state.PlayerDiscardPile.AddCard(cardToDiscard);
            }
        }

        private void DriveOrFerry(IPandemicState state, City city)
        {
            _state = state;
            var playerState = _state.PlayerStates[_currentPlayerId];
            playerState.Location = city;
        }

        private void TreatDisease(IPandemicState state, City city, Disease disease)
        {
            _state = state;
            var diseaseState = _state.DiscoveredCures[disease];
            var currentPlayerRole = _state.PlayerStates[_currentPlayerId].PlayerRole;

            var node = _state.Cities.Single(c => c.City == city);

            if (disease == 0)
            {
                return;
            }

            if (diseaseState == DiseaseState.Cured || currentPlayerRole == PlayerRole.Medic)
            {
                _state.DiseaseCubeReserve[disease] += node.DiseaseCubes[disease];
                node.DiseaseCubes[disease] = 0;
            }
            else
            {
                node.DiseaseCubes[disease]--;
                _state.DiseaseCubeReserve[disease]++;
            }
          
        }

        public void Epidemic(IPandemicState state)
        {
            _state = state;
            var bottomCard = _state.InfectionDeck.DrawBottom();
            _log.Information($"Epidemic in {(City)bottomCard.Value}");
            _state.InfectionDiscardPile.AddCard(bottomCard);
            AddDiseaseCubes(_state, (City)bottomCard.Value, 3);

            // Intensify
            _state.InfectionDiscardPile.Shuffle();
            _state.InfectionDeck.AddCardDeck(_state.InfectionDiscardPile, CardDeckPosition.Top);

            _state.InfectionRateMarker++;

            _messageSender.SendMessageASync("AutoCardboard/Pandemic/StateEditor", $"Epidemic handled at {(City)bottomCard.Value}");
        }

        public void InfectCities(IPandemicState state)
        {
            _state = state;
            var infectionRate = _state.InfectionRateTrack[_state.InfectionRateMarker];
            _messageSender.SendMessageASync("AutoCardboard/Pandemic/StateEditor", $"Infecting Cities. Rate = {infectionRate}");

            if (_state.InfectionDeck.CardCount < infectionRate)
            {
                _state.IsGameOver = true;
                return;
            }

            var infectionCards = _state.InfectionDeck.Draw(infectionRate).ToList();

            foreach (var infectionCard in infectionCards)
            {
                if (!_state.IsGameOver)
                {
                    var city = (City)infectionCard.Value;
                    var disease = city.GetDefaultDisease();
                    AddDiseaseCube(_state, disease, city);
                    _messageSender.SendMessageASync("AutoCardboard/Pandemic/StateEditor", $"Infected {city} with {disease}");
                }
            }

            if (!state.IsGameOver)
            {
                _state.InfectionDiscardPile.AddCards(infectionCards);
                _messageSender.SendMessageASync("AutoCardboard/Pandemic/StateEditor", $"Finished infecting Cities.");
            }
        }

        public void AddDiseaseCubes(IPandemicState state, City city, int count = 1)
        {
            _state = state;
            var disease = city.GetDefaultDisease();

            for (var i = 0; i < count; i++)
            {
                AddDiseaseCube(_state, disease, city);
            }
        }

        public void AddDiseaseCube(IPandemicState state, Disease disease, City city, List<City> ignoreCities = null)
        {
            _state = state;
            var node = _state.Cities.Single(n => n.City == city);


            // Dont place disease if quarantineSpecialist is here or in neighbouring city
            var quarantineSpecialist = GetPlayerStateByRole(_state, PlayerRole.QuarantineSpecialist);
            if (quarantineSpecialist != null)
            {
                if (quarantineSpecialist.Location == city || _state.Cities.Single(n => n.City == quarantineSpecialist.Location).ConnectedCities.Contains(city))
                {
                    _log.Information($"quarantineSpecialist in {quarantineSpecialist.Location} prevented {disease} in {city}");
                    return;
                }
            }

            _log.Information($"Adding {disease} to {city}");

            ignoreCities = ignoreCities ?? new List<City>();

            if (node.DiseaseCubes[disease] < 3)
            {
                if (_state.DiseaseCubeReserve[disease] == 0)
                {
                    _log.Information($"Game over. No cubes left for {disease}");
                    _state.IsGameOver = true;
                    return;
                }

                node.DiseaseCubes[disease] += 1;
                _state.DiseaseCubeReserve[disease]--;

                return;
            }

            _log.Information($"Outbreak in {node.City}");
            _state.OutbreakCount++;

            if (_state.OutbreakCount > 7)
            {
                _log.Information($"Game over. Too many outbreaks");
                _state.IsGameOver = true;
                return;
            }

            ignoreCities.Add(city);
            foreach (var connectedCity in node.ConnectedCities.Where(c => !ignoreCities.Contains(c)))
            {
                AddDiseaseCube(_state, disease, connectedCity, ignoreCities);
            }

            _log.Information($"Finished adding {disease} to {city}");
        }

        private void SetupPlayerStates(IPandemicState state, IEnumerable<IPlayer<IPandemicTurn>> players)
        {
            _state = state;
            var roleDeck = new RoleDeck();

            _state.PlayerStates = new Dictionary<int, PandemicPlayerState>();
            foreach (var player in players)
            {
                _state.PlayerStates[player.Id] = new PandemicPlayerState
                {
                    PlayerHand = new List<PandemicPlayerCard>(),
                    Location = City.Atlanta,
                    PlayerRole = (PlayerRole)roleDeck.DrawTop().Value
                };
            }

            SetupPlayerDeck(_state);
        }

        public void SetupPlayerDeck(IPandemicState state)
        {
            _state = state;
            _state.PlayerDeck = new PlayerDeck();

            // Add city cards
            foreach (var city in Enum.GetValues(typeof(City)))
            {
                _state.PlayerDeck.AddCard(new PandemicPlayerCard{ Value = (int)city, Name = city.ToString(), PlayerCardType = PlayerCardType.City});
            }

            // Add event cards
            foreach (var eventCard in Enum.GetValues(typeof(EventCard)))
            {
                _state.PlayerDeck.AddCard(new PandemicPlayerCard{ Value = (int)eventCard, Name = eventCard.ToString(), PlayerCardType = PlayerCardType.Event});
            }

            _state.PlayerDeck.Shuffle();

            if (_state.PlayerStates != null)
            {
                foreach (var player in  _state.PlayerStates)
                {
                    var newPlayerCards = _state.PlayerDeck.Draw(4);

                    foreach (var card in newPlayerCards)
                    {
                        player.Value.PlayerHand.Add(card);
                    }
                }
            }
           

            if (_state.PandemicCardCount != 0)
            {
                // Divide empties out State.PlayerDeck of cards 
                var playerDeckCardPiles = _state.PlayerDeck.Divide(_state.PandemicCardCount).ToList();

                foreach (var cardDeck in playerDeckCardPiles)
                {
                    cardDeck.AddCard( new PandemicPlayerCard{ PlayerCardType = PlayerCardType.Epidemic, Name = "Epidemic", Value = 0});
                    cardDeck.Shuffle();
                }

                // Divide removed all cards from State.PlayerDeck when it moved them into piles
                // add the shuffled piles back, starting from rightmost pile
                foreach (var cardDeck in playerDeckCardPiles)
                {
                    _state.PlayerDeck.Add(cardDeck);
                }
            }
        }

        private PandemicPlayerState GetPlayerStateByRole(IPandemicState state, PlayerRole playerRole)
        {
            _state = state;
            foreach (var playerState in _state.PlayerStates)
            {
                if (playerState.Value.PlayerRole == playerRole)
                {
                    return playerState.Value;
                }
            }
            return null;
        }

        private void PerformInitialInfections(IPandemicState state)
        {
            _state = state;
            _log.Information("Starting initial infections");

            var infectionCards = _state.InfectionDeck.Draw(3).ToList();
            _state.InfectionDiscardPile.AddCards(infectionCards);
            foreach (var infectionCard in infectionCards)
            {
                AddDiseaseCubes(_state, (City)infectionCard.Value, 3);
            }

            infectionCards = _state.InfectionDeck.Draw(3).ToList();
            _state.InfectionDiscardPile.AddCards(infectionCards);
            foreach (var infectionCard in infectionCards)
            {
                AddDiseaseCubes(_state, (City)infectionCard.Value, 2);
            }

            infectionCards = _state.InfectionDeck.Draw(3).ToList();
            _state.InfectionDiscardPile.AddCards(infectionCards);
            foreach (var infectionCard in infectionCards)
            {
                AddDiseaseCubes(_state, (City)infectionCard.Value, 1);
            }

            _log.Information("Finished initial infections");
        }

        public IEnumerable<PandemicPlayerCard> DrawPlayerCards(IPandemicState state)
        {
            _state = state;
            if (_state.PlayerDeck.CardCount < 2)
            {
                _log.Information("Game over. Empty playerdeck");
                _state.IsGameOver = true;
                return new List<PandemicPlayerCard>();
            }

            return _state.PlayerDeck.Draw(2);
        }

    }
}
