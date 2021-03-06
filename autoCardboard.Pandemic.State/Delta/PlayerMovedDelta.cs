namespace autoCardboard.Pandemic.State.Delta
{
    public class PlayerMovedDelta: Delta, IPlayerMovedDelta
    {
        public int PlayerId { get; set; }
        public City City { get; set; }
        public override DeltaType DeltaType => DeltaType.PlayerLocation;
    }
}
