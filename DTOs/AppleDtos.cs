using System.ComponentModel.DataAnnotations;
namespace sports_api.DTOs;
public class AppleAuthDto
{
    [Required]
    public string IdToken { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? LastName { get; set; }
    public string? Platform { get; set; }
}