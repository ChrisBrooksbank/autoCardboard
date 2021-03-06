using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using autoCardboard.Common;
using autoCardboard.Infrastructure;
using autoCardboard.Messaging;
using autoCardboard.Pandemic.State;
using autoCardboard.Pandemic.State.Delta;
using autoCardboard.Pandemic.TurnState;
using Newtonsoft.Json;

namespace autoCardboard.Pandemic.Game
{
    /// <summary>
    /// Implements game of pandemic
    /// </summary>
    public class PandemicGame : IGame<IPandemicState, IPandemicTurn>
    {
        private const int PlayerHandLimit = 7;

        private readonly IPandemicState _state;
        private readonly IPandemicStateEditor _stateEditor;
        private readonly IPandemicActionValidator _validator;
        private readonly IMessageSender _messageSender;
        private readonly MessageSenderConfiguration _messageSenderConfiguration;

        public IPandemicState State => _state;

        public IEnumerable<IPlayer<IPandemicTurn>> Players { get; set; }

        public PandemicGame(IPandemicState gamestate, IPandemicStateEditor stateEditor,
            IPandemicActionValidator validator, IMessageSender messageSender,
            MessageSenderConfiguration messageSenderConfiguration)
        {
            _state = gamestate;
            _stateEditor = stateEditor;
            _validator = validator;
            _messageSender = messageSender;
            _messageSenderConfiguration = messageSenderConfiguration;
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

                    AllowPlayersToPlayEventCards();
                    for (var actionNumber = 1; actionNumber <= 4; actionNumber++)
                    {
                        var turn = new PandemicTurn(_validator)
                        {
                            CurrentPlayerId = player.Id, State = _state, TurnType = PandemicTurnType.TakeActions
                        };
                        player.GetTurn(turn);
                        _stateEditor.ApplyTurn(_state, turn);
                        BroadCastDeltaForTurn(_state, turn);
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
                            _state.PlayerStates[player.Id].PlayerHand.Add(newPlayerCard);
                        }
                    }

                    if (!State.IsGameOver)
                    {
                        AllowPlayersToPlayEventCards();
                        PlayerDiscardsDownToHandLimit(player, player.Id);
                        _stateEditor.InfectCities(_state);
                    }
                }
            }

            return _state;
        }

        private void AllowPlayersToPlayEventCards()
        {
            if (_state.IsGameOver)
            {
                return;
            }

            foreach (var player in Players)
            {
                var playerState = _state.PlayerStates[player.Id];
                if (playerState.PlayerHand.Any(c => c.PlayerCardType == PlayerCardType.Event))
                {
                    var playEventsTurn = new PandemicTurn(_validator)
                    {
                        CurrentPlayerId = player.Id, State = _state, TurnType = PandemicTurnType.PlayEventCards
                    };

                    player.GetTurn(playEventsTurn);
                    _stateEditor.ApplyTurn(_state, playEventsTurn);
                }
            }

        }

        private void PlayerDiscardsDownToHandLimit(IPlayer<IPandemicTurn> player, int playerId)
        {
            var playerState = _state.PlayerStates[playerId];

            while (playerState.PlayerHand.Count > PlayerHandLimit)
            {
                var discardCardsTurn = new PandemicTurn(_validator)
                {
                    CurrentPlayerId = playerId, State = _state, TurnType = PandemicTurnType.DiscardCards
                };
                player.GetTurn(discardCardsTurn);
                _stateEditor.ApplyTurn(_state, discardCardsTurn);
            }
        }

        public void Setup(IEnumerable<IPlayer<IPandemicTurn>> players)
        {
            var deltas = _stateEditor.Setup(_state, players);
            Players = players;
            var topic = _messageSenderConfiguration.TopicStateDelta.Replace("gameId",_state.Id);
            BroadCastStateDeltas(topic, deltas);
        }

        // TODO
        private void BroadCastDeltaForTurn(IPandemicState state, PandemicTurn turn)
        {
        }

        private void BroadCastStateDeltas(string topic, IEnumerable<IDelta> deltas)
        {
            var tasks = deltas
                .Select(stateDelta => JsonConvert.SerializeObject(stateDelta, Formatting.Indented))
                .Select(payLoad => _messageSender.SendMessageASync(topic, payLoad)).ToArray();
            Task.WaitAll(tasks, CancellationToken.None);
        }

}
}
