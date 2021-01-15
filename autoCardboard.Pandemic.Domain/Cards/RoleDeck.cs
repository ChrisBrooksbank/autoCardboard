using System;
using autoCardboard.Common.Domain.Cards;

namespace autoCardboard.Pandemic.Domain
{
    public class RoleDeck: CardDeck<Card>
    {
        public RoleDeck()
        {
            foreach (var role in Enum.GetValues(typeof(PlayerRole)))
            {
                AddCard(new Card{ Value = (int)role, Name = role.ToString()});
            }
            Shuffle();
        }
    }
}
