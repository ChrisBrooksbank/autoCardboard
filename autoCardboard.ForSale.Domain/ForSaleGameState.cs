using autoCardboard.Common.Domain.Interfaces;
using autoCardboard.ForSale.Domain.Interfaces;
using System.Collections.Generic;

namespace autoCardboard.ForSale.Domain
{
    public class ForSaleGameState: IGameState
    {
        public IEnumerable<ICard> PropertyDrawDeck { get; set; }
        public IEnumerable<ICard> PropertyDiscardDeck { get; set; }
        public IEnumerable<ICard> PropertyBidDeck { get; set; }

        public IEnumerable<IPlayer> Players { get; set; }

        // TODO move to a base class ????
        public T GetState<T>()
        {
            throw new System.NotImplementedException();
        }

        public T GetState<T>(IPlayer playerBot)
        {
            throw new System.NotImplementedException();
        }
    }
}
