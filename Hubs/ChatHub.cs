using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using sports_api.DTOs;
using sports_api.Interfaces;
using sports_api.Models;
using System.Security.Claims;

namespace sports_api.Hubs;

[Authorize(Roles = "Player,Admin")]
public class ChatHub(
    IConversationRepository conversationRepository,
    IChatMessageRepository chatMessageRepository,
    IPlayerRepository playerRepository) : Hub
{
    // Client calls this method to join a conversation
    public async Task JoinConversation(Guid conversationId)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            await Clients.Caller.SendAsync("Error", "No autenticado");
            return;
        }

        // Assure the conversation exists
        var conversation = await conversationRepository.GetByIdAsync(conversationId);
        if (conversation == null)
        {
            await Clients.Caller.SendAsync("Error", "Conversación no encontrada");
            return;
        }

        // Assure the user is a leader of one of the two teams
        var isLeaderOfTeamA = await playerRepository.IsLeaderOfTeamAsync(userId.Value, conversation.TeamAId);
        var isLeaderOfTeamB = await playerRepository.IsLeaderOfTeamAsync(userId.Value, conversation.TeamBId);

        if (!isLeaderOfTeamA && !isLeaderOfTeamB)
        {
            await Clients.Caller.SendAsync("Error", "No tienes permiso para unirte a esta conversación");
            return;
        }

        // Join the SignalR group for this conversation
        var groupName = $"conversation-{conversationId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        // Send history of messages to the connecting client
        var messages = await chatMessageRepository.GetByConversationIdAsync(conversationId);
        var history = messages.Select(m => new ChatMessageResponseDto
        {
            Id = m.Id,
            ConversationId = m.ConversationId,
            SenderPlayerId = m.SenderPlayerId,
            Content = m.Content,
            SentAt = m.SentAt
        }).ToList();

        await Clients.Caller.SendAsync("ReceiveHistory", history);
    }

    // Client calls this method to send a message
    public async Task SendMessage(Guid conversationId, Guid senderPlayerId, string content)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            await Clients.Caller.SendAsync("Error", "No autenticado");
            return;
        }

        // Verify that the player belongs to the authenticated user
        var player = await playerRepository.GetByIdAsync(senderPlayerId);
        if (player == null || player.UserId != userId)
        {
            await Clients.Caller.SendAsync("Error", "Jugador no válido");
            return;
        }

        // Verify that the player is a leader
        if (!player.IsLeader)
        {
            await Clients.Caller.SendAsync("Error", "Solo los líderes pueden enviar mensajes");
            return;
        }

        // Verify that the player belongs to one of the teams in the conversation
        var conversation = await conversationRepository.GetByIdAsync(conversationId);
        if (conversation == null)
        {
            await Clients.Caller.SendAsync("Error", "Conversación no encontrada");
            return;
        }

        if (player.TeamId != conversation.TeamAId && player.TeamId != conversation.TeamBId)
        {
            await Clients.Caller.SendAsync("Error", "No perteneces a esta conversación");
            return;
        }

        // Save to DB
        var message = new ChatMessage
        {
            ConversationId = conversationId,
            SenderPlayerId = senderPlayerId,
            Content = content,
            SentAt = DateTime.UtcNow
        };

        await chatMessageRepository.CreateAsync(message);

        // Push to all in the group
        var groupName = $"conversation-{conversationId}";
        await Clients.Group(groupName).SendAsync("ReceiveMessage", new ChatMessageResponseDto
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            SenderPlayerId = message.SenderPlayerId,
            Content = message.Content,
            SentAt = message.SentAt
        });
    }

    private Guid? GetUserId()
    {
        var claim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)
                 ?? Context.User?.FindFirst("sub");

        return claim != null && Guid.TryParse(claim.Value, out var id) ? id : null;
    }
}