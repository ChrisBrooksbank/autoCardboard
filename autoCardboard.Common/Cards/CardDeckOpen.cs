using System;
using System.Collections.Generic;

namespace autoCardboard.Common
{
    [Serializable]
    public class CardDeckOpen<TCardType>: CardDeck<TCardType>, ICardDeck<TCardType> where TCardType: ICard
    {
        public List<TCardType> Cards => cards;
    }
}
