using System.Collections.Generic;
using autoCardboard.Common;
using autoCardBoard.Pandemic.Bots;
using autoCardboard.Pandemic.TurnState;

namespace autoCardboard.Pandemic.Game
{
    public class PandemicPlayerFactory : IPlayerFactory<IPandemicTurn>
    {
        private readonly IRouteHelper _routeHelper;
        private readonly IResearchStationHelper _researchStationHelper;
        private readonly IHandManagementHelper _playerDeckHelper;
        private readonly IEventCardHelper _eventCardHelper;
        private readonly IKnowledgeShareHelper _knowledgeShareHelper;

        public PandemicPlayerFactory(IRouteHelper routeHelper, 
            IResearchStationHelper researchStationHelper, IHandManagementHelper playerDeckHelper,
            IEventCardHelper eventCardHelper, IKnowledgeShareHelper knowledgeShareHelper)
        {
            _routeHelper = routeHelper;
            _researchStationHelper = researchStationHelper;
            _playerDeckHelper = playerDeckHelper;
            _eventCardHelper = eventCardHelper;
            _knowledgeShareHelper = knowledgeShareHelper;
        }

        public IEnumerable<IPlayer<IPandemicTurn>> CreatePlayers(PlayerConfiguration playerConfiguration)
        {
            List<IPlayer<IPandemicTurn>> players = new List<IPlayer<IPandemicTurn>>();
            for (int player = 1; player <= playerConfiguration.PlayerCount; player++)
            {
                var newPlayer = new PandemicBotStandard(_routeHelper, _playerDeckHelper, _researchStationHelper, _eventCardHelper, _knowledgeShareHelper)
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
