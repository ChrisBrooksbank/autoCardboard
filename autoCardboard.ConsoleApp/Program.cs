using System;
using System.Collections.Generic;
using autoCardboard.Common.Domain.Interfaces;
using autoCardboard.ForSale.Domain;

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

            ShowCards(deck.Reveal(5));
            ShowCards(deck.Reveal(5));
            ShowCards(deck.Draw(5));
            ShowCards(deck.Draw(5));
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
