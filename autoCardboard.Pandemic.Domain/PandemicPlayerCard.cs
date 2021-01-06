using System;
using autoCardboard.Common.Domain;

namespace autoCardboard.Pandemic.Domain
{
    [Serializable]
    public class PandemicPlayerCard: Card
    {
        public PlayerCardType PlayerCardType { get; set; }
    }
}
