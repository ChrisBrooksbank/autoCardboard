namespace autoCardboard.Messaging
{
    public class MessageSenderConfiguration
    {
        public bool Enabled { get; set; }
        public string Uri { get; set; }
        public int PortNumber { get; set; }
        public string ClientId { get; set; }
        public bool Secure { get; set; }
        public int AutoConnectDelaySeconds { get; set; }
        public string TopicStateDelta { get; set; }
    }
}
