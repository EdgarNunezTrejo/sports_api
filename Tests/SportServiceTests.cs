using Moq;
using sports_api.Interfaces;
using sports_api.Models;
using sports_api.Services;
using Xunit;

namespace Tests;

public class SportServiceTests
{
    private readonly Mock<ISportRepository> _repoMock;
    private readonly SportService _sut;

    public SportServiceTests()
    {
        _repoMock = new Mock<ISportRepository>();
        _sut = new SportService(_repoMock.Object);
    }

    [Fact]
    public async Task GetAllSportsAsync_ReturnsAllSports()
    {
        var sports = new List<Sport> { new() { Name = "Football" }, new() { Name = "Basketball" } };
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(sports);

        var result = await _sut.GetAllSportsAsync();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetSportByIdAsync_WhenExists_ReturnsSport()
    {
        var id = Guid.NewGuid();
        var sport = new Sport { Id = id, Name = "Football" };
        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(sport);

        var result = await _sut.GetSportByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal("Football", result.Name);
    }

    [Fact]
    public async Task GetSportByIdAsync_WhenNotFound_ReturnsNull()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Sport?)null);

        var result = await _sut.GetSportByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateSportAsync_ReturnsCreatedSport()
    {
        var created = new Sport { Name = "Tennis" };
        _repoMock.Setup(r => r.CreateAsync(It.IsAny<Sport>())).ReturnsAsync(created);

        var result = await _sut.CreateSportAsync("Tennis");

        Assert.NotNull(result);
        Assert.Equal("Tennis", result.Name);
    }
}
