using autoCardboard.ForSale.Domain;

namespace autoCardboard.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = new ForSaleGame();
            var playerFactory = new PlayerFactory();
            var players = playerFactory.CreatePlayers(5);

            for(var gameNumber = 1; gameNumber < 100; gameNumber++)
            {
                game.Play(players);
            }
        }

    }
}
