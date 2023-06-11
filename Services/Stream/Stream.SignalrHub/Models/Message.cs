namespace Stream.SignalrHub.Models
{
    public class Message
    {
        public MessageType Type { get; }
        public string UserName { get; set; }
        public string Content { get; }
        public int? Amount { get; }
        public bool IsHost { get; } = false;

        public Message(MessageType type, string userName, string content, int? amount, bool isHost = false)
        {
            Type = type;
            UserName = userName;
            Content = content;
            Amount = amount;
            IsHost = isHost;
        }
    }
}