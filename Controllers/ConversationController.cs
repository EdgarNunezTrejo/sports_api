using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sports_api.DTOs;
using sports_api.Services;

namespace sports_api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ConversationController(ConversationService conversationService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ConversationResponseDto>> GetConversation(Guid id)
    {
        var conversation = await conversationService.GetConversationByIdAsync(id);

        if (conversation == null)
            return NotFound();

        return Ok(new ConversationResponseDto
        {
            Id = conversation.Id,
            TeamAId = conversation.TeamAId,
            TeamBId = conversation.TeamBId
        });
    }

    [HttpPost]
    public async Task<ActionResult<ConversationResponseDto>> CreateConversation(
        CreateConversationDto dto)
    {
        var (conversation, error) = await conversationService
            .CreateConversationAsync(dto.TeamAId, dto.TeamBId);

        if (error != null)
            return BadRequest(error);

        return CreatedAtAction(nameof(GetConversation), new { id = conversation!.Id },
            new ConversationResponseDto
            {
                Id = conversation.Id,
                TeamAId = conversation.TeamAId,
                TeamBId = conversation.TeamBId
            });
    }

    [HttpGet("{id:guid}/messages")]
    public async Task<ActionResult<List<ChatMessageResponseDto>>> GetMessages(Guid id)
    {
        var messages = await conversationService.GetMessagesAsync(id);

        var result = messages.Select(m => new ChatMessageResponseDto
        {
            Id = m.Id,
            ConversationId = m.ConversationId,
            SenderPlayerId = m.SenderPlayerId,
            Content = m.Content,
            SentAt = m.SentAt
        }).ToList();

        return Ok(result);
    }

    [HttpPost("{id:guid}/messages")]
    public async Task<ActionResult<ChatMessageResponseDto>> SendMessage(
        Guid id, SendMessageDto dto)
    {
        var (message, error) = await conversationService
            .SendMessageAsync(id, dto.SenderPlayerId, dto.Content);

        if (error != null)
            return BadRequest(error);

        return Ok(new ChatMessageResponseDto
        {
            Id = message!.Id,
            ConversationId = message.ConversationId,
            SenderPlayerId = message.SenderPlayerId,
            Content = message.Content,
            SentAt = message.SentAt
        });
    }
}