using System.Threading.Tasks;
using MQTTnet.Client.Publishing;

namespace autoCardboard.Messaging
{
    public interface IMessageSender
    {
        Task<MqttClientPublishResult> SendMessageASync(string topic, string payload);
    }
}
