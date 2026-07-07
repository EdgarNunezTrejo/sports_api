using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sports_api.DTOs;
using sports_api.Services;

namespace sports_api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "Player,Admin")]
public class PlayerController(PlayerService playerService) : ControllerBase
{

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PlayerResponseDto>> GetPlayer(Guid id)
    {
        var player = await playerService.GetPlayerByIdAsync(id);

        if (player == null)
            return NotFound();

        return Ok(new PlayerResponseDto
        {
            Id = player.Id,
            Name = player.Name,
            IsLeader = player.IsLeader,
            TeamId = player.TeamId,
            PositionId = player.PositionId
        });
    }

    [HttpPost]
    public async Task<ActionResult<PlayerResponseDto>> CreatePlayer(CreatePlayerDto dto)
    {
        var (player, error) = await playerService.CreatePlayerAsync(
            dto.Name, dto.IsLeader, dto.TeamId, dto.PositionId);

        if (error != null)
            return BadRequest(error);

        return CreatedAtAction(nameof(GetPlayer), new { id = player!.Id }, new PlayerResponseDto
        {
            Id = player.Id,
            Name = player.Name,
            IsLeader = player.IsLeader,
            TeamId = player.TeamId,
            PositionId = player.PositionId
        });
    }
}