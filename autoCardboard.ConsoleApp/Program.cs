using System;
using autoCardboard.ForSale.Domain;

namespace autoCardboard.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            DrawCardsFromPropertyDeck();

            // TODO create five players
            // implement a TakeTurn() which just says "player n taking turn"

            Console.ReadLine();
        }

        static void DrawCardsFromPropertyDeck()
        {
            var deck = new PropertyDeck();

            var cards = deck.Draw(30);

            foreach (var card in cards)
            {
                Console.WriteLine(card.Id);
            }
        }
    }
}
