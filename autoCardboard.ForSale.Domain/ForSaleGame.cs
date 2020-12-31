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

        // TODO as soon as a player passes, they should get the lowest property card left on the table
        public override void Play()
        {
            var propertyBiddingIsFinished = false;
            var propertyDeck = State.PropertyDeck;
            var playerCount = Players.Count();


            while (!propertyBiddingIsFinished)
            {
                var propertiesToBidOn = propertyDeck.Draw(playerCount);

                if (propertiesToBidOn.Count() == playerCount)
                {
                    State.PropertyCardsOnTable = propertiesToBidOn;

                    Console.Write("Property cards drawn to bid on : ");
                    foreach(var propertyCard in propertiesToBidOn)
                    {
                        Console.Write(propertyCard.Name + " ");
                    }
                    Console.WriteLine("");

                    var activeBidders = true;

                    while (activeBidders)
                    {
                        foreach (var player in Players)
                        {
                            //TODO this changing !!
                            //State = player.TakeTurn(State);
                        }

                        if (Players.Count(p => ((string)p.State["LastAction"]).Equals("bid")) < 2)
                        {
                            activeBidders = false;
                            HandleEndOfBiddingTurn();
                        }

                    }
                }
                else
                {
                    propertyBiddingIsFinished = true;
                }
            }
        }

        public override void Setup(IEnumerable<IPlayer> players)
        {
            var propertyDeck = new CardDeck();
            for (var cardNumber = 1; cardNumber <= 30; cardNumber++)
            {
                propertyDeck.AddCard(new Card { Id = cardNumber, Name = cardNumber.ToString() });
            }
            propertyDeck.Shuffle();
            State.PropertyDeck = propertyDeck;

            State.PropertyCardsOnTable = new List<ICard>();
            State.CurrentBid = 0;

            Players = players;
        }

        private void HandleEndOfBiddingTurn()
        {
            var propertiesInOrder = State.PropertyCardsOnTable.OrderByDescending(c => c.Id).ToList();
            var playersInBidOrder = Players.OrderByDescending(p => (int)p.State["CoinsBid"]).ToList();

            foreach(var player in playersInBidOrder)
            {
                var playerPropertyCards = (List<ICard>)player.State["PropertyCards"];
                var propertyCardWon = propertiesInOrder[0];
                playerPropertyCards.Add(propertyCardWon);
                propertiesInOrder.Remove(propertyCardWon);
                Console.WriteLine($"Player {player.Id} won property {propertyCardWon.Id}");
            }

        }
    }
}
