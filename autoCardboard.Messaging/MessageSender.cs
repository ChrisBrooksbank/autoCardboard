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

        public IManagedMqttClient Client => _messageClient;

        public MessageSender()
        {
            var mqttUri = "localhost";
            int mqttPort = 1884;
            var clientId = Guid.NewGuid().ToString();
            var mqttSecure = false;

            var messageBuilder = new MqttClientOptionsBuilder()
                .WithClientId(clientId)
                .WithTcpServer(mqttUri, mqttPort)
                .WithCleanSession();

            var options = mqttSecure ? messageBuilder.WithTls().Build() : messageBuilder.Build();

            var managedOptions = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(options)
                .Build();

            var clientFactory = new MqttFactory();
            _messageClient = clientFactory.CreateManagedMqttClient();

            _messageClient.StartAsync(managedOptions);
            SendMessageASync("AutoCardboard", "Started messenger client");
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
