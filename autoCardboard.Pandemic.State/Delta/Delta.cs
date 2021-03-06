namespace autoCardboard.Pandemic.State.Delta
{
    public abstract class Delta: IDelta
    {
        public string GameId { get; set; }
        public virtual DeltaType DeltaType { get; set; }
    }
}
