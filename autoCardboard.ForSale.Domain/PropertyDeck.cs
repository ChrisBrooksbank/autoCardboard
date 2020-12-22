using autoCardboard.Common.Domain.Interfaces;
using autoCardboard.ForSale.Domain.Interfaces;
using System.Collections.Generic;

namespace autoCardboard.ForSale.Domain
{
    class PropertyDeck : IPropertyDeck
    {
        private List<PropertyCard> cards;

        public PropertyDeck()
        {

        }

        public IEnumerable<ICard> Draw(int count)
        {
            throw new System.NotImplementedException();
        }

        public void Shuffle()
        {
            throw new System.NotImplementedException();
        }
    }
}
