using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sports_api.DTOs;
using sports_api.Services;

namespace sports_api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[AllowAnonymous]
public class AuthController(AuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
    {
        var (response, error) = await authService.RegisterAsync(dto);

        if (error != null)
            return BadRequest(error);

        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var (response, error) = await authService.LoginAsync(dto);

        if (error != null)
            return Unauthorized(error);

        return Ok(response);
    }

    [HttpPost("google")]
    public async Task<ActionResult<AuthResponseDto>> GoogleAuth(GoogleAuthDto dto)
    {
        var (response, error) = await authService.GoogleAuthAsync(dto);

        if (error != null)
            return BadRequest(error);

        return Ok(response);
    }

    [HttpPost("apple")]
    public async Task<ActionResult<AuthResponseDto>> AppleAuth(AppleAuthDto dto)
    {
        var (response, error) = await authService.AppleAuthAsync(dto);

        if(error != null)
            return BadRequest(error);

        return Ok(response);
    }
}