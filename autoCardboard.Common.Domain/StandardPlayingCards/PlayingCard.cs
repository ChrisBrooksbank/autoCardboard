using autoCardboard.Common.Domain.Cards;

namespace autoCardboard.Common.Domain.StandardPlayingCards
{
    public class PlayingCard: Card
    {
        public PlayingCardSuit Suit { get; set; }
        public PlayingCardRank Rank { get; set; }
    }
}
