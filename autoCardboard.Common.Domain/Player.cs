using autoCardboard.Common.Domain.Interfaces;

namespace autoCardboard.Common.Domain
{
    public class Player : IPlayer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IPlayerState State { get; set; }

        public IGameState TakeTurn(IGameState gameState)
        {
            throw new System.NotImplementedException();
        }
    }
}
