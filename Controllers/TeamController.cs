using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sports_api.DTOs;
using sports_api.Services;

namespace sports_api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TeamController(TeamService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<TeamResponseDto>>> GetTeams()
    {
        var teams = await service.GetAllTeamsAsync();
        var result = teams.Select(
            t => new TeamResponseDto
            {
                Id = t.Id,
                Name = t.Name,
                InviteCode = t.InviteCode,
                LeagueId = t.LeagueId
            }
        ).ToList();
        return Ok(teams);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TeamResponseDto>> GetTeam(Guid id)
    {
        var team = await service.GetTeamByIdAsync(id);

        if (team == null)
        {
            return NotFound();
        }

        return Ok(new TeamResponseDto
        {
            Id = team.Id,
            Name = team.Name,
            InviteCode = team.InviteCode,
            LeagueId = team.LeagueId
        });
    }

    [HttpPost]
    public async Task<ActionResult<TeamResponseDto>> CreateTeam(CreateTeamDto dto)
    {
        var team = await service.CreateTeamAsync(dto.Name, dto.LeagueId);

        if (team == null)
        {
            return BadRequest("League does not exists.");
        }

        return CreatedAtAction(nameof(GetTeam), new { id = team.Id }, new TeamResponseDto
        {
            Id = team.Id,
            Name = team.Name,
            InviteCode = team.InviteCode,
            LeagueId = team.LeagueId
        });
    }
}
