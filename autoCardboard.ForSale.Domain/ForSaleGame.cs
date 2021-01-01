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

        public override void Setup(IEnumerable<IPlayer<ForSaleGameTurn>> players)
        {
            State.PropertyDeck = new CardDeck();
            for (var cardNumber = 1; cardNumber <= 30; cardNumber++)
            {
                State.PropertyDeck.AddCard(new Card { Id = cardNumber, Name = cardNumber.ToString() });
            }
            State.PropertyDeck.Shuffle();
          
            State.PropertyCardsOnTable = new List<ICard>();
            State.CurrentBid = 0;
            Players = players;

            SetupPlayerStates();
        }

        public override void Play()
        {
            while (!State.PropertyDeck.Empty)
            {
                PlayPropertyBidRound();
            }
        }

        private void PlayPropertyBidRound()
        {
            State.PropertyCardsOnTable = State.PropertyDeck.Draw(Players.Count());

            foreach (var player in Players)
            {
                var turn = new ForSaleGameTurn { State = State };
                player.GetTurn(turn);

                // IF player passed, they get half their coins back and the lowest valued property deck
            }
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
