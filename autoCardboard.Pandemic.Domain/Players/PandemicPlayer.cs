using autoCardboard.Common.Domain.Interfaces;

namespace autoCardboard.Pandemic.Domain
{
    public class PandemicPlayer: IPlayer<PandemicTurn>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public void GetTurn(PandemicTurn turn)
        {
            // TODO implement turn
        }
    }
}
