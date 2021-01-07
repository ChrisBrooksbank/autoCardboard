using System;
using autoCardboard.Common.Domain.Interfaces;

namespace autoCardboard.Common.Domain.Cards
{
    [Serializable]
    public class Card: ICard
    {
        public int Value { get; set; }
        public string Name { get; set; }
    }
}
