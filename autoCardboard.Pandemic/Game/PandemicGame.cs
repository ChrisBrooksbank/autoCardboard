using autoCardboard.Common;
using System.Collections.Generic;
using autoCardboard.Infrastructure;

namespace autoCardboard.Pandemic
{

    /// <summary>
    /// Implements game of pandemic
    /// </summary>
    public class PandemicGame : IGame<IPandemicGameState, IPandemicTurn>
    {
        private readonly ICardboardLogger _logger;
        private readonly IPandemicGameState _state;
        private readonly IPandemicTurnValidator _validator;
        private readonly IGameTurnHandler<IPandemicGameState, IPandemicTurn> _turnHandler;

        public IEnumerable<IPlayer<IPandemicTurn>> Players { get; set; }

        public PandemicGame(ICardboardLogger logger, IPandemicGameState gamestate, IPandemicTurnValidator validator,
            IGameTurnHandler<IPandemicGameState, IPandemicTurn> turnHandler)
        {
            _logger = logger;
            _state = gamestate;
            _validator = validator;
            _turnHandler = turnHandler;
        }

        public void Play()
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

                    var turn = new PandemicTurn(_logger, _validator) { CurrentPlayerId = player.Id, State = _state };
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

            _logger.Information("Game Over !");
        }

        private void ProcessTurn(IPandemicTurn turn)
        {
            _turnHandler.TakeTurn(_state, turn);
        }

        private void ProcessDiscardToHandLimitTurn(IPandemicTurn turn)
        {
            // TODO
            var playerId = turn.CurrentPlayerId;
        }

        public void Setup(IEnumerable<IPlayer<IPandemicTurn>> players)
        {
            _state.Setup(players); 
            Players = players;
        }

    }
}
