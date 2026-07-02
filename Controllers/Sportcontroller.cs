using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sports_api.DTOs;
using sports_api.Services;

namespace sports_api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class SportController(SportService sportService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<SportResponseDto>>> GetSports()
    {
        var sports = await sportService.GetAllSportsAsync();

        var result = sports.Select(s => new SportResponseDto
        {
            Id = s.Id,
            Name = s.Name
        }).ToList();

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SportResponseDto>> GetSport(Guid id)
    {
        var sport = await sportService.GetSportByIdAsync(id);

        if (sport == null)
            return NotFound();

        return Ok(new SportResponseDto { Id = sport.Id, Name = sport.Name });
    }

    [HttpPost]
    public async Task<ActionResult<SportResponseDto>> CreateSport(CreateSportDto dto)
    {
        var sport = await sportService.CreateSportAsync(dto.Name);

        return CreatedAtAction(nameof(GetSport), new { id = sport.Id },
            new SportResponseDto { Id = sport.Id, Name = sport.Name });
    }
}