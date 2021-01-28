using System;
using autoCardboard.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace autoCardboard.Pandemic.State
{
    [Serializable]
    public class PandemicPlayerCard: Card
    {
        [JsonConverter(typeof(StringEnumConverter))] 
        public PlayerCardType PlayerCardType { get; set; }
    }
}
