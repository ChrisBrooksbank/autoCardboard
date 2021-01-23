using System;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;

namespace autoCardboard.Messaging
{
    // https://dzone.com/articles/mqtt-publishing-and-subscribing-messages-to-mqtt-b
    public class GameMessenger
    {
        public async void Connect()
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
            var client = clientFactory.CreateManagedMqttClient();

            await client.StartAsync(managedOptions);
        }
    }
}
