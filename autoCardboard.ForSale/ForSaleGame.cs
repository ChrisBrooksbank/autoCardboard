using System;
using System.Collections.Generic;
using System.Linq;
using autoCardboard.Common;
using autoCardboard.Infrastructure;

namespace autoCardboard.ForSale
{
    public class ForSaleGame : IGame<IForSaleGameState, IForSaleGameTurn>
    {
        private readonly ICardboardLogger _logger;
        private readonly IForSaleGameState _state;

        public IEnumerable<IPlayer<IForSaleGameTurn>> Players { get; set; }

        public ForSaleGame(ICardboardLogger logger, IForSaleGameState gameState)
        {
            _logger = logger;
            _state = gameState;
        }

        public IGameState Play()
        {
            Setup(Players);

            // Property bidding rounds
            while (_state.PropertyDeck.CardCount >= Players.Count())
            {
                PlayPropertyBidRound();
            }

            // Property flipping rounds
            while (_state.ChequeDeck.CardCount >= Players.Count())
            {
                PlayPropertyFlippingRound();
            }

            return _state;
        }
        
        private void PlayPropertyBidRound()
        {
            _state.PropertyCardsOnTable = _state.PropertyDeck.Draw(Players.Count()).OrderBy(c => c.Value).ToList();

            _logger.Information($"Property cards drawn for bidding round");

            var passedPlayers = new List<int>();

            while (_state.PropertyCardsOnTable.Any())
            {
                foreach (var player in Players.Where(p => !passedPlayers.Contains(p.Id)))
                {
                    var turn = new ForSaleGameTurn { CurrentPlayerId = player.Id, State = _state };

                    if (_state.PropertyCardsOnTable.Count() == 1)
                    {
                        PlayerPaysForLastPropertyCard(turn);
                        // TODO this player gos first in next round
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

        private void PlayPropertyFlippingRound()
        {
            _state.ChequesOnTable = _state.ChequeDeck.Draw(Players.Count()).OrderBy(c => c.Value).ToList();

            foreach(var player in Players)
            {
                var turn = new ForSaleGameTurn { CurrentPlayerId = player.Id, State = _state };
                player.GetTurn(turn);
                ProcessTurn(turn);
            }

            // give cheques out to players
            var chequesInOrder = _state.ChequesOnTable.OrderByDescending(c => c.Value).ToList();
            var flippingPropertiesInOrder = _state.PlayerStates
                .Select( p => p.Value.PropertySelling.Value)
                .OrderByDescending(p => p)
                .ToList();

            foreach (var propertyToFlip in flippingPropertiesInOrder)
            {
                var flippingPlayer = _state.PlayerStates
                    .SingleOrDefault(p => p.Value.PropertySelling.Value == propertyToFlip);
                var chequeToAllocate = chequesInOrder[0];
                if (chequesInOrder.Remove(chequeToAllocate))
                {
                    flippingPlayer.Value.ChequeCards.Add(chequeToAllocate);
                }
            }
        }
        
        private void PlayerPaysForLastPropertyCard(ForSaleGameTurn turn)
        {
            var playerState = _state.PlayerStates[turn.CurrentPlayerId];
            var cardsOnTable = _state.PropertyCardsOnTable.ToList();
            var cardToTake = cardsOnTable[0];

            if (cardsOnTable.Remove(cardToTake))
            {
                playerState.PropertyCards.Add(cardToTake);
                playerState.CoinsBid = 0;
                _state.PropertyCardsOnTable = cardsOnTable;
            }
        }

        private void ProcessTurn(ForSaleGameTurn turn)
        {
            if (_state.PropertyCardsOnTable.Any())
            {
                ProcessPropertyBidTurn(turn);
            }
            else
            {
                ProcessPropertyFlippingTurn(turn);
            }
        }

        private void ProcessPropertyFlippingTurn(ForSaleGameTurn turn)
        {
            var playerState = _state.PlayerStates[turn.CurrentPlayerId];
            var propertyBeingFlipped = playerState.PropertyCards.SingleOrDefault(p => p.Value == turn.PropertyToFlip.Value);
            if (propertyBeingFlipped != null && playerState.PropertyCards.Remove(propertyBeingFlipped))
            {
                playerState.PropertySelling = propertyBeingFlipped;
            }
        }

        private void ProcessPropertyBidTurn(ForSaleGameTurn turn)
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
            var playerState = _state.PlayerStates[playerId];

            var cardsOnTable = _state.PropertyCardsOnTable.ToList();
            var lowestPropertyIdOnTable = cardsOnTable.Min(c => c.Value);
            var cardToTake = _state.PropertyCardsOnTable.SingleOrDefault(c => c.Value.Equals(lowestPropertyIdOnTable));
         
            if ( cardsOnTable.Remove(cardToTake) )
            {
                // Refund half of players bid
                var refund = (playerState.CoinsBid / 2);

                playerState.CoinBalance += refund;
                playerState.CoinsBid = 0;
                // Remove lowest property card and give to this player
                playerState.PropertyCards.Add(cardToTake);
                _state.PropertyCardsOnTable = cardsOnTable;
            }
        }

        private void ProcessBiddingTurn(int playerId, int bidAmount)
        {
            var playerState = _state.PlayerStates[playerId];

            if (bidAmount > playerState.CoinsBid + playerState.CoinBalance)
            {
                throw new ApplicationException("Player cannot afford bid made");
            }

            var oldBid = playerState.CoinsBid;
            playerState.CoinsBid = bidAmount;
            playerState.CoinBalance = playerState.CoinBalance - (bidAmount - oldBid);
        }

        public void Setup(IEnumerable<IPlayer<IForSaleGameTurn>> players)
        {
            _state.PropertyDeck = new CardDeck<Card>();
            for (var cardNumber = 1; cardNumber <= 30; cardNumber++)
            {
                _state.PropertyDeck.AddCard(new Card { Value = cardNumber, Name = cardNumber.ToString() });
            }
            _state.PropertyDeck.Shuffle();

            _state.ChequeDeck = new CardDeck<Card>();
            _state.ChequeDeck.AddCard(new Card { Value = 0, Name = "0" });
            _state.ChequeDeck.AddCard(new Card { Value = 0, Name = "0" });
            for (var chequeThousandsAmount = 2; chequeThousandsAmount <= 15; chequeThousandsAmount++)
            {
                _state.ChequeDeck.AddCard(new Card { Value = chequeThousandsAmount, Name = chequeThousandsAmount.ToString() });
                _state.ChequeDeck.AddCard(new Card { Value = chequeThousandsAmount, Name = chequeThousandsAmount.ToString() });
            }
            _state.ChequeDeck.Shuffle();

            _state.PropertyCardsOnTable = new List<ICard>();
            _state.ChequesOnTable = new List<ICard>();
            Players = players;

            SetupPlayerStates();
        }

        private void SetupPlayerStates()
        {
            _state.PlayerStates = new Dictionary<int, ForSalePlayerState>();
            foreach(var player in Players)
            {
                _state.PlayerStates[player.Id] = new ForSalePlayerState
                {
                    PropertyCards = new List<ICard>(),
                    ChequeCards = new List<ICard>(),
                    CoinBalance = 16,
                    CoinsBid = 0
                };
            }
        }
    }
}
