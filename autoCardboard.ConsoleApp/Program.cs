using System;
using System.Collections.Generic;
using autoCardboard.Common.Domain;
using autoCardboard.Common.Domain.Interfaces;
using autoCardboard.ForSale.Domain;
using autoCardboard.ForSale.Domain.Interfaces;

namespace autoCardboard.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            DrawCardsFromPropertyDeck();

            // TODO develop gameState types. Initialise gamestate.

        }

        static void DrawCardsFromPropertyDeck()
        {
            var deck = new PropertyDeck();

            List<IPlayer> players = new List<IPlayer>();
            const int playerCount = 5;
            for(int player = 1;player <=playerCount;player++)
            {
                players.Add(new Player
                {
                    Id = player,
                    Name = player.ToString(),
                    State = new PlayerState
                    {
                        PropertyCards = new List<Card>(),
                        OneThousandDollarCoinCount = playerCount < 5 ? 14 : 10,
                        TwoThousandDollarCoinCount = 2
                    }
                }); ;
            }

            var gameState = new ForSaleGameState();

            for (var turn = 1;turn<=3;turn++)
            {
                foreach (var player in players)
                {
                    // gameState = player.TakeTurn(gameState);
                }
            }

        }

        private static void ShowCards(IEnumerable<ICard> cards)
        {
            foreach (var card in cards)
            {
                Console.Write($"{card.Id} ");
            }
            Console.WriteLine();
        }
    }
}
