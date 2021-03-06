﻿using autoCardboard.Common;
using System.Linq;

namespace autoCardboard.ForSale
{
    public class ForSalePlayer : IPlayer<IForSaleGameTurn>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        private Die _d100 = new Die(100);

        /// <summary>
        /// Choose a turn based on turn.State and make it
        /// any changes made to turn.State are ignored
        /// </summary>
        /// <param name="turn"></param>
        public void GetTurn(IForSaleGameTurn turn)
        {
            if (turn.State.PropertyCardsOnTable.Any())
            {
                GetPropertyBiddingTurn(turn);
            }
            else
            {
                GetPropertyFlippingTurn(turn);
            }
           
        }

        private void GetPropertyBiddingTurn(IForSaleGameTurn turn)
        {
            var playerState = turn.State.PlayerStates[Id];
            var currentHighestBid = turn.State.PlayerStates.Max(p => p.Value.CoinsBid);
            var minimumNextBid = currentHighestBid + 1;

            if (minimumNextBid <= playerState.CoinBalance && FuzzyDecideIfIWantToPass(turn))
            {
                turn.Bid(minimumNextBid);
            }
            else
            {
                turn.Pass();
            }
        }

        private void GetPropertyFlippingTurn(IForSaleGameTurn turn)
        {
            var playerState = turn.State.PlayerStates[turn.CurrentPlayerId];
            var die = new Die(playerState.PropertyCards.Count);
            turn.PropertyToFlip = playerState.PropertyCards[die.Roll() -1];
        }

        private bool FuzzyDecideIfIWantToPass(IForSaleGameTurn turn)
        {
            return _d100.Roll() > GetPercentageRoundDone(turn);
        }

        private float GetPercentageRoundDone(IForSaleGameTurn turn)
        {
            var totalTurns = 30 / turn.State.PlayerStates.Count();
            var turnsRemaining = turn.State.PropertyDeck.CardCount / turn.State.PlayerStates.Count();
            var turnsTaken = totalTurns - turnsRemaining;
            var progress = ((float)turnsTaken / (float)totalTurns) * 100;
            return progress;
        }
    }
}
