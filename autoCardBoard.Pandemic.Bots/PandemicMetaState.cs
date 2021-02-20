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

        private bool _shouldBuildResearchStationDirty, _routeToNearestResearchStationDirty, _nearestCityWithResearchStationDirty;
        private bool _shouldBuildResearchStation;
        private List<City> _routeToNearestResearchStation;
        private City? _nearestCityWithResearchStation;

        public IPandemicTurn PandemicTurn{ get; set; }
        public int PlayerId{get; set;}
        public PandemicPlayerState PlayerState{ get; set; }
        public List<Disease> CurableDiseases{ get; set; }
        public City NextTurnStartsFromLocation{ get; set; }
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
            UpdateLocation(PlayerState.Location);
            UpdatePlayerHand(PlayerState.PlayerHand);
        }

        public void UpdateLocation(City city)
        {
            NextTurnStartsFromLocation = city;
            AtResearchStation = PandemicTurn.State.Cities.Single(c => c.City.Equals(NextTurnStartsFromLocation)).HasResearchStation;
            _shouldBuildResearchStationDirty = true;
            _routeToNearestResearchStationDirty = true;
            _nearestCityWithResearchStationDirty = true;
        }

        public void UpdatePlayerHand(List<PandemicPlayerCard> hand)
        {
            CurableDiseases = _playerDeckHelper.GetDiseasesCanCure(PlayerState.PlayerRole, hand).ToList();
            _shouldBuildResearchStationDirty = true;
        }

        private bool GetShouldBuildResearchStation()
        {
            if (_shouldBuildResearchStationDirty)
            {
                _shouldBuildResearchStation = _researchStationHelper.ShouldBuildResearchStation(PandemicTurn.State, NextTurnStartsFromLocation, PlayerState.PlayerRole, PlayerState.PlayerHand);
                _shouldBuildResearchStationDirty = false;
            }
            return _shouldBuildResearchStation;
        }

        private List<City> GetRouteToNearestResearchStation()
        {
            if (_routeToNearestResearchStationDirty)
            {
                _routeToNearestResearchStation = NearestCityWithResearchStation == null ? new List<City>() : _routeHelper.GetShortestPath(PandemicTurn.State, NextTurnStartsFromLocation, NearestCityWithResearchStation.Value);
                _routeToNearestResearchStationDirty = false;
            }

            return _routeToNearestResearchStation;
        }

        private City? GetNearestCityWithResearchStation()
        {
            if (_nearestCityWithResearchStationDirty)
            {
                _nearestCityWithResearchStation = _routeHelper.GetNearestCitywithResearchStation(PandemicTurn.State, NextTurnStartsFromLocation);
                _nearestCityWithResearchStationDirty = false;
            }

            return _nearestCityWithResearchStation;
        }
    }
}
