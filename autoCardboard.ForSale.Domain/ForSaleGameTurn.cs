using autoCardboard.Common.Domain.Interfaces;

namespace autoCardboard.ForSale.Domain
{
    public class ForSaleGameTurn: IGameTurn
    {
        public ForSaleGameState State { get; set; }

        void Pass()
        {
            // TODO
        }

        void Bid()
        {
            // TODO
        }

        void Bid(int amount)
        {
            // TODO
            // throw an Exception if too high ( check state )
        }
    }
}
