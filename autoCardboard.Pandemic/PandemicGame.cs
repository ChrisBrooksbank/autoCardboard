using System.Collections.Generic;
using autoCardboard.Common;
using autoCardboard.Infrastructure;
using autoCardboard.Messaging;
using autoCardboard.Pandemic.State;
using autoCardboard.Pandemic.TurnState;

namespace autoCardboard.Pandemic.Game
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
        private readonly IPandemicActionValidator _validator;
        private readonly IMessageSender _messageSender;

        public IPandemicState State => _state;

        public IEnumerable<IPlayer<IPandemicTurn>> Players { get; set; }

        public PandemicGame(ICardboardLogger logger, IPandemicState gamestate, IPandemicStateEditor stateEditor, 
            IPandemicActionValidator validator, IMessageSender messageSender )
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

                    for (var actionNumber = 1; actionNumber <= 4; actionNumber++)
                    {
                        var turn = new PandemicTurn(_logger, _validator) {
                            CurrentPlayerId = player.Id, State = _state, TurnType = PandemicTurnType.TakeActions
                        };
                        player.GetTurn(turn);
                        _stateEditor.ApplyTurn(_state, turn);
                    }
                  
                    // draw 2 new player cards
                    var newPlayerCards = _state.PlayerDeck.Draw(2);
                    foreach (var newPlayerCard in newPlayerCards)
                    {
                        if (State.IsGameOver)
                        {
                            break;
                        }
                        if (newPlayerCard.PlayerCardType == PlayerCardType.Epidemic)
                        {
                            _stateEditor.Epidemic(_state);
                            _state.PlayerDiscardPile.AddCard(newPlayerCard);
                        }
                        else
                        {
                            _state.PlayerStates[ player.Id].PlayerHand.Add(newPlayerCard);
                        }
                    }

                    if (!State.IsGameOver)
                    {
                        PlayerDiscardsDownToHandLimit(player, player.Id);
                        _stateEditor.InfectCities(_state);
                    }
                }
            }
            return _state;
        }
        
        private void PlayerDiscardsDownToHandLimit(IPlayer<IPandemicTurn> player, int playerId)
        {
            var playerState = _state.PlayerStates[playerId];

            while (playerState.PlayerHand.Count > PlayerHandLimit)
            {
                var discardCardsTurn = new PandemicTurn(_logger, _validator)
                {
                    CurrentPlayerId =playerId, State = _state, TurnType = PandemicTurnType.DiscardCards
                };
                player.GetTurn(discardCardsTurn);
                _stateEditor.ApplyTurn(_state, discardCardsTurn);
            }
        }

        public void Setup(IEnumerable<IPlayer<IPandemicTurn>> players)
        {
            _stateEditor.Setup(_state, players); 
            Players = players;
        }
    }
}
