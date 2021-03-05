namespace autoCardboard.Pandemic.State
{
    public class KnowledgeShare
    {
        public int GivingPlayerId { get; set; }
        public int ReceivingPlayerId { get; set; }
        public City CityCardToGive { get; set; }
    }
}
