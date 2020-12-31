using autoCardboard.Common.Domain.Interfaces;

namespace autoCardboard.ForSale.Domain
{
    public class ForSaleGameTurn: IGameTurn
    {
        public ForSaleGameState State { get; set; }

        public void Pass()
        {
            // TODO
        }

        public void Bid()
        {
            // TODO
        }

        public void Bid(int amount)
        {
            // TODO
            // throw an Exception if too high ( check state )
        }
    }
}
