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

        public string TopicGameStart { get; set; }
        public string TopicBotSetup { get; set; }
        public string TopicGameEnd { get; set; }
        public string TopicInfect { get; set; }
        public string TopicTreat { get; set; }
        public string TopicCure { get; set; }
        public string TopicBotMoved{ get; set; }
        public string TopicBuilt { get; set; }
        public string TopicDiscardedInfection { get; set; }
        public string TopicBotDrew { get; set; }
        public string TopicBotDiscarded { get; set; }
        public string TopicBotThought { get; set; }
    }
}
