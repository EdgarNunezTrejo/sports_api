using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sports_api.DTOs;
using sports_api.Services;

namespace sports_api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Player,Admin")]
public class MatchController(MatchService matchService, MatchEventService matchEventService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<MatchResponseDto>>> GetMatches()
    {
        var matches = await matchService.GetAllMatchesAsync();

        var result = matches.Select(m => new MatchResponseDto
        {
            Id = m.Id,
            LeagueId = m.LeagueId,
            HomeTeamId = m.HomeTeamId,
            AwayTeamId = m.AwayTeamId,
            ScheduledDate = m.ScheduledDate,
            Status = m.Status.ToString()
        }).ToList();

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MatchResponseDto>> GetMatch(Guid id)
    {
        var match = await matchService.GetMatchByIdAsync(id);

        if (match == null)
            return NotFound();

        return Ok(new MatchResponseDto
        {
            Id = match.Id,
            LeagueId = match.LeagueId,
            HomeTeamId = match.HomeTeamId,
            AwayTeamId = match.AwayTeamId,
            ScheduledDate = match.ScheduledDate,
            Status = match.Status.ToString()
        });
    }

    [HttpPost]
    public async Task<ActionResult<MatchResponseDto>> CreateMatch(CreateMatchDto dto)
    {
        var (match, error) = await matchService.CreateMatchAsync(
            dto.LeagueId, dto.HomeTeamId, dto.AwayTeamId, dto.ScheduledDate);

        if (error != null)
            return BadRequest(error);

        return CreatedAtAction(nameof(GetMatch), new { id = match!.Id }, new MatchResponseDto
        {
            Id = match.Id,
            LeagueId = match.LeagueId,
            HomeTeamId = match.HomeTeamId,
            AwayTeamId = match.AwayTeamId,
            ScheduledDate = match.ScheduledDate,
            Status = match.Status.ToString()
        });
    }

    [HttpGet("{id:guid}/score")]
    public async Task<ActionResult<MatchScoreDto>> GetScore(Guid id)
    {
        var match = await matchService.GetMatchByIdAsync(id);
        if (match == null)
            return NotFound();

        var (homeScore, awayScore) = await matchEventService.GetScoreAsync(
            id, match.HomeTeamId, match.AwayTeamId);

        return Ok(new MatchScoreDto { HomeScore = homeScore, AwayScore = awayScore });
    }
}