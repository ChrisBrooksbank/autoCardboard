using autoCardboard.Common.Domain.Interfaces;
using System.Collections.Generic;
using autoCardboard.Common.Domain;

namespace autoCardboard.ForSale.Domain
{
    public class PropertyDeck : CardDeck
    {
      public PropertyDeck()
        {
            cards = new List<ICard>();

            for (var cardNumber = 1; cardNumber <= 30; cardNumber++)
            {
                cards.Add( new Card{ Id = cardNumber, Name = cardNumber.ToString()});
            }

            Shuffle();
        }
    }
}
