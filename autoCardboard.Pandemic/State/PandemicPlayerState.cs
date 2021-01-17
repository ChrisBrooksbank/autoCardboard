using System;
using System.Collections.Generic;

namespace autoCardboard.Pandemic
{
    [Serializable]
    public class PandemicPlayerState
    {
        public PlayerRole PlayerRole { get; set; }
        public City Location { get; set; }
        public List<PandemicPlayerCard> PlayerHand { get; set; }
    }
}
