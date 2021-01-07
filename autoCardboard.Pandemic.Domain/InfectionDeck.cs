using System;
using autoCardboard.Common.Domain.Cards;

namespace autoCardboard.Pandemic.Domain
{
    [Serializable]
    public class InfectionDeck: CardDeck<Card>
    {
        public InfectionDeck()
        {
            foreach (var city in Enum.GetValues(typeof(City)))
            {
                AddCard(new Card{ Value = (int)city, Name = city.ToString()});
            }
            Shuffle();
        }
    }
}
