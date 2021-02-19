using System;
using System.Collections.Generic;
using System.Linq;
using autoCardboard.Common;
using autoCardboard.DependencyInjection;
using autoCardboard.Infrastructure;
using autoCardboard.Pandemic.State;
using autoCardboard.Pandemic.TurnState;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace autoCardBoard.Pandemic.Bots.Test
{
    public class Tests
    {
        private IServiceProvider _serviceProvider;
        private IPandemicStateEditor _stateEditor;
        private IPandemicState _state;
        private IPlayerFactory<IPandemicTurn> _playerFactory;
        private IPlayer<IPandemicTurn> _sut;
        private IPandemicTurn _turn;

        [SetUp]
        public void Setup()
        {
            _serviceProvider = ServiceProviderFactory.GetServiceProvider();
            _stateEditor = _serviceProvider.GetService<IPandemicStateEditor>();
            _playerFactory = _serviceProvider.GetService<IPlayerFactory<IPandemicTurn>>();

            _sut = _playerFactory.CreatePlayers( new PlayerConfiguration { PlayerCount = 1 }).SingleOrDefault();

            _state =_serviceProvider.GetService<IPandemicState>();
            _stateEditor.Clear(_state, 6);

            var cardboardLogger = A.Fake<ICardboardLogger>();
            var turnValidator = A.Fake<IPandemicTurnValidator>();
            _turn = new PandemicTurn(cardboardLogger, turnValidator);
        }

        [Test]
        public void StandardBotTakesShuttleFlights()
        {
            _state.PlayerStates = new Dictionary<int, PandemicPlayerState>();
            _state.PlayerStates[1] = new PandemicPlayerState
            {
                PlayerHand = new List<PandemicPlayerCard>(),
                Location = City.Atlanta,
                PlayerRole = PlayerRole.Researcher
            };

            var mumbai = _state.Cities.Single(c => c.City == City.Mumbai);
            mumbai.HasResearchStation = true;
            _state.ResearchStationStock = 5;

            var bangkok =  _state.Cities.Single(c => c.City == City.Mumbai);
            bangkok.DiseaseCubes[Disease.Blue] = 3;
            _state.DiseaseCubeReserve[Disease.Blue] -= 3;
            
            _turn.State = _state;
            _turn.CurrentPlayerId = 1;
            _sut.GetTurn(_turn);

            Assert.AreEqual(_turn.ActionsTaken.Any(a => a.PlayerActionType == PlayerActionType.ShuttleFlight), true);
        }
    }
}