using autoCardboard.Common;

namespace autoCardboard.Pandemic.Domain
{
    public class PandemicPlayer: IPlayer<IPandemicTurn>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public void GetTurn(IPandemicTurn turn)
        {
            // TODO implement turn
        }
    }
}
