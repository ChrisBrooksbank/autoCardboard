using System;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Client.Publishing;
using MQTTnet.Extensions.ManagedClient;

namespace autoCardboard.Messaging
{
    public class MessageSender: IMessageSender
    {
        private readonly IManagedMqttClient _messageClient;
        private readonly MessageSenderConfiguration _messageSenderConfiguration;

        public MessageSender(MessageSenderConfiguration messageSenderConfiguration)
        {
            _messageSenderConfiguration = messageSenderConfiguration;

            if (!_messageSenderConfiguration.Enabled)
            {
                return;
            }

            var messageBuilder = new MqttClientOptionsBuilder()
                .WithClientId(messageSenderConfiguration.ClientId)
                .WithTcpServer(messageSenderConfiguration.Uri, messageSenderConfiguration.PortNumber)
                .WithCleanSession();

            var options = messageSenderConfiguration.Secure? messageBuilder.WithTls().Build() : messageBuilder.Build();

            var managedOptions = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(messageSenderConfiguration.AutoConnectDelaySeconds))
                .WithClientOptions(options)
                .Build();

            var clientFactory = new MqttFactory();
            _messageClient = clientFactory.CreateManagedMqttClient();

            _messageClient.StartAsync(managedOptions);
        }

        public Task<MqttClientPublishResult> SendMessageASync(string topic, string payload)
        {
            if (!_messageSenderConfiguration.Enabled)
            {
                return Task.FromResult(new MqttClientPublishResult
                {
                    ReasonString = "Messaging disabled"
                });
            }

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();

            return _messageClient.PublishAsync(message, CancellationToken.None);
        }

    }
}
