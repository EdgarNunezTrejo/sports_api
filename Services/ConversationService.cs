using sports_api.Models;
using sports_api.Interfaces;
namespace sports_api.Services;

public class ConversationService(
    IConversationRepository conversationRepository,
    IChatMessageRepository chatMessageRepository,
    ITeamRepository teamRepository,
    IPlayerRepository playerRepository)
{
    public async Task<Conversation?> GetConversationByIdAsync(Guid id)
    {
        return await conversationRepository.GetByIdAsync(id);
    }

    public async Task<Conversation?> GetConversationByTeamsAsync(Guid teamAId, Guid teamBId)
    {
        return await conversationRepository.GetByTeamsAsync(teamAId, teamBId);
    }

    public async Task<(Conversation? Conversation, string? Error)> CreateConversationAsync(
        Guid teamAId, Guid teamBId)
    {
        if (teamAId == teamBId)
            return (null, "A team cannot have a conversation with itself");

        var teamALeagueId = await teamRepository.GetLeagueIdAsync(teamAId);
        if (teamALeagueId == null)
            return (null, "Team A does not exist");
        var teamBLeagueId = await teamRepository.GetLeagueIdAsync(teamBId);
        if (teamBLeagueId == null)
            return (null, "Team B does not exist");

        var alreadyExists = await conversationRepository.ExistsAsync(teamAId, teamBId);
        if (alreadyExists)
            return (null, "A conversation already exists between these two teams");
        var conversation = new Conversation { TeamAId = teamAId, TeamBId = teamBId };
        var created = await conversationRepository.CreateAsync(conversation);
        return (created, null);
    }

    public async Task<List<ChatMessage>> GetMessagesAsync(Guid conversationId)
    {
        return await chatMessageRepository.GetByConversationIdAsync(conversationId);
    }

    public async Task<(ChatMessage? Message, string? Error)> SendMessageAsync(
        Guid conversationId, Guid senderPlayerId, string content)
    {
        var conversation = await conversationRepository.GetByIdAsync(conversationId);
        if (conversation == null)
            return (null, "The conversation does not exist");

        var isLeader = await playerRepository.IsLeaderAsync(senderPlayerId);
        if (!isLeader)
            return (null, "Only team leaders can send messages");

        var belongsToTeamA = await playerRepository.BelongsToTeamAsync(senderPlayerId, conversation.TeamAId);
        var belongsToTeamB = await playerRepository.BelongsToTeamAsync(senderPlayerId, conversation.TeamBId);

        if (!belongsToTeamA && !belongsToTeamB)
            return (null, "Player does not belong to either team in the conversation");

        var message = new ChatMessage
        {
            ConversationId = conversationId,
            SenderPlayerId = senderPlayerId,
            Content = content,
            SentAt = DateTime.UtcNow
        };

        var created = await chatMessageRepository.CreateAsync(message);
        return (created, null);
    }
}