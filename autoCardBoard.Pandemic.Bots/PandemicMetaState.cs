using autoCardboard.Pandemic.State;
using System.Collections.Generic;
using System.Linq;
using autoCardboard.Pandemic.TurnState;

namespace autoCardBoard.Pandemic.Bots
{
    public class PandemicMetaState : IPandemicMetaState
    {
        private readonly IRouteHelper _routeHelper;
        private readonly IPlayerDeckHelper _playerDeckHelper;
        private readonly IResearchStationHelper _researchStationHelper;

        private bool? _shouldBuildResearchStation;
        private List<City> _routeToNearestResearchStation;
        private City? _nearestCityWithResearchStation;
        private List<Disease> _curableDiseases;

        public IPandemicTurn PandemicTurn{ get; set; }
        public int PlayerId{get; set;}
        public PandemicPlayerState PlayerState{ get; set; }
        public List<Disease> CurableDiseases => GetCurableDiseases();
        public City City{ get; set; }
        public MapNode MapNode{ get; set; }
        public bool AtResearchStation {get; set;}
        public bool ShouldBuildResearchStation => GetShouldBuildResearchStation();
        public City? NearestCityWithResearchStation => GetNearestCityWithResearchStation();
        public List<City> RouteToNearestResearchStation => GetRouteToNearestResearchStation();

        public PandemicMetaState(IRouteHelper routeHelper, IPlayerDeckHelper playerDeckHelper, IResearchStationHelper researchStationHelper)
        {
            _routeHelper = routeHelper;
            _playerDeckHelper = playerDeckHelper;
            _researchStationHelper = researchStationHelper;
        }

        public void Load(IPandemicTurn turn)
        {
            PandemicTurn = turn;
            PlayerId = turn.CurrentPlayerId;
            PlayerState = turn.State.PlayerStates[PlayerId];
            City = PlayerState.Location;
            MapNode = turn.State.Cities.Single(n => n.City == City);
        }
        private bool GetShouldBuildResearchStation()
        {
            if (_shouldBuildResearchStation == null)
            {
                _shouldBuildResearchStation = _researchStationHelper.ShouldBuildResearchStation(PandemicTurn.State, City, PlayerState.PlayerRole, PlayerState.PlayerHand);
            }
            return _shouldBuildResearchStation.Value;
        }

        private List<City> GetRouteToNearestResearchStation()
        {
            if (_routeToNearestResearchStation == null)
            {
                _routeToNearestResearchStation = NearestCityWithResearchStation == null ? new List<City>() : _routeHelper.GetShortestPath(PandemicTurn.State, City, NearestCityWithResearchStation.Value);
            }

            return _routeToNearestResearchStation;
        }

        private City? GetNearestCityWithResearchStation()
        {
            if (_nearestCityWithResearchStation == null)
            {
                _nearestCityWithResearchStation = _routeHelper.GetNearestCitywithResearchStation(PandemicTurn.State, City);
            }

            return _nearestCityWithResearchStation;
        }

        private List<Disease> GetCurableDiseases()
        {
            if (_curableDiseases == null)
            {
                _curableDiseases = _playerDeckHelper.GetDiseasesCanCure(PlayerState.PlayerRole, PlayerState.PlayerHand).ToList();
            }

            return _curableDiseases;
        }
    }
}
