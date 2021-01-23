using System;
using MQTTnet;
using MQTTnet.Server;

namespace autoCardboard.Message.Server
{

    // https://github.com/chkr1011/MQTTnet/wiki/Server
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
            await mqttServer.StartAsync(optionsBuilder.Build());

            Console.WriteLine("AutoCardboard messaging server - press any key to exit.");
            Console.ReadKey();
            await mqttServer.StopAsync();
        }
    }
}
