using Microsoft.EntityFrameworkCore;
using sports_api.Data;
using sports_api.Models;

namespace sports_api.Repositories;

public class ChatMessageRepository(AppDbContext context)
{
    public async Task<List<ChatMessage>> GetByConversationIdAsync(Guid conversationId)
    {
        return await context.ChatMessages
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.SentAt)
            .ToListAsync();
    }

    public async Task<ChatMessage> CreateAsync(ChatMessage message)
    {
        context.ChatMessages.Add(message);
        await context.SaveChangesAsync();
        return message;
    }
}