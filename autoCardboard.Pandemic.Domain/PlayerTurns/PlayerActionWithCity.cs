using autoCardboard.Pandemic.Domain.State;

namespace autoCardboard.Pandemic.Domain
{
    public class PlayerActionWithCity
    {
        public PlayerStandardAction PlayerAction { get; set; }
        public City City { get; set; }
        public Disease Disease { get; set; }
    }
}
