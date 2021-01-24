namespace autoCardboard.Messaging
{
    public interface IMessageSender
    {
        void SendMessageASync(string topic, string payload);
    }
}
