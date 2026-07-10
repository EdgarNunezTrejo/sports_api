using Microsoft.Extensions.Configuration;
using Moq;
using sports_api.DTOs;
using sports_api.Interfaces;
using sports_api.Models;
using sports_api.Services;
using Xunit;

namespace Tests;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<ITeamRepository> _teamRepoMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _teamRepoMock = new Mock<ITeamRepository>();
        _configMock = new Mock<IConfiguration>();

        _configMock.Setup(c => c["JwtSettings:SecretKey"]).Returns("this-is-a-test-secret-key-with-enough-length");
        _configMock.Setup(c => c["JwtSettings:Issuer"]).Returns("test-issuer");
        _configMock.Setup(c => c["JwtSettings:Audience"]).Returns("test-audience");
        _configMock.Setup(c => c["JwtSettings:ExpirationHours"]).Returns("1");

        _sut = new AuthService(_userRepoMock.Object, _teamRepoMock.Object, _configMock.Object);
    }

    private static RegisterDto ValidRegisterDto() => new()
    {
        Email = "Edgar@Example.com",
        Name = "Edgar",
        LastName = "Nunez",
        Password = "Valid1@Password",
        AcceptsTermsAndConditions = true
    };

    [Fact]
    public async Task RegisterAsync_WhenTermsNotAccepted_ReturnsError()
    {
        // Arrange
        var dto = ValidRegisterDto();
        dto.AcceptsTermsAndConditions = false;

        // Act
        var (response, error) = await _sut.RegisterAsync(dto);

        // Assert
        Assert.Null(response);
        Assert.Equal("User must accept Terms and Conditions", error);
        _userRepoMock.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailAlreadyRegistered_ReturnsError()
    {
        // Arrange
        var dto = ValidRegisterDto();
        _userRepoMock.Setup(r => r.ExistsAsync(dto.Email)).ReturnsAsync(true);

        // Act
        var (response, error) = await _sut.RegisterAsync(dto);

        // Assert
        Assert.Null(response);
        Assert.Equal("Email is already registered", error);
    }

    [Fact]
    public async Task RegisterAsync_WhenInviteCodeIsInvalid_ReturnsError()
    {
        // Arrange
        var dto = ValidRegisterDto();
        dto.InviteCode = "BAD-CODE";
        _userRepoMock.Setup(r => r.ExistsAsync(dto.Email)).ReturnsAsync(false);
        _teamRepoMock.Setup(r => r.GetByInviteCodeAsync(dto.InviteCode)).ReturnsAsync((Team?)null);

        // Act
        var (response, error) = await _sut.RegisterAsync(dto);

        // Assert
        Assert.Null(response);
        Assert.Equal("The invite code is not valid", error);
    }

    [Fact]
    public async Task RegisterAsync_WhenValidData_CreatesUserWithNameLastNameAndTerms()
    {
        // Arrange
        var dto = ValidRegisterDto();
        _userRepoMock.Setup(r => r.ExistsAsync(dto.Email)).ReturnsAsync(false);

        User? capturedUser = null;
        _userRepoMock
            .Setup(r => r.CreateAsync(It.IsAny<User>()))
            .Callback<User>(u => capturedUser = u)
            .ReturnsAsync(() => capturedUser!);

        // Act
        var (response, error) = await _sut.RegisterAsync(dto);

        // Assert
        Assert.Null(error);
        Assert.NotNull(response);
        Assert.NotNull(capturedUser);
        Assert.Equal("edgar@example.com", capturedUser!.Email);
        Assert.Equal("Edgar", capturedUser.Name);
        Assert.Equal("Nunez", capturedUser.LastName);
        Assert.True(capturedUser.AcceptsTermsAndConditions);
        Assert.NotEmpty(response!.Token);
    }

    [Fact]
    public async Task LoginAsync_WhenUserNotFound_ReturnsError()
    {
        // Arrange
        _userRepoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        // Act
        var (response, error) = await _sut.LoginAsync(new LoginDto { Email = "missing@example.com", Password = "x" });

        // Assert
        Assert.Null(response);
        Assert.Equal("Invalid credentials", error);
    }

    [Fact]
    public async Task LoginAsync_WhenUserRegisteredWithOAuthProvider_ReturnsError()
    {
        // Arrange
        var user = new User { Email = "edgar@example.com", Provider = AuthProvider.Google, PasswordHash = null };
        _userRepoMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

        // Act
        var (response, error) = await _sut.LoginAsync(new LoginDto { Email = user.Email, Password = "x" });

        // Assert
        Assert.Null(response);
        Assert.Equal("This account uses Google or Apple to log in", error);
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordIsInvalid_ReturnsError()
    {
        // Arrange
        var user = new User
        {
            Email = "edgar@example.com",
            Provider = AuthProvider.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword1@")
        };
        _userRepoMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

        // Act
        var (response, error) = await _sut.LoginAsync(new LoginDto { Email = user.Email, Password = "WrongPassword" });

        // Assert
        Assert.Null(response);
        Assert.Equal("Invalid credentials", error);
    }

    [Fact]
    public async Task LoginAsync_WhenCredentialsAreValid_ReturnsToken()
    {
        // Arrange
        var user = new User
        {
            Email = "edgar@example.com",
            Role = UserRole.Player,
            Provider = AuthProvider.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword1@")
        };
        _userRepoMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

        // Act
        var (response, error) = await _sut.LoginAsync(new LoginDto { Email = user.Email, Password = "CorrectPassword1@" });

        // Assert
        Assert.Null(error);
        Assert.NotNull(response);
        Assert.Equal(user.Email, response!.Email);
        Assert.Equal("Player", response.Role);
        Assert.NotEmpty(response.Token);
    }
}
