using sports_api.Models;

namespace sports_api.Interfaces;

public interface IChatMessageRepository
{
    Task<List<ChatMessage>> GetByConversationIdAsync(Guid conversationId);
    Task<ChatMessage> CreateAsync(ChatMessage message);
}