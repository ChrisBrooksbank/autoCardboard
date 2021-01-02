using autoCardboard.Common.Domain;
using autoCardboard.Common.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace autoCardboard.ForSale.Domain
{
    public class ForSaleGame: Game<ForSaleGameState, ForSaleGameTurn>
    {
        public ForSaleGame()
        {
            State = new ForSaleGameState();
        }
             

        public override void Play(IEnumerable<IPlayer<ForSaleGameTurn>> players)
        {
            Setup(players);

            // Play property bidding rounds
            while (State.PropertyDeck.CardCount >= players.Count())
            {
                PlayPropertyBidRound();
            }

            // TODO play property flipping rounds

        }

        private void PlayPropertyBidRound()
        {
            State.PropertyCardsOnTable = State.PropertyDeck.Draw(Players.Count());

            var passedPlayers = new List<int>();
            foreach (var player in Players.Where(p=> !passedPlayers.Contains(p.Id)))
            {
                var turn = new ForSaleGameTurn { CurrentPlayerId = player.Id, State = State };
                player.GetTurn(turn);
                ProcessTurn(turn);
                if (turn.Passed)
                {
                    passedPlayers.Add(player.Id);
                }
            }
        }

        private void ProcessTurn(ForSaleGameTurn turn)
        {
         
            // TODO if last player, they cant pass
            if (turn.Passed)
            {
                ProcessPassingTurn(turn.CurrentPlayerId);
            }
            else
            {
                ProcessBiddingTurn(turn.CurrentPlayerId, turn.BidAmount);
            }
        }

        private void ProcessPassingTurn(int playerId)
        {
            var playerState = State.PlayerStates[playerId];

            var cardsOnTable = State.PropertyCardsOnTable.ToList();
            var lowestPropertyIdOnTable = cardsOnTable.Min(c => c.Id);
            var cardToTake = State.PropertyCardsOnTable.SingleOrDefault(c => c.Id.Equals(lowestPropertyIdOnTable));
         
            if ( cardsOnTable.Remove(cardToTake) )
            {
                // Refund half of players bid
                playerState.CoinBalance += (playerState.CoinsBid / 2);
                playerState.CoinsBid = 0;
                // Remove lowest property card and give to this player
                playerState.PropertyCards.Add(cardToTake);
                State.PropertyCardsOnTable = cardsOnTable;
            }
        }

        private void ProcessBiddingTurn(int playerId, int bidAmount)
        {
            var playerState = State.PlayerStates[playerId];

            if (bidAmount > playerState.CoinsBid + playerState.CoinBalance)
            {
                throw new ApplicationException("Player cannot afford bid made");
            }
            
            playerState.CoinsBid = bidAmount;
            playerState.CoinBalance -= bidAmount;
        }

        private void Setup(IEnumerable<IPlayer<ForSaleGameTurn>> players)
        {
            State.PropertyDeck = new CardDeck();
            for (var cardNumber = 1; cardNumber <= 30; cardNumber++)
            {
                State.PropertyDeck.AddCard(new Card { Id = cardNumber, Name = cardNumber.ToString() });
            }
            State.PropertyDeck.Shuffle();

            State.PropertyCardsOnTable = new List<ICard>();
            Players = players;

            SetupPlayerStates();
        }

        private void SetupPlayerStates()
        {
            State.PlayerStates = new Dictionary<int, ForSalePlayerState>();
            foreach(var player in Players)
            {
                State.PlayerStates[player.Id] = new ForSalePlayerState
                {
                    PropertyCards = new List<ICard>(),
                    CoinBalance = 16,
                    CoinsBid = 0
                };
            }
        }
    }
}
