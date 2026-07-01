namespace sports_api.Models;

public class Conversation
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid TeamAId { get; set; }
    public Team? TeamA { get; set; }

    public Guid TeamBId { get; set; }
    public Team? TeamB { get; set; }

    public List<ChatMessage> Messages { get; set; } = new();
}