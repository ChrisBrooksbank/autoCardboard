using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Client.Publishing;
using MQTTnet.Extensions.ManagedClient;

namespace autoCardboard.Messaging
{
    public class MessageSender: IMessageSender
    {
        private readonly IManagedMqttClient _messageClient;

        public MessageSender()
        {
            var messageBuilder = new MqttClientOptionsBuilder()
                .WithClientId("cardboardAPI")
                .WithTcpServer("localhost", 1884)
                .WithCleanSession();

            var options = false? messageBuilder.WithTls().Build() : messageBuilder.Build();

            var managedOptions = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(options)
                .Build();

            var clientFactory = new MqttFactory();
            _messageClient = clientFactory.CreateManagedMqttClient();

            _messageClient.StartAsync(managedOptions);
        }

        public Task<MqttClientPublishResult> SendMessageASync(string topic, string payload)
        {
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
