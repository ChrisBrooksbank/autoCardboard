using System.Collections.Generic;
using autoCardboard.Common;
using autoCardboard.Infrastructure;
using autoCardboard.Messaging;
using autoCardBoard.Pandemic.Bots;
using autoCardboard.Pandemic.TurnState;

namespace autoCardboard.Pandemic.Game
{
    public class PandemicPlayerFactory : IPlayerFactory<IPandemicTurn>
    {
        private readonly ICardboardLogger _log;
        private readonly IRouteHelper _routeHelper;
        private readonly IResearchStationHelper _researchStationHelper;
        private readonly IHandManagementHelper _playerDeckHelper;
        private readonly IEventCardHelper _eventCardHelper;
        private readonly IMessageSender _messageSender;

        public PandemicPlayerFactory(ICardboardLogger log, IRouteHelper routeHelper, 
            IResearchStationHelper researchStationHelper, IHandManagementHelper playerDeckHelper,
            IEventCardHelper eventCardHelper, IMessageSender messageSender)
        {
            _log = log;
            _routeHelper = routeHelper;
            _researchStationHelper = researchStationHelper;
            _playerDeckHelper = playerDeckHelper;
            _eventCardHelper = eventCardHelper;
            _messageSender = messageSender;
        }

        public IEnumerable<IPlayer<IPandemicTurn>> CreatePlayers(PlayerConfiguration playerConfiguration)
        {
            List<IPlayer<IPandemicTurn>> players = new List<IPlayer<IPandemicTurn>>();
            for (int player = 1; player <= playerConfiguration.PlayerCount; player++)
            {
                var newPlayer = new PandemicBotStandard(_log, _routeHelper, _messageSender, _playerDeckHelper, _researchStationHelper, _eventCardHelper)
                {
                    Id = player,
                    Name = player.ToString()
                };
                players.Add(newPlayer); ;
            }

            return players;
        }
    }
}
