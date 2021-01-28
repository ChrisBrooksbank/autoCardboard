using autoCardboard.Pandemic.State;

namespace autoCardboard.Pandemic
{
    public class PlayerAction
    {
        public int PlayerId { get; set; }
        public PlayerActionType PlayerActionType { get; set; }
        public City City { get; set; }
        public Disease Disease { get; set; }
    }
}
