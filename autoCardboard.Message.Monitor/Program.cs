using System;
using System.Text;
using System.Threading.Tasks;
using autoCardboard.Messaging;
using MQTTnet;
using MQTTnet.Extensions.ManagedClient;

namespace autoCardboard.Message.Monitor
{
    class Program
    {
        static void Main(string[] args)
        {
            var messenger = new GameMessenger();

            messenger.Client.UseConnectedHandler(async e =>
            {
                Console.WriteLine("### CONNECTED WITH SERVER ###");

                // Subscribe to a topic
                await  messenger.Client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("AutoCardboard").Build());

                Console.WriteLine("### SUBSCRIBED ###");
            });

            messenger.Client.UseApplicationMessageReceivedHandler(e =>
            {
                Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
                Console.WriteLine();
            });

            Console.ReadLine();
        }
    }
}
