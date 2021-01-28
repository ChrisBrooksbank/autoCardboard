using System;
using autoCardboard.Common;

namespace autoCardboard.Pandemic.State
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
