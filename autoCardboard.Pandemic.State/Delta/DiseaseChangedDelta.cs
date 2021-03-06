namespace autoCardboard.Pandemic.State.Delta
{
    public class DiseaseChangedDelta: Delta, IDiseaseChangedDelta
    {
        public City City { get; set; }
        public Disease Disease { get; set; }
        public int NewAmount { get; set; }
        public override DeltaType DeltaType => DeltaType.Disease;
    }
}
