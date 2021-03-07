using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace autoCardboard.Pandemic.State.Delta
{
    public class DiseaseStateChanged: Delta, IDiseaseStateChanged
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public Disease Disease { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public DiseaseState DiseaseState { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public override DeltaType DeltaType => DeltaType.DiseaseStateChanged;
    }
}
