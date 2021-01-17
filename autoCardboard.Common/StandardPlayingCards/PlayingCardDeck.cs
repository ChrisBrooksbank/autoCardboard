using System;

namespace autoCardboard.Common
{
    public class PlayingCardDeck: CardDeck<PlayingCard>
    {
        public PlayingCardDeck()
        {
            var value = 1;
            foreach (var suit in Enum.GetValues(typeof(PlayingCardSuit)))
            {
                foreach (var rank in Enum.GetValues(typeof(PlayingCardRank)))
                {
                    AddCard( new PlayingCard
                    {
                        Value = value++,
                        Name = $"{rank} of {suit}s",
                        Suit = (PlayingCardSuit)suit,
                        Rank = (PlayingCardRank)rank
                    });
                }
            }
            Shuffle();
        }
    }
}
