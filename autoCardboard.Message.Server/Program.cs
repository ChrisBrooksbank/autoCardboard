using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
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
        private static IConfiguration _configuration;

        static void Main(string[] args)
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            StartServer();
        }

        static async void StartServer()
        {
            var optionsBuilder = new MqttServerOptionsBuilder()
                .WithConnectionBacklog(int.Parse(_configuration["backlog"]))
                .WithDefaultEndpointPort(int.Parse(_configuration["portNumber"]));

            var mqttServer = new MqttFactory().CreateMqttServer();

            var isEchoOn = bool.Parse(_configuration["echo"]);

            if (isEchoOn)
            {
                mqttServer.UseClientConnectedHandler(ClientConnectedHandler);
                mqttServer.UseClientDisconnectedHandler(ClientDisconnectedHandler);
                mqttServer.UseApplicationMessageReceivedHandler(MessageReceivedHandler);
            }
          
            await mqttServer.StartAsync(optionsBuilder.Build());

            Console.WriteLine($"Mqtt broker listening on port {_configuration["portNumber"]} echoOn={isEchoOn} - press enter to exit.");
            Console.ReadLine();
            await mqttServer.StopAsync();
        }

        private static Task ClientConnectedHandler(MqttServerClientConnectedEventArgs arg)
        {
            //Console.WriteLine($"Client {arg.ClientId} connected");
            return Task.FromResult(12);
        }

        private static Task ClientDisconnectedHandler(MqttServerClientDisconnectedEventArgs arg)
        {
            //Console.WriteLine($"Client {arg.ClientId} disconnected");
            return Task.FromResult(12);
        }

        private static Task MessageReceivedHandler(MqttApplicationMessageReceivedEventArgs arg)
        {
            Console.WriteLine($"Received message from client {arg.ClientId} with topic {arg.ApplicationMessage.Topic} : {Encoding.UTF8.GetString(arg.ApplicationMessage.Payload)}");
            return Task.FromResult(12);
        }

    }
}
