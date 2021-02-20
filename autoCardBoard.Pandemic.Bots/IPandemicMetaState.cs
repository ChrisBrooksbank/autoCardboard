﻿using System.Collections.Generic;
using autoCardboard.Pandemic.State;
using autoCardboard.Pandemic.TurnState;

namespace autoCardBoard.Pandemic.Bots
{
    public interface IPandemicMetaState
    {
        IPandemicTurn PandemicTurn{ get; set; }
        int PlayerId{ get; set; }
        PandemicPlayerState PlayerState{ get; set; }
        List<Disease> CurableDiseases{ get; set; }
        City NextTurnStartsFromLocation{ get; set; }
        bool AtResearchStation { get; set; }
        bool ShouldBuildResearchStation{ get; }
        City? NearestCityWithResearchStation{ get; }
        List<City> RouteToNearestResearchStation{ get;}

        void Load(IPandemicTurn turn);
        void UpdateLocation(City city);
        void UpdatePlayerHand(List<PandemicPlayerCard> hand);
    }
}