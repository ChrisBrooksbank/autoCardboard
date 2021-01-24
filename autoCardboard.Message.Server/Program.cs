using System;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Server;

namespace autoCardboard.Message.Server
{

    /// <summary>
    /// This is a message server hosted in a console application
    /// Send messages using a IMessageSender
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            StartServer();
        }

        static async void StartServer()
        {
            var optionsBuilder = new MqttServerOptionsBuilder()
                .WithConnectionBacklog(100)
                .WithDefaultEndpointPort(1884);

            var mqttServer = new MqttFactory().CreateMqttServer();

            mqttServer.UseClientConnectedHandler(ClientConnectedHandler);
            mqttServer.UseClientDisconnectedHandler(ClientDisconnectedHandler);
            mqttServer.UseApplicationMessageReceivedHandler(MessageReceivedHandler);

            await mqttServer.StartAsync(optionsBuilder.Build());

            Console.WriteLine("AutoCardboard messaging server listening on port 1884 - press enter to exit.");
            Console.ReadLine();
            await mqttServer.StopAsync();
        }

        private static Task ClientConnectedHandler(MqttServerClientConnectedEventArgs arg)
        {
            Console.WriteLine($"Client {arg.ClientId} connected");
            return Task.FromResult(12);
        }

        private static Task ClientDisconnectedHandler(MqttServerClientDisconnectedEventArgs arg)
        {
            Console.WriteLine($"Client {arg.ClientId} disconnected");
            return Task.FromResult(12);
        }

        private static Task MessageReceivedHandler(MqttApplicationMessageReceivedEventArgs arg)
        {
            Console.WriteLine($"Received message from client {arg.ClientId} with topic {arg.ApplicationMessage.Topic} : {Encoding.UTF8.GetString(arg.ApplicationMessage.Payload)}");
            return Task.FromResult(12);
        }

    }
}
