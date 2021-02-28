using autoCardboard.Pandemic.State;

namespace autoCardboard.Pandemic.TurnState
{
    public class PlayerEventPlayed
    {
        public int PlayerId { get; set; }
        public EventCard EventCard{ get; set; }
        public City City { get; set; }
    }
}
