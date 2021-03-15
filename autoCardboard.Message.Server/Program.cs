using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MQTTnet;
using MQTTnet.Channel;
using MQTTnet.Client.Options;
using MQTTnet.Implementations;
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
        private static string[] _topicsFilter;

        static void Main(string[] args)
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            StartServer();
        }

        // TODO workout how to allow GamesRoom to connect to the broker we are hosting here via WebSockets
        // See : https://github.com/chkr1011/MQTTnet/issues/65
        // see MqttWebSocketChannel
        // See IMqttServerAdapter
        static async void StartServer()
        {
            var title = $"Mqtt broker listening on port {_configuration["portNumber"]} - press enter to exit.";

            IMqttChannel webSocketChannel = new MqttWebSocketChannel( new MqttClientWebSocketOptions{});

            var optionsBuilder = new MqttServerOptionsBuilder()
                .WithConnectionBacklog(int.Parse(_configuration["backlog"]))
                .WithDefaultEndpointPort(int.Parse(_configuration["portNumber"]));
            var options = optionsBuilder.Build();


            var mqttServer = new MqttFactory().CreateMqttServer();

            var isEchoOn = bool.Parse(_configuration["echo"]);
            _topicsFilter = _configuration["echoTopicFilters"].Split(",");

            Console.WriteLine(title);

            if (isEchoOn)
            {
                Console.WriteLine($"echoing messages using topicFilter of {_configuration["echoTopicFilters"]}");
                mqttServer.UseClientConnectedHandler(ClientConnectedHandler);
                mqttServer.UseClientDisconnectedHandler(ClientDisconnectedHandler);
                mqttServer.UseApplicationMessageReceivedHandler(MessageReceivedHandler);
            }
          
            await mqttServer.StartAsync(options);

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
            if (_topicsFilter.Any(f => arg.ApplicationMessage.Topic.Contains(f)))
            {
                Console.WriteLine($"Received message from client {arg.ClientId} with topic {arg.ApplicationMessage.Topic} : {Encoding.UTF8.GetString(arg.ApplicationMessage.Payload)}");
            }
            return Task.FromResult(12);
        }

    }


}
