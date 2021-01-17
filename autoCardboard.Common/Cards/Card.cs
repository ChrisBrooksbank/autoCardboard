using System;

namespace autoCardboard.Common
{
    [Serializable]
    public class Card: ICard
    {
        public int Value { get; set; }
        public string Name { get; set; }
    }
}
