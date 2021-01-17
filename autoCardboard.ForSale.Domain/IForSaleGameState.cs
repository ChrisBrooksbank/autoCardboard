﻿using System.Collections.Generic;
using autoCardboard.Common.Domain.Cards;
using autoCardboard.Common.Domain.Interfaces;

namespace autoCardboard.ForSale.Domain
{
    public interface IForSaleGameState: IGameState
    {
        CardDeck<Card> PropertyDeck { get; set; }
        CardDeck<Card> ChequeDeck { get; set; }
        IEnumerable<ICard> PropertyCardsOnTable { get; set; }
        IEnumerable<ICard> ChequesOnTable { get; set; }

        Dictionary<int,ForSalePlayerState> PlayerStates { get; set; }
    }
}
