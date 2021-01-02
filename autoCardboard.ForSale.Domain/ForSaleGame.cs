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

            Console.WriteLine();
            foreach(var player in players)
            {
                Console.Write($"player {player.Id} got properties : ");
                foreach(var card in State.PlayerStates[player.Id].PropertyCards)
                {
                    Console.Write($"{card.Id} ");
                }
                Console.WriteLine();
            }
            Console.ReadLine();

            // TODO play property flipping rounds
        }

        private void PlayPropertyBidRound()
        {
            State.PropertyCardsOnTable = State.PropertyDeck.Draw(Players.Count());
            while (State.PropertyCardsOnTable.Count() > 0)
            {
                Console.Write("Bidding on these properties : ");
                foreach (var card in State.PropertyCardsOnTable)
                {
                    Console.Write($"{card.Id} ");
                }
                Console.WriteLine();

                var passedPlayers = new List<int>();
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

                Console.WriteLine("Bidding round ends");
                Console.WriteLine();
                State.PropertyCardsOnTable = State.PropertyDeck.Draw(Players.Count());
            }
          
        }

        private void PlayerPaysForLastPropertyCard(ForSaleGameTurn turn)
        {
            var playerState = State.PlayerStates[turn.CurrentPlayerId];
            var cardsOnTable = State.PropertyCardsOnTable.ToList();
            var cardToTake = cardsOnTable[0];

            if (cardsOnTable.Remove(cardToTake))
            {
                playerState.PropertyCards.Add(cardToTake);
                playerState.CoinsBid = 0;
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
                playerState.CoinBalance += refund;
                playerState.CoinsBid = 0;
                // Remove lowest property card and give to this player
                playerState.PropertyCards.Add(cardToTake);
                State.PropertyCardsOnTable = cardsOnTable;
                Console.WriteLine($"Player {playerId} passed. They got property {cardToTake.Id} and got {refund} coins back.");
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
            Console.WriteLine($"Player {playerId} bids {bidAmount}");
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
