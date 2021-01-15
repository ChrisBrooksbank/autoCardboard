using System;
using autoCardboard.Common.Domain;
using autoCardboard.Common.Domain.Interfaces;
using System.Collections.Generic;
using autoCardboard.Pandemic.Domain.State;

namespace autoCardboard.Pandemic.Domain
{

    /// <summary>
    /// Implements game of pandemic
    /// </summary>
    public class PandemicGame : Game<IPandemicGameState, PandemicTurn>
    {

        // TODO get this DI working
        public PandemicGame(IPandemicGameState gamestate)
        {
            State = gamestate;
        }

        public override void Play(IEnumerable<IPlayer<PandemicTurn>> players)
        {
            Setup(players);

            while (!State.IsGameOver)
            {
                foreach (var player in Players)
                {
                    if (State.IsGameOver)
                    {
                        break;
                    }

                    var turn = new PandemicTurn() { CurrentPlayerId = player.Id, State = State };
                    player.GetTurn(turn);
                    ProcessTurn(turn);

                    // draw 2 new player cards
                    var newPlayerCards = State.PlayerDeck.Draw(2);
                    foreach (var newPlayerCard in newPlayerCards)
                    {
                        if (newPlayerCard.PlayerCardType == PlayerCardType.Epidemic)
                        {
                            State.Epidemic();
                            State.PlayerDiscardPile.AddCard(newPlayerCard);
                        }
                        else
                        {
                            State.PlayerStates[turn.CurrentPlayerId].PlayerHand.Add(newPlayerCard);
                        }
                    }

                    // Player must discard down to their hand limit
                    if (State.PlayerStates[turn.CurrentPlayerId].PlayerHand.Count > 7)
                    {
                        // TODO mark this turn as being to discard down to hand limit ( only )
                        player.GetTurn(turn);
                        ProcessDiscardToHandLimitTurn(turn);

                        // TODO remove this fake code, designed to allow testing of flow of game
                        var cardToDiscard = State.PlayerStates[turn.CurrentPlayerId].PlayerHand[0];
                        State.PlayerStates[turn.CurrentPlayerId].PlayerHand.Remove(cardToDiscard);
                        State.PlayerDiscardPile.AddCard(cardToDiscard);
                    }

                    State.InfectCities();
                }
            }

            Console.WriteLine("GAME OVER !!!");
        }

        private void ProcessTurn(PandemicTurn turn)
        {
           // TODO
           var playerId = turn.CurrentPlayerId;
        }

        private void ProcessDiscardToHandLimitTurn(PandemicTurn turn)
        {
            // TODO
            var playerId = turn.CurrentPlayerId;
        }

        public void Setup(IEnumerable<IPlayer<PandemicTurn>> players)
        {
            State.Setup(players); 
            Players = players;
        }

    }
}
