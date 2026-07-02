using Moq;
using sports_api.Interfaces;
using sports_api.Models;
using sports_api.Services;
using Xunit;

namespace Tests;

public class PositionServiceTests
{
    private readonly Mock<IPositionRepository> _positionRepoMock;
    private readonly Mock<ISportRepository> _sportRepoMock;
    private readonly PositionService _sut;

    public PositionServiceTests()
    {
        _positionRepoMock = new Mock<IPositionRepository>();
        _sportRepoMock = new Mock<ISportRepository>();
        _sut = new PositionService(_positionRepoMock.Object, _sportRepoMock.Object);
    }

    [Fact]
    public async Task GetAllPositionsAsync_ReturnsAllPositions()
    {
        var positions = new List<Position> { new() { Name = "Forward" }, new() { Name = "Goalkeeper" } };
        _positionRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(positions);

        var result = await _sut.GetAllPositionsAsync();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetPositionByIdAsync_WhenExists_ReturnsPosition()
    {
        var id = Guid.NewGuid();
        var position = new Position { Id = id, Name = "Forward" };
        _positionRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(position);

        var result = await _sut.GetPositionByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal("Forward", result.Name);
    }

    [Fact]
    public async Task GetPositionByIdAsync_WhenNotFound_ReturnsNull()
    {
        _positionRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Position?)null);

        var result = await _sut.GetPositionByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task CreatePositionAsync_WhenSportDoesNotExist_ReturnsError()
    {
        _sportRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Sport?)null);

        var (position, error) = await _sut.CreatePositionAsync("Forward", Guid.NewGuid());

        Assert.Null(position);
        Assert.Equal("The specified sport does not exist.", error);
    }

    [Fact]
    public async Task CreatePositionAsync_WhenSportExists_ReturnsPosition()
    {
        var sportId = Guid.NewGuid();
        var sport = new Sport { Id = sportId, Name = "Football" };
        var created = new Position { Name = "Forward", SportId = sportId };

        _sportRepoMock.Setup(r => r.GetByIdAsync(sportId)).ReturnsAsync(sport);
        _positionRepoMock.Setup(r => r.CreateAsync(It.IsAny<Position>())).ReturnsAsync(created);

        var (position, error) = await _sut.CreatePositionAsync("Forward", sportId);

        Assert.NotNull(position);
        Assert.Null(error);
        Assert.Equal("Forward", position.Name);
        Assert.Equal(sportId, position.SportId);
    }
}
