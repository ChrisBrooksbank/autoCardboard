using System;
using autoCardboard.Common.Domain.Interfaces;
using System.Collections.Generic;
using autoCardboard.Pandemic.Domain.State;

namespace autoCardboard.Pandemic.Domain
{

    /// <summary>
    /// Implements game of pandemic
    /// </summary>
    public class PandemicGame : IGame<IPandemicGameState, PandemicTurn>
    {
        private readonly IPandemicGameState _state;

        public IEnumerable<IPlayer<PandemicTurn>> Players { get; set; }

        // TODO get this DI working
        public PandemicGame(IPandemicGameState gamestate)
        {
            _state = gamestate;
        }

        public void Play(IEnumerable<IPlayer<PandemicTurn>> players)
        {
            Setup(players);

            while (!_state.IsGameOver)
            {
                foreach (var player in Players)
                {
                    if (_state.IsGameOver)
                    {
                        break;
                    }

                    var turn = new PandemicTurn() { CurrentPlayerId = player.Id, State = _state };
                    player.GetTurn(turn);
                    ProcessTurn(turn);

                    // draw 2 new player cards
                    var newPlayerCards = _state.PlayerDeck.Draw(2);
                    foreach (var newPlayerCard in newPlayerCards)
                    {
                        if (newPlayerCard.PlayerCardType == PlayerCardType.Epidemic)
                        {
                            _state.Epidemic();
                            _state.PlayerDiscardPile.AddCard(newPlayerCard);
                        }
                        else
                        {
                            _state.PlayerStates[turn.CurrentPlayerId].PlayerHand.Add(newPlayerCard);
                        }
                    }

                    // Player must discard down to their hand limit
                    if (_state.PlayerStates[turn.CurrentPlayerId].PlayerHand.Count > 7)
                    {
                        // TODO mark this turn as being to discard down to hand limit ( only )
                        player.GetTurn(turn);
                        ProcessDiscardToHandLimitTurn(turn);

                        // TODO remove this fake code, designed to allow testing of flow of game
                        var cardToDiscard = _state.PlayerStates[turn.CurrentPlayerId].PlayerHand[0];
                        _state.PlayerStates[turn.CurrentPlayerId].PlayerHand.Remove(cardToDiscard);
                        _state.PlayerDiscardPile.AddCard(cardToDiscard);
                    }

                    _state.InfectCities();
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
            _state.Setup(players); 
            Players = players;
        }

    }
}
