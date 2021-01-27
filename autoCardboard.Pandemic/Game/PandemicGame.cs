using autoCardboard.Common;
using System.Collections.Generic;
using autoCardboard.Infrastructure;
using autoCardboard.Messaging;

namespace autoCardboard.Pandemic
{

    /// <summary>
    /// Implements game of pandemic
    /// </summary>
    public class PandemicGame : IGame<IPandemicState, IPandemicTurn>
    {
        private const int PlayerHandLimit = 7;

        private readonly ICardboardLogger _logger;
        private readonly IPandemicState _state;
        private readonly IPandemicStateEditor _stateEditor;
        private readonly IPandemicTurnValidator _validator;
        private readonly IMessageSender _messageSender;

        public IPandemicState State => _state;

        public IEnumerable<IPlayer<IPandemicTurn>> Players { get; set; }

        public PandemicGame(ICardboardLogger logger, IPandemicState gamestate, IPandemicStateEditor stateEditor, 
            IPandemicTurnValidator validator, IMessageSender messageSender )
        {
            _logger = logger;
            _state = gamestate;
            _stateEditor = stateEditor;
            _validator = validator;
            _messageSender = messageSender;
        }

        public IGameState Play()
        {
            Setup(Players);

            while (!_state.IsGameOver)
            {
                foreach (var player in Players)
                {
                    if (_state.IsGameOver)
                    {
                        break;
                    }

                    var turn = new PandemicTurn(_logger, _validator) { CurrentPlayerId = player.Id, State = _state};

                    _messageSender.SendMessageASync($"AutoCardboard/Pandemic/Player/{turn.CurrentPlayerId}", $"Getting turn");
                    player.GetTurn(turn);
                    ProcessTurn(turn);
                    _messageSender.SendMessageASync($"AutoCardboard/Pandemic/Player/{turn.CurrentPlayerId}", $"Finished turn");

                    // draw 2 new player cards
                    var newPlayerCards = _state.PlayerDeck.Draw(2);
                    foreach (var newPlayerCard in newPlayerCards)
                    {
                        if (newPlayerCard.PlayerCardType == PlayerCardType.Epidemic)
                        {
                            _messageSender.SendMessageASync($"AutoCardboard/Pandemic/Player/{turn.CurrentPlayerId}", $"*** Draws Epidemic ***");
                            _stateEditor.Epidemic(_state);
                            _state.PlayerDiscardPile.AddCard(newPlayerCard);
                        }
                        else
                        {
                            _state.PlayerStates[turn.CurrentPlayerId].PlayerHand.Add(newPlayerCard);
                            _messageSender.SendMessageASync($"AutoCardboard/Pandemic/Player/{turn.CurrentPlayerId}", $"Draws card");
                        }
                    }

                    CurrentPlayerDiscardsDownToHandLimit(turn);

                    _stateEditor.InfectCities(_state);
                }
            }

            _logger.Information("Game Over !");
            return _state;
        }

        private void ProcessTurn(IPandemicTurn turn)
        {
            _stateEditor.TakeTurn(_state, turn);
        }

        private void CurrentPlayerDiscardsDownToHandLimit(IPandemicTurn turn)
        {
            var playerId = turn.CurrentPlayerId;
            var playerState = _state.PlayerStates[playerId];

            while (playerState.PlayerHand.Count > PlayerHandLimit)
            {
                var discardCardAtIndex = new Die(playerState.PlayerHand.Count).Roll();
                var cardToDiscard = _state.PlayerStates[turn.CurrentPlayerId].PlayerHand[discardCardAtIndex - 1];
                _state.PlayerStates[turn.CurrentPlayerId].PlayerHand.Remove(cardToDiscard);
                _state.PlayerDiscardPile.AddCard(cardToDiscard);
            }
        }

        public void Setup(IEnumerable<IPlayer<IPandemicTurn>> players)
        {
            _stateEditor.Setup(_state, players); 
            Players = players;
        }
        
    }
}
