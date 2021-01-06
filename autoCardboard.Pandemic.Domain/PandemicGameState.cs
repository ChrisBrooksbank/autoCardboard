using autoCardboard.Common.Domain;
using System.Collections.Generic;

namespace autoCardboard.Pandemic.Domain
{
    public class PandemicGameState: GameState
    {
        public PandemicBoard Board { get; set; }
        public Dictionary<int, PandemicPlayerState> PlayerStates { get; set; }

        public PandemicGameState()
        {
            Board = new PandemicBoard();
            PlayerStates = new Dictionary<int,PandemicPlayerState>();
        }
    }
}
