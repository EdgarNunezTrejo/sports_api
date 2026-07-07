using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sports_api.DTOs;
using sports_api.Models;
using sports_api.Services;

namespace sports_api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "Admin")]
public class LeagueController(LeagueService leagueService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<LeagueDto>>> GetLeagues()
    {
        var leagues = await leagueService.GetAllAsync();
        return Ok(leagues);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LeagueDto>> GetLeague(Guid id)
    {
        var league = await leagueService.GetByIdAsync(id);
        if (league == null)
        {
            return NotFound();
        }
        return Ok(league);
    }
}