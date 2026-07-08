namespace sports_api.Models;

public enum AuthProvider
{
    Email,
    Google,
    Apple
}

public enum UserRole
{
    Player,
    Admin
}

public class User
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Email { get; set; } = string.Empty;
    public string? PasswordHash { get; set; }
    public AuthProvider Provider { get; set; } = AuthProvider.Email;
    public string? ProviderId { get; set; }
    public string? Platform { get; set; }
    public UserRole Role { get; set; } = UserRole.Player;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Player> Players { get; set; } = [];

    public string AvatarUrl { get; set; } = string.Empty;
}