using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace autoCardboard.Pandemic.State.Delta
{
    [Serializable]
    public class CardIsDrawnOrDiscardedDelta: Delta, ICardIsDrawnOrDiscardedDelta
    {
        public int? PlayerId { get; set; }
        public PandemicPlayerCard PandemicPlayerCard{ get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public City? InfectionCard { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public DrawnOrDiscarded DrawnOrDiscarded{ get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public override DeltaType DeltaType => DeltaType.CardDrawnOrDiscarded;

    }
}
