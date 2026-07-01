namespace sports_api.Models;
public class ChatMessage
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

    public Guid ConversationId { get; set; }
    public Conversation? Conversation { get; set; }

    public Guid SenderPlayerId { get; set; }
    public Player? SenderPlayer { get; set; }

    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}