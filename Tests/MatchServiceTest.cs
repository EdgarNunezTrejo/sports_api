using Moq;
using sports_api.Interfaces;
using sports_api.Models;
using sports_api.Services;
using Xunit;
using Match = sports_api.Models.Match;

namespace Tests;

public class MatchServiceTest
{
    private readonly MatchService _sut;
    private readonly Mock<IMatchRepository> _matchRepoMock;
    private readonly Mock<ITeamRepository> _teamRepoMock;
    private readonly Mock<ILeagueRepository> _leagueRepoMock;

    public MatchServiceTest()
    {
        _matchRepoMock = new Mock<IMatchRepository>();
        _teamRepoMock = new Mock<ITeamRepository>();
        _leagueRepoMock = new Mock<ILeagueRepository>();

        _sut = new MatchService(
            _matchRepoMock.Object,
            _teamRepoMock.Object,
            _leagueRepoMock.Object
        );

    }
    [Fact]
    public async Task TeamIsPlayingAgainstItself_ReturnsError()
    {
        _matchRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new sports_api.Models.Match
            {
                HomeTeamId = Guid.NewGuid(),
                AwayTeamId = Guid.NewGuid()
            });

        var sameTeamId = Guid.NewGuid();

        var (match, error) = await _sut.CreateMatchAsync(
            Guid.NewGuid(),
            sameTeamId,
            sameTeamId,
            DateTime.Now
        );

        Assert.Null(match);
        Assert.Equal("A team cannot play against itself", error);
    }

    [Fact]
    public async Task SpecifiedLeagueDoesNotExist_ReturnsError()
    {
        _leagueRepoMock
        .Setup(r => r.ExistsAsync(It.IsAny<Guid>()))
        .ReturnsAsync(false);


        var (match, error) = await _sut.CreateMatchAsync(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.Now
        );

        Assert.Null(match);
        Assert.Equal("The specified league does not exist", error);
    }

    [Fact]
    public async Task TeamDoesNotBelongToSpecifiedLeague_ReturnsError()
    {
        var leagueId = Guid.NewGuid();
        var differentLeagueId = Guid.NewGuid();

        _leagueRepoMock
            .Setup(r => r.ExistsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);

        _teamRepoMock
            .Setup(r => r.GetLeagueIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(differentLeagueId);

        var (match, error) = await _sut.CreateMatchAsync(
            leagueId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.Now
        );

        Assert.Null(match);
        Assert.Equal("Both teams must belong to the specified league", error);
    }

    [Fact]
    public async Task GetAllMatchesAsync_ReturnsAllMatches()
    {
        var matches = new List<Match> { new(), new() };
        _matchRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(matches);

        var result = await _sut.GetAllMatchesAsync();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetMatchByIdAsync_WhenExists_ReturnsMatch()
    {
        var id = Guid.NewGuid();
        var match = new Match { Id = id };
        _matchRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(match);

        var result = await _sut.GetMatchByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }

    [Fact]
    public async Task GetMatchByIdAsync_WhenNotFound_ReturnsNull()
    {
        _matchRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Match?)null);

        var result = await _sut.GetMatchByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateMatchAsync_WhenHomeTeamDoesNotExist_ReturnsError()
    {
        var leagueId = Guid.NewGuid();
        var homeTeamId = Guid.NewGuid();

        _leagueRepoMock.Setup(r => r.ExistsAsync(leagueId)).ReturnsAsync(true);
        _teamRepoMock.Setup(r => r.GetLeagueIdAsync(homeTeamId)).ReturnsAsync((Guid?)null);

        var (match, error) = await _sut.CreateMatchAsync(leagueId, homeTeamId, Guid.NewGuid(), DateTime.Now);

        Assert.Null(match);
        Assert.Equal("The specified home team does not exist", error);
    }

    [Fact]
    public async Task CreateMatchAsync_WhenAwayTeamDoesNotExist_ReturnsError()
    {
        var leagueId = Guid.NewGuid();
        var homeTeamId = Guid.NewGuid();
        var awayTeamId = Guid.NewGuid();

        _leagueRepoMock.Setup(r => r.ExistsAsync(leagueId)).ReturnsAsync(true);
        _teamRepoMock.Setup(r => r.GetLeagueIdAsync(homeTeamId)).ReturnsAsync(leagueId);
        _teamRepoMock.Setup(r => r.GetLeagueIdAsync(awayTeamId)).ReturnsAsync((Guid?)null);

        var (match, error) = await _sut.CreateMatchAsync(leagueId, homeTeamId, awayTeamId, DateTime.Now);

        Assert.Null(match);
        Assert.Equal("The specified away team does not exist", error);
    }

    [Fact]
    public async Task CreateMatchAsync_WhenValidData_ReturnsMatch()
    {
        var leagueId = Guid.NewGuid();
        var homeTeamId = Guid.NewGuid();
        var awayTeamId = Guid.NewGuid();
        var created = new Match { LeagueId = leagueId, HomeTeamId = homeTeamId, AwayTeamId = awayTeamId };

        _leagueRepoMock.Setup(r => r.ExistsAsync(leagueId)).ReturnsAsync(true);
        _teamRepoMock.Setup(r => r.GetLeagueIdAsync(homeTeamId)).ReturnsAsync(leagueId);
        _teamRepoMock.Setup(r => r.GetLeagueIdAsync(awayTeamId)).ReturnsAsync(leagueId);
        _matchRepoMock.Setup(r => r.CreateAsync(It.IsAny<Match>())).ReturnsAsync(created);

        var (match, error) = await _sut.CreateMatchAsync(leagueId, homeTeamId, awayTeamId, DateTime.Now);

        Assert.NotNull(match);
        Assert.Null(error);
        Assert.Equal(leagueId, match.LeagueId);
    }
}