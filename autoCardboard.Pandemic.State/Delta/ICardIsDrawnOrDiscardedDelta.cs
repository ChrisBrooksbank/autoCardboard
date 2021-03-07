namespace autoCardboard.Pandemic.State.Delta
{
    interface ICardIsDrawnOrDiscardedDelta: IDelta
    {
        int? PlayerId { get; set; }
        PandemicPlayerCard PandemicPlayerCard{ get; set; }
        City? InfectionCard{ get; set; }
        DrawnOrDiscarded DrawnOrDiscarded{ get; set; }
    }
}
