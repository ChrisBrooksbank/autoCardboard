using autoCardboard.Common;
using System.Collections.Generic;
using autoCardboard.Common.Hubs;
using autoCardboard.Infrastructure;

namespace autoCardboard.Pandemic
{

    /// <summary>
    /// Implements game of pandemic
    /// </summary>
    public class PandemicGame : IGame<IPandemicState, IPandemicTurn>
    {
        private readonly ICardboardLogger _logger;
        private readonly IPandemicState _state;
        private readonly IPandemicStateEditor _stateEditor;
        private readonly IPandemicTurnValidator _validator;
        private readonly IGameHub _gameHub;

        public IEnumerable<IPlayer<IPandemicTurn>> Players { get; set; }

        public PandemicGame(ICardboardLogger logger, IPandemicState gamestate, IPandemicStateEditor stateEditor, 
            IPandemicTurnValidator validator, IGameHub gameHub)
        {
            _logger = logger;
            _state = gamestate;
            _stateEditor = stateEditor;
            _stateEditor.State = _state;
            _validator = validator;
            _gameHub = gameHub;
        }

        public void Play()
        {
            Setup(Players);

            _gameHub.SendGameState(_state);

            while (!_state.IsGameOver)
            {
                foreach (var player in Players)
                {
                    if (_state.IsGameOver)
                    {
                        break;
                    }

                    var turn = new PandemicTurn(_logger, _validator) { CurrentPlayerId = player.Id, State = _state };
                    player.GetTurn(turn);
                    ProcessTurn(turn);

                    _gameHub.SendGameState(_state);

                    // draw 2 new player cards
                    var newPlayerCards = _state.PlayerDeck.Draw(2);
                    foreach (var newPlayerCard in newPlayerCards)
                    {
                        if (newPlayerCard.PlayerCardType == PlayerCardType.Epidemic)
                        {
                            _stateEditor.Epidemic();
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

                    _stateEditor.InfectCities();
                    _gameHub.SendGameState(_state);
                }
            }

            _gameHub.SendGameState(_state);
            _logger.Information("Game Over !");
        }

        private void ProcessTurn(IPandemicTurn turn)
        {
            _stateEditor.TakeTurn(turn);
        }

        private void ProcessDiscardToHandLimitTurn(IPandemicTurn turn)
        {
            // TODO
            var playerId = turn.CurrentPlayerId;
        }

        public void Setup(IEnumerable<IPlayer<IPandemicTurn>> players)
        {
            _stateEditor.Setup(players); 
            Players = players;
        }

    }
}
