using System;
using System.Collections.Generic;
using System.Linq;
using autoCardboard.Common;
using autoCardboard.DependencyInjection;
using autoCardboard.Infrastructure;
using autoCardboard.Messaging;
using autoCardboard.Pandemic.State;
using autoCardboard.Pandemic.TurnState;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;

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

        public void Setup()
        {
            _serviceProvider = ServiceProviderFactory.GetServiceProvider(new MessageSenderConfiguration());
            _stateEditor = _serviceProvider.GetService<IPandemicStateEditor>();
            _playerFactory = _serviceProvider.GetService<IPlayerFactory<IPandemicTurn>>();

            _sut = _playerFactory.CreatePlayers( new PlayerConfiguration { PlayerCount = 1 }).SingleOrDefault();

            _state =_serviceProvider.GetService<IPandemicState>();
            _stateEditor.Clear(_state, 6);

            var cardboardLogger = A.Fake<ICardboardLogger>();
            var turnValidator = A.Fake<IPandemicActionValidator>();
            _turn = new PandemicTurn(turnValidator);
        }

        public void StandardBotTakesShuttleFlights()
        {
            _state.PlayerStates = new Dictionary<int, PandemicPlayerState>();
            _state.PlayerStates[1] = new PandemicPlayerState
            {
                PlayerHand = new List<PandemicPlayerCard>(),
                Location = City.Atlanta,
                PlayerRole = PlayerRole.Researcher
            };

            var atlanta = _state.Cities.Single(c => c.City == City.Atlanta);
            atlanta.HasResearchStation = true;
            _state.ResearchStationStock = 6;

            var mumbai = _state.Cities.Single(c => c.City == City.Mumbai);
            mumbai.HasResearchStation = true;
            _state.ResearchStationStock--;

            var bangkok =  _state.Cities.Single(c => c.City == City.Bangkok);
            bangkok.DiseaseCubes[Disease.Blue] = 3;
            _state.DiseaseCubeReserve[Disease.Blue] -= 3;
            
            _turn.State = _state;
            _turn.CurrentPlayerId = 1;
            _sut.GetTurn(_turn);

            // Assert.AreEqual(_turn.ActionTaken.PlayerActionType == PlayerActionType.ShuttleFlight, true);
        }
    }
}