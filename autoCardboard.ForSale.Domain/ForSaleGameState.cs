using autoCardboard.Common.Domain;
using autoCardboard.Common.Domain.Interfaces;
using System;
using System.Collections.Generic;
using autoCardboard.Common.Domain.Cards;

namespace autoCardboard.ForSale.Domain
{
    /// <summary>
    /// Responsible for storing all state of a ForSale game
    /// </summary>
    /// 
    [Serializable]
    public class ForSaleGameState: GameState
    {
        public CardDeck<Card> PropertyDeck { get; set; }
        public CardDeck<Card> ChequeDeck { get; set; }
        public IEnumerable<ICard> PropertyCardsOnTable { get; set; }
        public IEnumerable<ICard> ChequesOnTable { get; set; }

        public Dictionary<int,ForSalePlayerState> PlayerStates { get; set; }

        public ForSaleGameState()
        {
            PropertyDeck = new CardDeck<Card>();
            PropertyCardsOnTable = new List<Card>();
            PlayerStates = new Dictionary<int, ForSalePlayerState>();
        }
    }
}
