using autoCardboard.Common.Domain.Interfaces;

namespace autoCardboard.Pandemic.Domain
{
    public class PandemicPlayer: IPlayer<PandemicGameTurn>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public void GetTurn(PandemicGameTurn turn)
        {
            // TODO implement turn
        }
    }
}
