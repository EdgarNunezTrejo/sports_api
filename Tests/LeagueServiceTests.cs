using Moq;
using sports_api.Interfaces;
using sports_api.Models;
using sports_api.Services;
using Xunit;

namespace Tests;

public class LeagueServiceTests
{
    private readonly Mock<ILeagueRepository> _repoMock;
    private readonly LeagueService _sut;

    public LeagueServiceTests()
    {
        _repoMock = new Mock<ILeagueRepository>();
        _sut = new LeagueService(_repoMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllLeagues()
    {
        var leagues = new List<League> { new() { Name = "Premier League" }, new() { Name = "La Liga" } };
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(leagues);

        var result = await _sut.GetAllAsync();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsLeague()
    {
        var id = Guid.NewGuid();
        var league = new League { Id = id, Name = "Premier League" };
        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(league);

        var result = await _sut.GetByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal("Premier League", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((League?)null);

        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_ReturnsCreatedLeague()
    {
        var input = new League { Name = "Bundesliga" };
        var created = new League { Name = "Bundesliga" };
        _repoMock.Setup(r => r.CreateAsync(It.IsAny<League>())).ReturnsAsync(created);

        var result = await _sut.CreateAsync(input);

        Assert.NotNull(result);
        Assert.Equal("Bundesliga", result.Name);
    }
}
