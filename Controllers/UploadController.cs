using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sports_api.Services;

namespace sports_api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class UploadController(UploadService uploadService) : ControllerBase
{

    private async Task<ActionResult<string>> HandleUpload(IFormFile? file, string subfolder)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file provided");

        if (!file.ContentType.StartsWith("image/"))
            return BadRequest("File must be an image");

        if (file.Length > 5 * 1024 * 1024)
            return BadRequest("File size must be less than 5MB");

        using var stream = file.OpenReadStream();
        var url = await uploadService.UploadImageAsync(stream, file.FileName, subfolder);

        if (url == null)
            return StatusCode(500, "Failed to upload image");

        return Ok(new { url });
    }

    [HttpPost("avatar")]
    public async Task<ActionResult<string>> UploadAvatar(IFormFile file) => await HandleUpload(file, "avatars");

    [HttpPost("league-image")]
    public async Task<ActionResult<string>> UploadLeagueImage(IFormFile file) => await HandleUpload(file, "league-images");

    [HttpPost("team-logo")]
    public async Task<ActionResult<string>> UploadTeamLogo(IFormFile file) => await HandleUpload(file, "team-logos");

}