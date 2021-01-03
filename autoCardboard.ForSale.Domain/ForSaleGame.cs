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

            while (State.PropertyDeck.CardCount >= players.Count())
            {
                PlayPropertyBidRound();
            }

            OutputPropertyBiddingSummary(players);
        }

        private void OutputPropertyBiddingSummary(IEnumerable<IPlayer<ForSaleGameTurn>> players)
        {
            Console.WriteLine();
            foreach (var player in players)
            {
                Console.Write($"player {player.Id} has coin balance {State.PlayerStates[player.Id].CoinBalance} and properties : ");
                foreach (var card in State.PlayerStates[player.Id].PropertyCards)
                {
                    Console.Write($"{card.Id} ");
                }
                Console.WriteLine();
            }
        }

        private void PlayPropertyBidRound()
        {
            State.PropertyCardsOnTable = State.PropertyDeck.Draw(Players.Count()).OrderBy(c => c.Id).ToList();
            OutputPropertyBidRoundStartingState();
            var passedPlayers = new List<int>();
            while (State.PropertyCardsOnTable.Count() > 0)
            {
                foreach (var player in Players.Where(p => !passedPlayers.Contains(p.Id)))
                {
                    var turn = new ForSaleGameTurn { CurrentPlayerId = player.Id, State = State };

                    if (State.PropertyCardsOnTable.Count() == 1)
                    {
                        PlayerPaysForLastPropertyCard(turn);
                    }
                    else
                    {
                        player.GetTurn(turn);
                        ProcessTurn(turn);
                        if (turn.Passed)
                        {
                            passedPlayers.Add(player.Id);
                        }
                    }
                }
            }

        }

        private void OutputPropertyBidRoundStartingState()
        {
            Console.WriteLine("***");
            Console.Write("Opening bidding on properties : ");
            foreach (var card in State.PropertyCardsOnTable)
            {
                Console.Write($"{card.Id} ");
            }
            Console.WriteLine();
        }

        private void PlayerPaysForLastPropertyCard(ForSaleGameTurn turn)
        {
            var playerState = State.PlayerStates[turn.CurrentPlayerId];
            var cardsOnTable = State.PropertyCardsOnTable.ToList();
            var cardToTake = cardsOnTable[0];

            if (cardsOnTable.Remove(cardToTake))
            {
                Console.WriteLine($"Player {turn.CurrentPlayerId} takes last property {cardToTake.Id} paying {playerState.CoinsBid}");
                playerState.PropertyCards.Add(cardToTake);
                playerState.CoinsBid = 0;
                State.PropertyCardsOnTable = cardsOnTable;
            }
        }

        private void ProcessTurn(ForSaleGameTurn turn)
        {
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
                var refund = (playerState.CoinsBid / 2);
                if (playerState.CoinsBid > 0)
                {
                    Console.WriteLine($"Player {playerId} passes and gets a refund of {refund} on a bid of {playerState.CoinsBid} getting property {cardToTake.Id}");
                }
                else
                {
                    Console.WriteLine($"Player {playerId} passes on first turn getting property {cardToTake.Id}");
                }
               
                playerState.CoinBalance += refund;
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

            var oldBid = playerState.CoinsBid;
            playerState.CoinsBid = bidAmount;
            playerState.CoinBalance = playerState.CoinBalance - (bidAmount - oldBid);
            Console.WriteLine($"Player {playerId} bids {bidAmount} reducing their balance to {playerState.CoinBalance}");
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
