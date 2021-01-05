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
            State.EpidemicCardCount = 6;

            State.InfectionDeck = new InfectionDeck();

            State.PlayerDeck = new PlayerDeck();
            State.PlayerDeck.Setup(State.EpidemicCardCount);
          
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
