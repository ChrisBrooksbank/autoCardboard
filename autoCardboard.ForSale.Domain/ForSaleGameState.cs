using autoCardboard.Common.Domain;
using autoCardboard.Common.Domain.Interfaces;
using System;
using System.Collections.Generic;

namespace autoCardboard.ForSale.Domain
{
    /// <summary>
    /// Responsible for storing all state of a ForSale game
    /// </summary>
    /// 
    [Serializable]
    public class ForSaleGameState: GameState
    {
        public CardDeck PropertyDeck { get; set; }
        public IEnumerable<ICard> PropertyCardsOnTable { get; set; }

        public Dictionary<int,ForSalePlayerState> PlayerStates { get; set; }

        public ForSaleGameState()
        {
            PropertyDeck = new CardDeck();
            PropertyCardsOnTable = new List<Card>();
            PlayerStates = new Dictionary<int, ForSalePlayerState>();
        }
    }
}
