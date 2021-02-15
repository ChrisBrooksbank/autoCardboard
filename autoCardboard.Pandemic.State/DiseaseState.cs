using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace autoCardboard.Pandemic.State
{
    [JsonConverter(typeof(StringEnumConverter))] 
    public enum DiseaseState
    {
        NotCured,
        Cured,
        Eradicated
    }
}
