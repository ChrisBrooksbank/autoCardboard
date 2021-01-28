using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace autoCardboard.Pandemic.State
{
    [Serializable]
    public class PandemicPlayerState
    {
        [JsonConverter(typeof(StringEnumConverter))] 
        public PlayerRole PlayerRole { get; set; }
        [JsonConverter(typeof(StringEnumConverter))] 
        public City Location { get; set; }
        public List<PandemicPlayerCard> PlayerHand { get; set; }
    }
}
