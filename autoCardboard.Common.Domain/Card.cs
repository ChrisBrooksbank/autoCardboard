using autoCardboard.Common.Domain.Interfaces;

namespace autoCardboard.Common.Domain
{
    public class Card: ICard
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
