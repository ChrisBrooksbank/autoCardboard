using System;
using autoCardboard.Common;

namespace autoCardboard.Pandemic
{
    [Serializable]
    public class PandemicPlayerCard: Card
    {
        public PlayerCardType PlayerCardType { get; set; }
    }
}
