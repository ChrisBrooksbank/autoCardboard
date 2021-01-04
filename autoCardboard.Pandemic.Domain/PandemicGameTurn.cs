using autoCardboard.Common.Domain.Interfaces;
using System;

namespace autoCardboard.Pandemic.Domain
{
    [Serializable]
    public class PandemicGameTurn: IGameTurn
    {
        // _state is a clone of the game state ( so any changes to it by player are ignored )
        private PandemicGameState _state;

        public int CurrentPlayerId { get; set; }

        public PandemicGameState State
        {
            get
            {
                return _state;
            }

            set
            {
                _state = value.Clone() as PandemicGameState;
            }
        }

    }
}
