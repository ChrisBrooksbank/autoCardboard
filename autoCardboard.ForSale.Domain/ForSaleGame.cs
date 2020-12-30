using autoCardboard.Common.Domain;
using autoCardboard.Common.Domain.Interfaces;
using System.Collections.Generic;

namespace autoCardboard.ForSale.Domain
{
    public class ForSaleGame : Game
    {
        // TODO add boolean GameFinished

        public override void Play()
        {
            for (var turn = 1; turn <= 3; turn++)
            {
                // TODO draw <player> cards for players to bid on, put in gamestate
                foreach (var player in Players)
                {
                    // gameState = player.TakeTurn(gameState);
                }
            }
        }

        public override void Setup(IEnumerable<IPlayer> players)
        {
            State = new Dictionary<string,object>();

            var propertyDeck = new CardDeck();
            for (var cardNumber = 1; cardNumber <= 30; cardNumber++)
            {
                propertyDeck.AddCard(new Card { Id = cardNumber, Name = cardNumber.ToString() });
            }
            propertyDeck.Shuffle();
            State["PropertyDeck"] = propertyDeck;

            Players = players;
        }
    }
}
