using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using autoCardboard.Common;
using autoCardboard.Messaging;
using autoCardboard.Pandemic.State;
using autoCardboard.Pandemic.State.Delta;
using autoCardboard.Pandemic.TurnState;
using MQTTnet.Client.Publishing;
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
            BroadcastGameStart();
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
                        var stateDeltas =_stateEditor.ApplyTurn(_state, turn);
                        BroadCastStateDeltas(stateDeltas);
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
                            var stateDeltas = _stateEditor.Epidemic(_state);
                            BroadCastStateDeltas(stateDeltas);
                            _state.PlayerDiscardPile.AddCard(newPlayerCard);
                        }
                        else
                        {
                            _state.PlayerStates[player.Id].PlayerHand.Add(newPlayerCard);
                            var stateDeltas = new List<IDelta>() { new CardIsDrawnOrDiscardedDelta
                            {
                                PlayerId = player.Id,
                                PandemicPlayerCard = newPlayerCard,
                                DrawnOrDiscarded = DrawnOrDiscarded.Drawn
                            } };
                            BroadCastStateDeltas(stateDeltas);
                        }
                    }

                    if (!State.IsGameOver)
                    {
                       AllowPlayersToPlayEventCards();
                       PlayerDiscardsDownToHandLimit(player, player.Id);
                       var stateDeltas =  _stateEditor.InfectCities(_state);
                       BroadCastStateDeltas(stateDeltas);
                    }
                }
            }

            BroadcastGameEnd();

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
                    var stateDeltas = _stateEditor.ApplyTurn(_state, playEventsTurn);
                    BroadCastStateDeltas(stateDeltas);
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
                var stateDeltas = _stateEditor.ApplyTurn(_state, discardCardsTurn);
                BroadCastStateDeltas(stateDeltas);
            }
        }

        public void Setup(IEnumerable<IPlayer<IPandemicTurn>> players)
        {
            var deltas = _stateEditor.Setup(_state, players);
            Players = players;
            BroadCastStateDeltas(deltas);
        }
        
        private void BroadcastGameStart()
        {
            var topic = ExpandPlaceHolders(_messageSenderConfiguration.TopicGameStart);
            var payload = "OK";
            Task.WaitAll(_messageSender.SendMessageASync(topic, payload));
        }

        private async void BroadcastGameEnd()
        {
            var topic = ExpandPlaceHolders(_messageSenderConfiguration.TopicGameEnd);
            var payload = JsonConvert.SerializeObject(new { _state.GameOverReason });
            Task.WaitAll(_messageSender.SendMessageASync(topic, payload));
        }

        private void BroadCastStateDeltas(IEnumerable<IDelta> deltas)
        {
            var broadCastTasks = new List<Task<MqttClientPublishResult>>();
            foreach (var delta in deltas)
            {
                var topicAndPayLoad = MapDeltaToTopicAndPayLoad(delta);
                if (!string.IsNullOrEmpty(topicAndPayLoad.Item1) && !string.IsNullOrEmpty(topicAndPayLoad.Item2))
                {
                    var broadCastTask = _messageSender.SendMessageASync(topicAndPayLoad.Item1, topicAndPayLoad.Item2);
                    broadCastTasks.Add(broadCastTask);
                }
            }

            Task.WaitAll(broadCastTasks.ToArray(), CancellationToken.None);
        }

        private (string, string) MapDeltaToTopicAndPayLoad(IDelta delta)
        {
            var topic = string.Empty;
            var payload = string.Empty;

            switch (delta.DeltaType)
            {
                case DeltaType.PlayerSetup:
                    var setupDelta = delta as PlayerSetupDelta;
                    topic = ExpandPlaceHolders(_messageSenderConfiguration.TopicBotSetup);
                    payload = JsonConvert.SerializeObject(new { setupDelta.PlayerId, setupDelta.PlayerRole, setupDelta.City});
                    break;
                case DeltaType.PlayerLocation:
                    var locationDelta = delta as PlayerMovedDelta;
                    topic = ExpandPlaceHolders(_messageSenderConfiguration.TopicBotMoved);
                    payload =  JsonConvert.SerializeObject(new { Id = locationDelta.PlayerId, City = locationDelta.City});
                    break;
                case DeltaType.Disease:
                    var diseaseDelta = delta as DiseaseChangedDelta;
                    topic = ExpandPlaceHolders(_messageSenderConfiguration.TopicInfect);
                    payload = JsonConvert.SerializeObject(new { diseaseDelta.City, diseaseDelta.Disease, diseaseDelta.NewAmount});
                    break;
                case DeltaType.DiseaseStateChanged:
                    var diseaseStateDelta = delta as DiseaseStateChanged;
                    topic = ExpandPlaceHolders(_messageSenderConfiguration.TopicCure);
                    payload = JsonConvert.SerializeObject(new {diseaseStateDelta.Disease, diseaseStateDelta.DiseaseState });
                    break;
                case DeltaType.ResearchStation:
                    var buildDelta = delta as ResearchStationDelta;
                    topic = ExpandPlaceHolders(_messageSenderConfiguration.TopicBuilt);
                    payload = JsonConvert.SerializeObject(new { buildDelta.City});
                    break;
                case DeltaType.CardDrawnOrDiscarded:
                    var cardDelta = delta as CardIsDrawnOrDiscardedDelta;
                    if (cardDelta.DrawnOrDiscarded == DrawnOrDiscarded.Discarded)
                    {
                        topic = ExpandPlaceHolders(_messageSenderConfiguration.TopicBotDiscarded);
                        payload = JsonConvert.SerializeObject(new { cardDelta.PlayerId, cardDelta.PandemicPlayerCard});
                    }
                    else
                    {
                        topic = ExpandPlaceHolders(_messageSenderConfiguration.TopicBotDrew);
                        payload = JsonConvert.SerializeObject(new { cardDelta.PlayerId, cardDelta.PandemicPlayerCard});
                    }
                    break;
            }

            return (topic,payload);
        }
        
        private string ExpandPlaceHolders(string payLoad)
        {
            return payLoad.Replace("{gameId}",_state.Id);
        }
  }
}
