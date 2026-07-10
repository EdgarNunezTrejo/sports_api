using Moq;
using sports_api.Interfaces;
using sports_api.Models;
using sports_api.Services;
using Xunit;

namespace Tests;

public class PlayerServiceTests
{
    private readonly Mock<IPlayerRepository> _playerRepoMock;
    private readonly Mock<ITeamRepository> _teamRepoMock;
    private readonly Mock<IPositionRepository> _positionRepoMock;
    private readonly PlayerService _sut;

    public PlayerServiceTests()
    {
        _playerRepoMock = new Mock<IPlayerRepository>();
        _teamRepoMock = new Mock<ITeamRepository>();
        _positionRepoMock = new Mock<IPositionRepository>();

        _sut = new PlayerService(
            _playerRepoMock.Object,
            _positionRepoMock.Object,
            _teamRepoMock.Object);
    }

    [Fact]
    public async Task GetAllPlayersAsync_ReturnsAllPlayers()
    {
        var players = new List<Player> { new() { Name = "Edgar" }, new() { Name = "Luis" } };
        _playerRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(players);

        var result = await _sut.GetAllPlayersAsync();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetPlayerByIdAsync_WhenExists_ReturnsPlayer()
    {
        var id = Guid.NewGuid();
        var player = new Player { Id = id, Name = "Edgar" };
        _playerRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(player);

        var result = await _sut.GetPlayerByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal("Edgar", result.Name);
    }

    [Fact]
    public async Task GetPlayerByIdAsync_WhenNotFound_ReturnsNull()
    {
        _playerRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Player?)null);

        var result = await _sut.GetPlayerByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task CreatePlayerAsync_WhenTeamDoesNotExist_ReturnsError()
    {
        // Arrange
        _teamRepoMock
            .Setup(r => r.GetSportIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Guid?)null);

        // Act
        var (player, error) = await _sut.CreatePlayerAsync("Edgar", false, Guid.NewGuid(), null);

        // Assert
        Assert.Null(player);
        Assert.Equal("The specified team does not exist", error);
    }

    [Fact]
    public async Task CreatePlayerAsync_WhenPositionDoesNotExist_ReturnsError()
    {
        // Arrange
        _teamRepoMock
            .Setup(r => r.GetSportIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Guid.NewGuid());

        _positionRepoMock
            .Setup(r => r.GetSportIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Guid?)null);

        // Act
        var (player, error) = await _sut.CreatePlayerAsync(
            "Edgar", false, Guid.NewGuid(), Guid.NewGuid());

        // Assert
        Assert.Null(player);
        Assert.Equal("The specified position does not exist", error);
    }

    [Fact]
    public async Task CreatePlayerAsync_WhenPositionBelongsToDifferentSport_ReturnsError()
    {
        // Arrange
        var teamSportId = Guid.NewGuid();
        var differentSportId = Guid.NewGuid();

        _teamRepoMock
            .Setup(r => r.GetSportIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(teamSportId);

        _positionRepoMock
            .Setup(r => r.GetSportIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(differentSportId);

        // Act
        var (player, error) = await _sut.CreatePlayerAsync(
            "Edgar", false, Guid.NewGuid(), Guid.NewGuid());

        // Assert
        Assert.Null(player);
        Assert.Equal("The position does not belong to the sport of the team's league", error);
    }

    [Fact]
    public async Task CreatePlayerAsync_WhenValidData_ReturnsPlayer()
    {
        // Arrange
        var sportId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var positionId = Guid.NewGuid();

        _teamRepoMock
            .Setup(r => r.GetSportIdAsync(teamId))
            .ReturnsAsync(sportId);

        _positionRepoMock
            .Setup(r => r.GetSportIdAsync(positionId))
            .ReturnsAsync(sportId); // same sport - valid

        var expectedPlayer = new Player
        {
            Name = "Edgar",
            IsLeader = false,
            TeamId = teamId,
            PositionId = positionId
        };

        _playerRepoMock
            .Setup(r => r.CreateAsync(It.IsAny<Player>()))
            .ReturnsAsync(expectedPlayer);

        // Act
        var (player, error) = await _sut.CreatePlayerAsync("Edgar", false, teamId, positionId);

        // Assert
        Assert.NotNull(player);
        Assert.Null(error);
        Assert.Equal("Edgar", player.Name);
        Assert.Equal(teamId, player.TeamId);
    }

    [Fact]
    public async Task CreatePlayerAsync_WhenValidData_SetsStatusToActive()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var sportId = Guid.NewGuid();

        _teamRepoMock
            .Setup(r => r.GetSportIdAsync(teamId))
            .ReturnsAsync(sportId);

        Player? capturedPlayer = null;
        _playerRepoMock
            .Setup(r => r.CreateAsync(It.IsAny<Player>()))
            .Callback<Player>(p => capturedPlayer = p)
            .ReturnsAsync(() => capturedPlayer!);

        // Act
        var (player, error) = await _sut.CreatePlayerAsync("Edgar", false, teamId, null);

        // Assert
        Assert.Null(error);
        Assert.NotNull(player);
        Assert.Equal(PlayerStatus.Active, player.Status);
        Assert.Equal(PlayerStatus.Active, capturedPlayer?.Status);
    }
}