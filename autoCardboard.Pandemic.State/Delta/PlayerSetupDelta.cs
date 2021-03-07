using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace autoCardboard.Pandemic.State.Delta
{
    public class PlayerSetupDelta: Delta, IPlayerSetupDelta
    {
        public int PlayerId { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public PlayerRole PlayerRole { get; set; }
        [JsonConverter(typeof(StringEnumConverter))] 
        public City City { get; set; }
        [JsonConverter(typeof(StringEnumConverter))] 
        public override DeltaType DeltaType => DeltaType.PlayerSetup;
    }
}
