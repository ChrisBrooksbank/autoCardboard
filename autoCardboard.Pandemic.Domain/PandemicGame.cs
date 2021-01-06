using autoCardboard.Common.Domain;
using autoCardboard.Common.Domain.Interfaces;
using System.Collections.Generic;

namespace autoCardboard.Pandemic.Domain
{
    public class PandemicGame : Game<PandemicGameState, PandemicGameTurn>
    {
        public PandemicGame()
        {
            State = new PandemicGameState();
        }

        // TODO
        public override void Play(IEnumerable<IPlayer<PandemicGameTurn>> players)
        {
            Setup(players);
        }

        private void Setup(IEnumerable<IPlayer<PandemicGameTurn>> players)
        {
            State.Board.Setup();
            //SetupPlayerStates();
        }

        private void SetupPlayerStates()
        {
            State.PlayerStates = new Dictionary<int, PandemicPlayerState>();
            foreach (var player in Players)
            {
                State.PlayerStates[player.Id] = new PandemicPlayerState
                {
                   // TODO set properties
                };
            }
        }
    }
}
