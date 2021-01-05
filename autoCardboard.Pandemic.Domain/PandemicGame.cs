using System;
using autoCardboard.Common.Domain;
using autoCardboard.Common.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace autoCardboard.Pandemic.Domain
{
    public class PandemicGame : Game<PandemicGameState, PandemicGameTurn>
    {
        public PandemicGame()
        {

            State = new PandemicGameState();
        }

        // TODO
        public override void Play(IEnumerable<IPlayer<PandemicGameTurn>> players)
        {
            Setup(players);
        }

        private void Setup(IEnumerable<IPlayer<PandemicGameTurn>> players)
        {
            // TODO refactor e.g. into a new PlayerDeck type
            State.PlayerDeck = new CardDeck<PandemicPlayerCard>();
            foreach (var city in Enum.GetValues(typeof(City)))
            {
                State.PlayerDeck.AddCard(new PandemicPlayerCard{ Value = (int)city, Name = city.ToString(), PlayerCardType = PlayerCardType.City});
            }
            foreach (var eventCard in Enum.GetValues(typeof(EventCard)))
            {
                State.PlayerDeck.AddCard(new PandemicPlayerCard{ Value = (int)eventCard, Name = eventCard.ToString(), PlayerCardType = PlayerCardType.Event});
            }
            State.PlayerDeck.Shuffle();
            var playerDeckCardPiles = State.PlayerDeck.Divide(State.EpidemicCardCount).ToList();
            foreach (var cardDeck in playerDeckCardPiles)
            {
                cardDeck.AddCard( new PandemicPlayerCard{ PlayerCardType = PlayerCardType.Epidemic, Name = "Epidemic", Value = 0});
                cardDeck.Shuffle();
            }

            State.PlayerDeck.Add(playerDeckCardPiles);

            Console.WriteLine();
            Console.Write("Pandemic player deck : ");
            while (!State.PlayerDeck.Empty)
            {
                var card = State.PlayerDeck.Draw(1).Single() as PandemicPlayerCard;
                switch (card.PlayerCardType)
                {
                    case PlayerCardType.City:
                        Console.WriteLine($"{card.Name} ");
                        break;
                    case PlayerCardType.Epidemic:
                        Console.WriteLine($"***{card.Name} ");
                        break;
                    case PlayerCardType.Event:
                        Console.WriteLine($"**{card.Name} ");
                        break;
                }
            }

            Console.ReadLine();


            //SetupPlayerStates();
        }

        private void SetupPlayerStates()
        {
            State.PlayerStates = new Dictionary<int, PandemicPlayerState>();
            foreach (var player in Players)
            {
                State.PlayerStates[player.Id] = new PandemicPlayerState
                {
                   // TODO set properties
                };
            }
        }
    }
}
