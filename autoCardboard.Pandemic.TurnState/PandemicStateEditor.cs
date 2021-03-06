﻿using System;
using System;
using System.Collections.Generic;
using System.Linq;
using autoCardboard.Common;
using autoCardboard.Infrastructure.Exceptions;
using autoCardboard.Pandemic.State;
using autoCardboard.Pandemic.State.Delta;

namespace autoCardboard.Pandemic.TurnState
{
    public class PandemicStateEditor: IPandemicStateEditor
    {
        private List<IDelta> _stateDeltas;

        private int _currentPlayerId;
        private IPandemicState _state;
        private readonly IPandemicActionValidator _validator;

        public PandemicStateEditor(IPandemicActionValidator validator)
        {
            _validator = validator;
        }

        public IEnumerable<IDelta> Setup(IPandemicState state, IEnumerable<IPlayer<IPandemicTurn>> players, int pandemicCardCount = 6)
        {
            _state = state;
            _stateDeltas = new List<IDelta>();
            Clear(_state);
            _state.PandemicCardCount = pandemicCardCount;
            SetupPlayerStates(_state, players);
            PerformInitialInfections(_state);
            return _stateDeltas;
        }

        public void Clear(IPandemicState state, int pandemicCardCount = 6)
        {
            _state = state;
            _state.Id = "1"; // TODO Guid.NewGuid().ToString();
            _stateDeltas = new List<IDelta>();
            _state.ActionsPlayed = 0;
            _state.OutbreakCount = 0;
            var players = new List<IPlayer<IPandemicTurn>>();
            SetupPlayerStates(_state, players);
            _state.IsGameOver = false;
            _state.Cities = new List<MapNode>();

            _state.InfectionDeck = new InfectionDeck();
            _state.InfectionDiscardPile = new CardDeck<Card>();
            _state.PlayerDeck = new PlayerDeck();
            _state.EventCardsQueue = new PlayerDeck();
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

            // Atlanta starts with a research station
            _state.Cities.Single(c => c.City == City.Atlanta).HasResearchStation = true;
            _state.ResearchStationStock--;
        }

        public IEnumerable<IDelta> ApplyTurn(IPandemicState state, IPandemicTurn turn)
        {
            _stateDeltas = new List<IDelta>();

            switch (turn.TurnType)
            {
                case PandemicTurnType.TakeActions:
                    TakeActionsTurn(state, turn);
                    break;
                case PandemicTurnType.DiscardCards:
                    TakeDiscardCardsTurn(state,turn);
                    break;
                case PandemicTurnType.PlayEventCards:
                    TakePlayEventCardsTurn(state,turn);
                    break;
            }

            return _stateDeltas;
        }

        public void TakePlayEventCardsTurn(IPandemicState state, IPandemicTurn turn)
        {
            if (turn.EventCardsPlayed == null || !turn.EventCardsPlayed.Any())
            {
                return;
            }

            _state = state;
            _currentPlayerId = turn.CurrentPlayerId;
            var playerState = _state.PlayerStates[_currentPlayerId];

            foreach (var eventCardToPlay in turn.EventCardsPlayed)
            {
                if (eventCardToPlay.EventCard == EventCard.OneQuietNight)
                {
                    var card = playerState.PlayerHand.Single(c => c.PlayerCardType == PlayerCardType.Event && (EventCard)c.Value == EventCard.OneQuietNight );
                    _state.EventCardsQueue.AddCard(card);
                    playerState.PlayerHand.Remove(card);
                    state.PlayerDiscardPile.AddCard(card);
                    _stateDeltas.Add( new CardIsDrawnOrDiscardedDelta {PlayerId = _currentPlayerId, PandemicPlayerCard = card, DrawnOrDiscarded = DrawnOrDiscarded.Discarded});
                }

                if (eventCardToPlay.EventCard == EventCard.GovernmentGrant)
                {
                    var card = playerState.PlayerHand.Single(c => c.PlayerCardType == PlayerCardType.Event && (EventCard)c.Value == EventCard.GovernmentGrant );
                    BuildResearchStationWithGovernmentGrant(_state, eventCardToPlay.City.Value);
                    playerState.PlayerHand.Remove(card);
                    state.PlayerDiscardPile.AddCard(card);
                    _stateDeltas.Add( new CardIsDrawnOrDiscardedDelta {PlayerId = _currentPlayerId, PandemicPlayerCard = card, DrawnOrDiscarded = DrawnOrDiscarded.Discarded});
                }

                if (eventCardToPlay.EventCard == EventCard.Airlift)
                {
                    var card = playerState.PlayerHand.Single(c => c.PlayerCardType == PlayerCardType.Event && (EventCard)c.Value == EventCard.Airlift );
                    _state.PlayerStates[eventCardToPlay.PlayerId].Location = eventCardToPlay.City.Value;
                    playerState.PlayerHand.Remove(card);
                    state.PlayerDiscardPile.AddCard(card);
                    _stateDeltas.Add( new CardIsDrawnOrDiscardedDelta {PlayerId = _currentPlayerId, PandemicPlayerCard = card, DrawnOrDiscarded = DrawnOrDiscarded.Discarded});
                }
            }
        }


        public void TakeDiscardCardsTurn(IPandemicState state, IPandemicTurn turn)
        {
            if (!turn.CardsToDiscard.Any())
            {
                return;
            }

            _state = state;
            _currentPlayerId = turn.CurrentPlayerId;
            var playerState = _state.PlayerStates[_currentPlayerId];
            
            foreach (var cardToDiscard in turn.CardsToDiscard)
            {
                var cardInPlayerDeck = playerState.PlayerHand
                    .SingleOrDefault(c => c.PlayerCardType == cardToDiscard.PlayerCardType && c.Value == cardToDiscard.Value);
                if (cardInPlayerDeck != null)
                {
                    playerState.PlayerHand.Remove(cardInPlayerDeck);
                    state.PlayerDiscardPile.AddCard(cardInPlayerDeck);
                    _stateDeltas.Add( new CardIsDrawnOrDiscardedDelta {PlayerId = _currentPlayerId, PandemicPlayerCard = cardInPlayerDeck, DrawnOrDiscarded = DrawnOrDiscarded.Discarded});
                }
            }

        }

        public void TakeActionsTurn(IPandemicState state, IPandemicTurn turn)
        {
            _state = state;
            _currentPlayerId = turn.CurrentPlayerId;
            ApplyPlayerAction(_state, turn.ActionTaken);
            _state.ActionsPlayed++;
        }
        
        private void ApplyPlayerAction(IPandemicState state, PlayerAction action)
        {
            _state = state;
            _currentPlayerId = action.PlayerId;

            var validationFailures = _validator.ValidatePlayerAction(_currentPlayerId, state, action).ToList();
            if (validationFailures.Any())
            {
                throw new CardboardException(validationFailures[0]);
            }

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
                    DiscoverCure(_state, action.Disease, action.CardsToDiscard);
                    break;
                case PlayerActionType.ShareKnowledge:
                    ShareKnowledge(_state, action.City, action.PlayerId, action.OtherPlayerId);
                    break;
            }

        }

        private void ShareKnowledge(IPandemicState state, City city, int playerId, int otherPlayerId)
        { 
            _state = state;
            var player = _state.PlayerStates.SingleOrDefault(p => p.Key == playerId);
            var otherPlayer = _state.PlayerStates.SingleOrDefault(p => p.Key == otherPlayerId);

            var cityCardInPlayersHand = player.Value.PlayerHand
                .SingleOrDefault( c=> c.PlayerCardType == PlayerCardType.City && (City)c.Value == city);
            var cityCardInOtherPlayersHand = otherPlayer.Value.PlayerHand
                .SingleOrDefault( c=> c.PlayerCardType == PlayerCardType.City && (City)c.Value == city);

            if (cityCardInPlayersHand != null)
            {
                otherPlayer.Value.PlayerHand.Add(cityCardInPlayersHand);
                player.Value.PlayerHand.Remove(cityCardInPlayersHand);
                _stateDeltas.Add( new CardIsDrawnOrDiscardedDelta {PlayerId = playerId, PandemicPlayerCard = cityCardInPlayersHand, DrawnOrDiscarded = DrawnOrDiscarded.Discarded});
                _stateDeltas.Add( new CardIsDrawnOrDiscardedDelta {PlayerId = otherPlayerId, PandemicPlayerCard = cityCardInPlayersHand, DrawnOrDiscarded = DrawnOrDiscarded.Drawn});
            }
            else
            {
                if (cityCardInOtherPlayersHand != null)
                {
                    player.Value.PlayerHand.Add(cityCardInOtherPlayersHand);
                    otherPlayer.Value.PlayerHand.Remove(cityCardInOtherPlayersHand);
                    _stateDeltas.Add( new CardIsDrawnOrDiscardedDelta {PlayerId = otherPlayerId, PandemicPlayerCard = cityCardInPlayersHand, DrawnOrDiscarded = DrawnOrDiscarded.Discarded});
                    _stateDeltas.Add( new CardIsDrawnOrDiscardedDelta {PlayerId = playerId, PandemicPlayerCard = cityCardInPlayersHand, DrawnOrDiscarded = DrawnOrDiscarded.Drawn});
                }
            }
        }

        private void DiscoverCure(IPandemicState state, Disease disease, IEnumerable<PandemicPlayerCard> cardsToDiscard)
        {
            _state = state;
            _state.DiscoveredCures[disease] = _state.Cities.Any(n => n.DiseaseCubes[disease] > 0) ? DiseaseState.Cured : DiseaseState.Eradicated;
            foreach (var cardToDiscard in cardsToDiscard)
            {
                _state.PlayerStates[_currentPlayerId].PlayerHand.Remove(cardToDiscard);
                _state.PlayerDiscardPile.AddCard(cardToDiscard);
                _stateDeltas.Add( new CardIsDrawnOrDiscardedDelta {PlayerId = _currentPlayerId, PandemicPlayerCard = cardToDiscard, DrawnOrDiscarded = DrawnOrDiscarded.Discarded});
            }

            _stateDeltas.Add(new DiseaseStateChanged()
            {
              Disease = disease,
              DiseaseState = _state.DiscoveredCures[disease]
            });
        }

        private void ShuttleFlight(IPandemicState state, City city)
        {
            _state = state;
            var playerState = _state.PlayerStates[_currentPlayerId];
            playerState.Location = city;

            _stateDeltas.Add(new PlayerMovedDelta()
            {
                PlayerId = _currentPlayerId,
                City = city
            });
        }

        private void DirectFlight(IPandemicState state, City city)
        {
            _state = state;
            var playerState = _state.PlayerStates[_currentPlayerId];
            playerState.Location = city;

            var cardToDiscard = playerState.PlayerHand.Single(c => c.PlayerCardType == PlayerCardType.City && (City)c.Value == city);
            playerState.PlayerHand.Remove(cardToDiscard);
            _state.PlayerDiscardPile.AddCard(cardToDiscard);
            _stateDeltas.Add( new CardIsDrawnOrDiscardedDelta {PlayerId = _currentPlayerId, PandemicPlayerCard = cardToDiscard, DrawnOrDiscarded = DrawnOrDiscarded.Discarded});

            _stateDeltas.Add(new PlayerMovedDelta()
            {
                PlayerId = _currentPlayerId,
                City = city
            });
        }

        private void CharterFlight(IPandemicState state, City city)
        {
            _state = state;
            var playerState = _state.PlayerStates[_currentPlayerId];
            var startingCity = playerState.Location;
            playerState.Location = city;

            var cardToDiscard = playerState.PlayerHand.Single(c => c.PlayerCardType == PlayerCardType.City && (City)c.Value == startingCity);
            playerState.PlayerHand.Remove(cardToDiscard);
            _state.PlayerDiscardPile.AddCard(cardToDiscard);
            _stateDeltas.Add( new CardIsDrawnOrDiscardedDelta {PlayerId = _currentPlayerId, PandemicPlayerCard = cardToDiscard, DrawnOrDiscarded = DrawnOrDiscarded.Discarded});

            _stateDeltas.Add(new PlayerMovedDelta()
            {
                PlayerId = _currentPlayerId,
                City = city
            });
        }

        private void BuildResearchStationWithGovernmentGrant(IPandemicState state, City city)
        {
            BuildResearchStation(state, city, true);
        }

        private void BuildResearchStation(IPandemicState state, City city, bool haveGrant = false)
        {
            _state = state;
            var playerState = _state.PlayerStates[_currentPlayerId];

            var node = _state.Cities.Single(c => c.City == city);
            node.HasResearchStation = true;
            _state.ResearchStationStock--;

            if (!haveGrant && playerState.PlayerRole != PlayerRole.OperationsExpert)
            {
                var cardToDiscard = playerState.PlayerHand.Single(c => c.PlayerCardType == PlayerCardType.City && (City)c.Value == city);
                playerState.PlayerHand.Remove(cardToDiscard);
                _state.PlayerDiscardPile.AddCard(cardToDiscard);
                _stateDeltas.Add( new CardIsDrawnOrDiscardedDelta {PlayerId = _currentPlayerId, PandemicPlayerCard = cardToDiscard, DrawnOrDiscarded = DrawnOrDiscarded.Discarded});
            }

            _stateDeltas.Add(new ResearchStationDelta()
            {
                City = city
            });
        }
        
        private void DriveOrFerry(IPandemicState state, City city)
        {
            _state = state;
            var playerState = _state.PlayerStates[_currentPlayerId];
            playerState.Location = city;
            _stateDeltas.Add(new PlayerMovedDelta()
            {
               PlayerId = _currentPlayerId,
               City = city
            });
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

            _stateDeltas.Add(new DiseaseChangedDelta()
            {
                City = node.City,
                Disease = disease,
                NewAmount = node.DiseaseCubes[disease]
            });
        }

        public IEnumerable<IDelta> Epidemic(IPandemicState state)
        {
            _state = state;
            _stateDeltas = new List<IDelta>();
            var bottomCard = _state.InfectionDeck.DrawBottom();
            _state.InfectionDiscardPile.AddCard(bottomCard);
            AddDiseaseCubes(_state, (City)bottomCard.Value, 3);

            // Intensify
            _state.InfectionDiscardPile.Shuffle();
            _state.InfectionDeck.AddCardDeck(_state.InfectionDiscardPile, CardDeckPosition.Top);

            _state.InfectionRateMarker++;
            return _stateDeltas;
        }

        // TODO state deltas
        public IEnumerable<IDelta> InfectCities(IPandemicState state)
        {
            _state = state;
            _stateDeltas = new List<IDelta>();

            var oneQuietNightCard = _state.EventCardsQueue.Cards
                .SingleOrDefault(c => c.PlayerCardType == PlayerCardType.Event && (EventCard)c.Value == EventCard.OneQuietNight);
            if (oneQuietNightCard != null)
            {
                state.PlayerDiscardPile.AddCard(oneQuietNightCard);
                state.EventCardsQueue.Cards.Remove(oneQuietNightCard);
                _stateDeltas.Add( new CardIsDrawnOrDiscardedDelta {PlayerId = _currentPlayerId, PandemicPlayerCard = oneQuietNightCard, DrawnOrDiscarded = DrawnOrDiscarded.Discarded});
                return _stateDeltas;
            }

            var infectionRate = _state.InfectionRateTrack[_state.InfectionRateMarker];

            if (_state.InfectionDeck.CardCount < infectionRate)
            {
                _state.IsGameOver = true;
                _state.GameOverReason = "Infection deck empty";
                return _stateDeltas;
            }

            var infectionCards = _state.InfectionDeck.Draw(infectionRate).ToList();

            foreach (var infectionCard in infectionCards)
            {
                if (!_state.IsGameOver)
                {
                    var city = (City)infectionCard.Value;
                    var disease = city.GetDefaultDisease();
                    AddDiseaseCube(_state, disease, city);
                }
            }

            if (!state.IsGameOver)
            {
                _state.InfectionDiscardPile.AddCards(infectionCards);
                foreach (var infectionCard in infectionCards)
                {
                    _stateDeltas.Add( new CardIsDrawnOrDiscardedDelta
                    {
                        InfectionCard = (City)infectionCard.Value,
                        DrawnOrDiscarded = DrawnOrDiscarded.Discarded
                    });
                }
            }

            return _stateDeltas;
        }
        private void AddDiseaseCubes(IPandemicState state, City city, int count = 1)
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
                    return;
                }
            }

            ignoreCities = ignoreCities ?? new List<City>();

            if (node.DiseaseCubes[disease] < 3)
            {
                if (_state.DiseaseCubeReserve[disease] == 0)
                {
                    _state.IsGameOver = true;
                    _state.GameOverReason = $"Ran out of {disease} disease cubes";
                    return;
                }

                node.DiseaseCubes[disease] += 1;
                _state.DiseaseCubeReserve[disease]--;
                _stateDeltas.Add( new DiseaseChangedDelta()
                {
                    City = city, Disease = disease, NewAmount = node.DiseaseCubes[disease]
                } );

                return;
            }

            _state.OutbreakCount++;

            if (_state.OutbreakCount > 7)
            {
                _state.IsGameOver = true;
                _state.GameOverReason = "More than seven outbreaks";
                return;
            }

            ignoreCities.Add(city);
            foreach (var connectedCity in node.ConnectedCities.Where(c => !ignoreCities.Contains(c)))
            {
                AddDiseaseCube(_state, disease, connectedCity, ignoreCities);
            }
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
                _stateDeltas.Add( new PlayerSetupDelta
                {
                    PlayerId = player.Id, 
                    City = _state.PlayerStates[player.Id].Location, 
                    PlayerRole = _state.PlayerStates[player.Id].PlayerRole
                } );
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
                var card = new PandemicPlayerCard{Value = (int) city, Name = city.ToString(), PlayerCardType = PlayerCardType.City};
                _state.PlayerDeck.AddCard(card);
            }

            // Add event cards
            foreach (var eventCard in Enum.GetValues(typeof(EventCard)))
            {
                var card = new PandemicPlayerCard{Value = (int) eventCard, Name = eventCard.ToString(), PlayerCardType = PlayerCardType.Event};
                _state.PlayerDeck.AddCard(card);
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
                        _stateDeltas.Add( new CardIsDrawnOrDiscardedDelta {PlayerId = _currentPlayerId, PandemicPlayerCard = card, DrawnOrDiscarded = DrawnOrDiscarded.Drawn});
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

            var infectionCards = _state.InfectionDeck.Draw(3).ToList();
            _state.InfectionDiscardPile.AddCards(infectionCards);
           
            foreach (var infectionCard in infectionCards)
            {
                AddDiseaseCubes(_state, (City)infectionCard.Value, 3);
                _stateDeltas.Add( new CardIsDrawnOrDiscardedDelta {InfectionCard = (City)infectionCard.Value, DrawnOrDiscarded = DrawnOrDiscarded.Discarded});
            }

            infectionCards = _state.InfectionDeck.Draw(3).ToList();
            _state.InfectionDiscardPile.AddCards(infectionCards);
            foreach (var infectionCard in infectionCards)
            {
                AddDiseaseCubes(_state, (City)infectionCard.Value, 2);
                _stateDeltas.Add( new CardIsDrawnOrDiscardedDelta {InfectionCard = (City)infectionCard.Value, DrawnOrDiscarded = DrawnOrDiscarded.Discarded});
            }

            infectionCards = _state.InfectionDeck.Draw(3).ToList();
            _state.InfectionDiscardPile.AddCards(infectionCards);
            foreach (var infectionCard in infectionCards)
            {
                AddDiseaseCubes(_state, (City)infectionCard.Value, 1);
                _stateDeltas.Add( new CardIsDrawnOrDiscardedDelta {InfectionCard = (City)infectionCard.Value, DrawnOrDiscarded = DrawnOrDiscarded.Discarded});
            }

        }

    }
}
