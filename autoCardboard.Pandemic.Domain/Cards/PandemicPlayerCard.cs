using System;
using autoCardboard.Common;

namespace autoCardboard.Pandemic.Domain
{
    [Serializable]
    public class PandemicPlayerCard: Card
    {
        public PlayerCardType PlayerCardType { get; set; }
    }
}
