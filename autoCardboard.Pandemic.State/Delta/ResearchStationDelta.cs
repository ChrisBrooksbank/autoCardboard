namespace autoCardboard.Pandemic.State.Delta
{
    class ResearchStationDelta: Delta, IResearchStationDelta
    {
        public City City { get; set; }
        public override DeltaType DeltaType => DeltaType.ResearchStation;
    }
}
