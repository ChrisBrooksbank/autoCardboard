using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace autoCardboard.Pandemic.State.Delta
{
    [Serializable]
    public class PlayerMovedDelta: Delta, IPlayerMovedDelta
    {
        public int PlayerId { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public City City { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public override DeltaType DeltaType => DeltaType.PlayerLocation;
    }
}
