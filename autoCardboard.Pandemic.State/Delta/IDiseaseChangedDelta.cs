namespace autoCardboard.Pandemic.State.Delta
{
    public interface IDiseaseChangedDelta: IDelta
    {
        City City{ get; set; }
        Disease Disease{ get; set; }
        int NewAmount{ get; set; }
    }
}
