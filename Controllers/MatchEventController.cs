using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sports_api.DTOs;
using sports_api.Services;

namespace sports_api.Controllers;

[ApiController]
[Route("api/matches/{matchId}/events")]
[Authorize(Roles = "Player,Admin")]
public class MatchEventController(MatchEventService matchEventService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<MatchEventResponseDto>>> GetMatchEvents(Guid matchId)
    {
        var events = await matchEventService.GetMatchEventsAsync(matchId);

        var result = events.Select(e => new MatchEventResponseDto
        {
            Id = e.Id,
            MatchId = e.MatchId,
            TeamId = e.TeamId,
            PlayerId = e.PlayerId,
            Type = e.Type.ToString(),
            Points = e.Points,
            Minute = e.Minute
        }).ToList();

        return Ok(result);
    }

    [HttpGet("score")]
    public async Task<ActionResult<MatchScoreDto>> GetScore(Guid matchId)
    {
        var events = await matchEventService.GetMatchEventsAsync(matchId);
        if (!events.Any())
            return Ok(new MatchScoreDto { HomeScore = 0, AwayScore = 0 });
        return Ok();
    }

    [HttpPost]
    public async Task<ActionResult<MatchEventResponseDto>> CreateMatchEvent(
        Guid matchId, CreateMatchEventDto dto)
    {
        var (matchEvent, error) = await matchEventService.CreateMatchEventAsync(
            matchId, dto.TeamId, dto.PlayerId, dto.Type, dto.Points, dto.Minute);

        if (error != null)
            return BadRequest(error);

        return Ok(new MatchEventResponseDto
        {
            Id = matchEvent!.Id,
            MatchId = matchEvent.MatchId,
            TeamId = matchEvent.TeamId,
            PlayerId = matchEvent.PlayerId,
            Type = matchEvent.Type.ToString(),
            Points = matchEvent.Points,
            Minute = matchEvent.Minute
        });
    }
}