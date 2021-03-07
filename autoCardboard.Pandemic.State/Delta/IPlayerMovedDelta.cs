namespace autoCardboard.Pandemic.State.Delta
{
    public interface IPlayerMovedDelta: IDelta
    {
        int PlayerId{ get; set; }
        City City{ get; set; }
    }
}
