using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace autoCardboard.Pandemic.State.Delta
{
    [Serializable]
    public class DiseaseChangedDelta: Delta, IDiseaseChangedDelta
    {
        [JsonConverter(typeof(StringEnumConverter))] 
        public City City { get; set; }
        [JsonConverter(typeof(StringEnumConverter))] 
        public Disease Disease { get; set; }
        public int NewAmount { get; set; }
        [JsonConverter(typeof(StringEnumConverter))] 
        public override DeltaType DeltaType => DeltaType.Disease;
    }
}
