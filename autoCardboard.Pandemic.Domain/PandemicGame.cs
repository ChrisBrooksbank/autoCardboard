﻿using System;
using autoCardboard.Common.Domain;
using autoCardboard.Common.Domain.Interfaces;
using System.Collections.Generic;

namespace autoCardboard.Pandemic.Domain
{
    public class PandemicGame : Game<PandemicGameState, PandemicGameTurn>
    {
        public PandemicGame()
        {
            State = new PandemicGameState();
        }

        public override void Play(IEnumerable<IPlayer<PandemicGameTurn>> players)
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

                    var turn = new PandemicGameTurn() { CurrentPlayerId = player.Id, State = State };
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

                    State.Infect();
                }
            }

            Console.WriteLine("GAME OVER !!!");
        }

        private void ProcessTurn(PandemicGameTurn turn)
        {
           // TODO
           var playerId = turn.CurrentPlayerId;

        }

        private void ProcessDiscardToHandLimitTurn(PandemicGameTurn turn)
        {
            // TODO
            var playerId = turn.CurrentPlayerId;

        }

        private void Setup(IEnumerable<IPlayer<PandemicGameTurn>> players)
        {
            State.Setup();
            Players = players;
            SetupPlayerStates();
        }

        private void SetupPlayerStates()
        {
            var roleDeck = new RoleDeck();
            
            State.PlayerStates = new Dictionary<int, PandemicPlayerState>();
            foreach (var player in Players)
            {
                State.PlayerStates[player.Id] = new PandemicPlayerState
                {
                    PlayerHand = new List<PandemicPlayerCard>(), // TODO start with 4 cards
                    Location = City.Atlanta,
                    PlayerRole = (PlayerRole)roleDeck.DrawTop().Value
                };
            }
        }
    }
}
