using System.Text;
using autoCardboard.Common.Hubs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using autoCardboard.Messaging;
using MQTTnet;
using MQTTnet.Extensions.ManagedClient;

namespace autoCardboard.GameWatcher
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // TODO refactor
            // listen for MQTT messages and send onwards as local SignalR messages ( no CORS issues )
            var messenger = new GameMessenger();
            var gameHub = new GameHub();

            messenger.Client.UseConnectedHandler(async e =>
            {
                await  messenger.Client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("AutoCardboard").Build());
            });

            messenger.Client.UseApplicationMessageReceivedHandler(e =>
            {
                var topic = e.ApplicationMessage.Topic;
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                var sendResult = gameHub.SendStatusMessage("test game status message");
            });

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
