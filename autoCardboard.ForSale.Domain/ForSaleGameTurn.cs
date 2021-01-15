using autoCardboard.Common.Domain.Interfaces;
using System;

namespace autoCardboard.ForSale.Domain
{
    [Serializable]
    public class ForSaleGameTurn : IForSaleGameTurn
    {
        // _state is a clone of the game state ( so any changes to it by player are ignored )
        private ForSaleGameState _state;

        public int CurrentPlayerId { get; set; }

        public bool Passed;
        public int BidAmount;
        public ICard PropertyToFlip { get; set; }

        public ForSaleGameState State
        {
            get
            {
                return _state;
            }

            set
            {
                _state = value.Clone() as ForSaleGameState;
            }
        }
       
        public void Pass()
        {
            BidAmount = 0;
            Passed = true;
        }

        public void Bid()
        {
            Passed = false;
            BidAmount = GetMaximumBid();
        }

        public void Bid(int amount)
        {
            if (amount >= GetMinimumBid() && amount <= GetMaximumBid())
            {
                Passed = false;
                BidAmount = amount;
            }
        }

        private int GetMinimumBid()
        {
            var maxBidOnTable = 0;
            foreach(var player in _state.PlayerStates)
            {
                if (player.Value.CoinsBid > maxBidOnTable)
                {
                    maxBidOnTable = player.Value.CoinsBid;
                }
            }

            return maxBidOnTable + 1;
        }

        private int GetMaximumBid()
        {
            var currentPlayerState = _state.PlayerStates[CurrentPlayerId];
            return currentPlayerState.CoinBalance + currentPlayerState.CoinsBid;
        }
    }
}
