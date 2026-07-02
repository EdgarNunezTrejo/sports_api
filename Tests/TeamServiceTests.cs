using Moq;
using sports_api.Interfaces;
using sports_api.Models;
using sports_api.Services;
using Xunit;

namespace Tests;

public class TeamServiceTests
{
    private readonly Mock<ITeamRepository> _teamRepoMock;
    private readonly Mock<ILeagueRepository> _leagueRepoMock;
    private readonly TeamService _sut;

    public TeamServiceTests()
    {
        _teamRepoMock = new Mock<ITeamRepository>();
        _leagueRepoMock = new Mock<ILeagueRepository>();
        _sut = new TeamService(_teamRepoMock.Object, _leagueRepoMock.Object);
    }

    [Fact]
    public async Task GetAllTeamsAsync_ReturnsAllTeams()
    {
        var teams = new List<Team> { new() { Name = "Real Madrid" }, new() { Name = "Barcelona" } };
        _teamRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(teams);

        var result = await _sut.GetAllTeamsAsync();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetTeamByIdAsync_WhenExists_ReturnsTeam()
    {
        var id = Guid.NewGuid();
        var team = new Team { Id = id, Name = "Real Madrid" };
        _teamRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(team);

        var result = await _sut.GetTeamByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal("Real Madrid", result.Name);
    }

    [Fact]
    public async Task GetTeamByIdAsync_WhenNotFound_ReturnsNull()
    {
        _teamRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Team?)null);

        var result = await _sut.GetTeamByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateTeamAsync_WhenLeagueDoesNotExist_ReturnsNull()
    {
        _leagueRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Guid>())).ReturnsAsync(false);

        var result = await _sut.CreateTeamAsync("Real Madrid", Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateTeamAsync_WhenLeagueExists_ReturnsTeam()
    {
        var leagueId = Guid.NewGuid();
        var created = new Team { Name = "Real Madrid", LeagueId = leagueId, InviteCode = "ABC12345" };

        _leagueRepoMock.Setup(r => r.ExistsAsync(leagueId)).ReturnsAsync(true);
        _teamRepoMock.Setup(r => r.CreateAsync(It.IsAny<Team>())).ReturnsAsync(created);

        var result = await _sut.CreateTeamAsync("Real Madrid", leagueId);

        Assert.NotNull(result);
        Assert.Equal("Real Madrid", result.Name);
        Assert.Equal(leagueId, result.LeagueId);
    }
}
