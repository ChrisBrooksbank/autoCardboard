using autoCardboard.Common.Domain.Interfaces;
using System;

namespace autoCardboard.Common.Domain
{
    [Serializable]
    public class Card: ICard
    {
        public int Value { get; set; }
        public string Name { get; set; }
    }
}
