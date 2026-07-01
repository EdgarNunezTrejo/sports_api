using Microsoft.AspNetCore.Mvc;
using sports_api.DTOs;
using sports_api.Services;

namespace sports_api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PositionController(PositionService positionService) : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult<List<PositionResponseDto>>> GetPositions()
    {
        var positions = await positionService.GetAllPositionsAsync();

        var result = positions.Select(p => new PositionResponseDto
        {
            Id = p.Id,
            Name = p.Name,
            SportId = p.SportId
        }).ToList();

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PositionResponseDto>> GetPosition(Guid id)
    {
        var position = await positionService.GetPositionByIdAsync(id);

        if (position == null)
            return NotFound();

        return Ok(new PositionResponseDto { Id = position.Id, Name = position.Name, SportId = position.SportId });
    }

    [HttpPost]
    public async Task<ActionResult<PositionResponseDto>> CreatePosition(CreatePositionDto dto)
    {
        var (position, error) = await positionService.CreatePositionAsync(dto.Name, dto.SportId);

        if (error != null)
            return BadRequest(error);

        return CreatedAtAction(nameof(GetPosition), new { id = position!.Id },
            new PositionResponseDto { Id = position.Id, Name = position.Name, SportId = position.SportId });
    }
}