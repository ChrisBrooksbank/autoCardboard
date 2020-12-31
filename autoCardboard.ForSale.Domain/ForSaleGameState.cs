using autoCardboard.Common.Domain;
using autoCardboard.Common.Domain.Interfaces;
using System.Collections.Generic;

namespace autoCardboard.ForSale.Domain
{
    public class ForSaleGameState: IGameState
    {
        public CardDeck PropertyDeck { get; set; }
        public IEnumerable<ICard> PropertyCardsOnTable { get; set; }
        public int CurrentBid { get; set; }

        public Dictionary<int,ForSalePlayerState> PlayerStates { get; set; }

        public ForSaleGameState()
        {
            PropertyDeck = new CardDeck();
            PropertyCardsOnTable = new List<Card>();
            PlayerStates = new Dictionary<int, ForSalePlayerState>();
        }
    }
}
