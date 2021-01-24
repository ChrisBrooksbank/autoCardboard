using System;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Server;

namespace autoCardboard.Message.Server
{

    class Program
    {
        static void Main(string[] args)
        {
            StartMQTTServer();
        }

        static async void StartMQTTServer()
        {
            var optionsBuilder = new MqttServerOptionsBuilder()
                .WithConnectionBacklog(100)
                .WithDefaultEndpointPort(1884);

            var mqttServer = new MqttFactory().CreateMqttServer();

            mqttServer.UseClientConnectedHandler(ClientConnectedHandler);

            await mqttServer.StartAsync(optionsBuilder.Build());

            Console.WriteLine("AutoCardboard messaging server listening on port 1884 - press enter to exit.");
            Console.ReadLine();
            await mqttServer.StopAsync();
        }

        private static Task ClientConnectedHandler(MqttServerClientConnectedEventArgs arg)
        {
            Console.WriteLine("Somebody connected");
            return Task.FromResult(12);
        }

    }
}
