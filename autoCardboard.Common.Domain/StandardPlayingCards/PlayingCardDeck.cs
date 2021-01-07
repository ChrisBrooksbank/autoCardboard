using System;
using autoCardboard.Common.Domain.Cards;

namespace autoCardboard.Common.Domain.StandardPlayingCards
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
