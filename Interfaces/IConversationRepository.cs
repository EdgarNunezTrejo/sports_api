using sports_api.Models;

namespace sports_api.Interfaces;

public interface IConversationRepository
{
    Task<Conversation?> GetByIdAsync(Guid id);
    Task<Conversation?> GetByTeamsAsync(Guid teamAId, Guid teamBId);
    Task<bool> ExistsAsync(Guid teamAId, Guid teamBId);
    Task<Conversation> CreateAsync(Conversation conversation);
}