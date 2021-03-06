namespace autoCardboard.Pandemic.State.Delta
{
    public interface IDelta
    {
        string GameId { get; set; }
        DeltaType DeltaType { get; }
    }
}
