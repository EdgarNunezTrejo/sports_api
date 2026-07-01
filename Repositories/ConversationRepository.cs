using Microsoft.EntityFrameworkCore;
using sports_api.Data;
using sports_api.Models;

namespace sports_api.Repositories;

public class ConversationRepository(AppDbContext context)
{
    public async Task<Conversation?> GetByIdAsync(Guid id)
    {
        return await context.Conversations.FindAsync(id);
    }

    public async Task<Conversation?> GetByTeamsAsync(Guid teamAId, Guid teamBId)
    {
        return await context.Conversations.FirstOrDefaultAsync(c =>
            (c.TeamAId == teamAId && c.TeamBId == teamBId) ||
            (c.TeamAId == teamBId && c.TeamBId == teamAId));
    }

    public async Task<bool> ExistsAsync(Guid teamAId, Guid teamBId)
    {
        return await context.Conversations.AnyAsync(c =>
            (c.TeamAId == teamAId && c.TeamBId == teamBId) ||
            (c.TeamAId == teamBId && c.TeamBId == teamAId));
    }

    public async Task<Conversation> CreateAsync(Conversation conversation)
    {
        context.Conversations.Add(conversation);
        await context.SaveChangesAsync();
        return conversation;
    }
}