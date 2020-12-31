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
            game.Setup(players);
            game.Play();
        }

    }
}
