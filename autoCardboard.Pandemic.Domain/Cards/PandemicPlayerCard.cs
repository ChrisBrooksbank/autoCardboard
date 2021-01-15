using System;
using autoCardboard.Common.Domain.Cards;

namespace autoCardboard.Pandemic.Domain
{
    [Serializable]
    public class PandemicPlayerCard: Card
    {
        public PlayerCardType PlayerCardType { get; set; }
    }
}
