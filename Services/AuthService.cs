using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using sports_api.DTOs;
using sports_api.Interfaces;
using sports_api.Models;

namespace sports_api.Services;

public class AuthService(
    IUserRepository userRepository,
    ITeamRepository teamRepository,
    IConfiguration configuration)
{
    public async Task<(AuthResponseDto? Response, string? Error)> RegisterAsync(RegisterDto dto)
    {
        // Verify that the email is not already registered
        var exists = await userRepository.ExistsAsync(dto.Email);
        if (exists)
            return (null, "Email is already registered");

        // If InviteCode is provided, verify that a team exists with that code
        if (!string.IsNullOrEmpty(dto.InviteCode))
        {
            var team = await teamRepository.GetByInviteCodeAsync(dto.InviteCode);
            if (team == null)
                return (null, "The invite code is not valid");
        }

        var user = new User
        {
            Email = dto.Email.ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Provider = AuthProvider.Email,
            Platform = dto.Platform,
            Role = UserRole.Player
        };

        var created = await userRepository.CreateAsync(user);
        var token = GenerateToken(created);

        return (token, null);
    }

    public async Task<(AuthResponseDto? Response, string? Error)> LoginAsync(LoginDto dto)
    {
        var user = await userRepository.GetByEmailAsync(dto.Email);
        if (user == null)
            return (null, "Invalid credentials");

        // Registered User with Google/Apple — has not password
        if (user.Provider != AuthProvider.Email || user.PasswordHash == null)
            return (null, "This account uses Google or Apple to log in");
        var passwordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        if (!passwordValid)
            return (null, "Invalid credentials");

        var token = GenerateToken(user);
        return (token, null);
    }

    private AuthResponseDto GenerateToken(User user)
    {
        var secretKey = configuration["JwtSettings:SecretKey"]!;
        var issuer = configuration["JwtSettings:Issuer"]!;
        var audience = configuration["JwtSettings:Audience"]!;
        var expirationHours = int.Parse(configuration["JwtSettings:ExpirationHours"]!);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddHours(expirationHours);

        // Claims — data inside JWT token
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiration,
            signingCredentials: credentials);

        return new AuthResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Email = user.Email,
            Role = user.Role.ToString(),
            ExpiresAt = expiration
        };
    }
}