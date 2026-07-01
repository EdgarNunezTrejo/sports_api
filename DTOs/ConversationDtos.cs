namespace sports_api.DTOs;

public class ConversationResponseDto
{
    public Guid Id { get; set; }
    public Guid TeamAId { get; set; }
    public Guid TeamBId { get; set; }
}

public class CreateConversationDto
{
    public Guid TeamAId { get; set; }
    public Guid TeamBId { get; set; }
}

public class ChatMessageResponseDto
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public Guid SenderPlayerId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
}

public class SendMessageDto
{
    public Guid SenderPlayerId { get; set; }
    public string Content { get; set; } = string.Empty;
}