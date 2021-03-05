using System.Collections.Generic;
using autoCardboard.Pandemic.State;

namespace autoCardboard.Pandemic.TurnState
{
    public class PlayerAction
    {
        public int PlayerId { get; set; }
        public int OtherPlayerId { get; set; }
        public PlayerActionType PlayerActionType { get; set; }
        public City City { get; set; }
        public Disease Disease { get; set; }
        public IEnumerable<PandemicPlayerCard> CardsToDiscard { get; set; }
    }
}
