using autoCardboard.Common;

namespace autoCardboard.ForSale.Domain
{
    public interface IForSaleGameTurn : IGameTurn
    {
        void Pass();
        void Bid();
        void Bid(int amount);
        ICard PropertyToFlip { get; set; }
        int CurrentPlayerId { get; set; }
        IForSaleGameState State {get; set;}
    }
}
