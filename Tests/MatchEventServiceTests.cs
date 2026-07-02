using Moq;
using sports_api.Interfaces;
using sports_api.Models;
using sports_api.Services;
using Xunit;
using Match = sports_api.Models.Match;

namespace Tests;

public class MatchEventServiceTests
{
    private readonly Mock<IMatchEventRepository> _matchEventRepoMock;
    private readonly Mock<IMatchRepository> _matchRepoMock;
    private readonly Mock<IPlayerRepository> _playerRepoMock;
    private readonly MatchEventService _sut;

    public MatchEventServiceTests()
    {
        _matchEventRepoMock = new Mock<IMatchEventRepository>();
        _matchRepoMock = new Mock<IMatchRepository>();
        _playerRepoMock = new Mock<IPlayerRepository>();
        _sut = new MatchEventService(_matchEventRepoMock.Object, _matchRepoMock.Object, _playerRepoMock.Object);
    }

    [Fact]
    public async Task GetMatchEventsAsync_ReturnsEvents()
    {
        var matchId = Guid.NewGuid();
        var events = new List<MatchEvent> { new() { MatchId = matchId }, new() { MatchId = matchId } };
        _matchEventRepoMock.Setup(r => r.GetByMatchIdAsync(matchId)).ReturnsAsync(events);

        var result = await _sut.GetMatchEventsAsync(matchId);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task CreateMatchEventAsync_WhenMatchNotFound_ReturnsError()
    {
        _matchRepoMock.Setup(r => r.GetByIdWithTeamsAsync(It.IsAny<Guid>())).ReturnsAsync((Match?)null);

        var (ev, error) = await _sut.CreateMatchEventAsync(Guid.NewGuid(), Guid.NewGuid(), null, EventType.Goal, 1, null);

        Assert.Null(ev);
        Assert.Equal("The specified match does not exist", error);
    }

    [Fact]
    public async Task CreateMatchEventAsync_WhenMatchCancelled_ReturnsError()
    {
        var match = new Match { Status = MatchStatus.Cancelled, HomeTeamId = Guid.NewGuid(), AwayTeamId = Guid.NewGuid() };
        _matchRepoMock.Setup(r => r.GetByIdWithTeamsAsync(It.IsAny<Guid>())).ReturnsAsync(match);

        var (ev, error) = await _sut.CreateMatchEventAsync(Guid.NewGuid(), match.HomeTeamId, null, EventType.Goal, 1, null);

        Assert.Null(ev);
        Assert.Equal("Cannot add events to a cancelled match", error);
    }

    [Fact]
    public async Task CreateMatchEventAsync_WhenTeamNotInMatch_ReturnsError()
    {
        var match = new Match
        {
            Status = MatchStatus.InProgress,
            HomeTeamId = Guid.NewGuid(),
            AwayTeamId = Guid.NewGuid()
        };
        _matchRepoMock.Setup(r => r.GetByIdWithTeamsAsync(It.IsAny<Guid>())).ReturnsAsync(match);

        var (ev, error) = await _sut.CreateMatchEventAsync(Guid.NewGuid(), Guid.NewGuid(), null, EventType.Goal, 1, null);

        Assert.Null(ev);
        Assert.Equal("The team does not participate in this match", error);
    }

    [Fact]
    public async Task CreateMatchEventAsync_WhenPlayerNotInTeam_ReturnsError()
    {
        var teamId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var match = new Match { Status = MatchStatus.InProgress, HomeTeamId = teamId, AwayTeamId = Guid.NewGuid() };

        _matchRepoMock.Setup(r => r.GetByIdWithTeamsAsync(It.IsAny<Guid>())).ReturnsAsync(match);
        _playerRepoMock.Setup(r => r.BelongsToTeamAsync(playerId, teamId)).ReturnsAsync(false);

        var (ev, error) = await _sut.CreateMatchEventAsync(Guid.NewGuid(), teamId, playerId, EventType.Goal, 1, null);

        Assert.Null(ev);
        Assert.Equal("The player does not belong to the specified team", error);
    }

    [Fact]
    public async Task CreateMatchEventAsync_WhenMatchIsScheduled_StartsMatch()
    {
        var teamId = Guid.NewGuid();
        var match = new Match { Status = MatchStatus.Scheduled, HomeTeamId = teamId, AwayTeamId = Guid.NewGuid() };
        var created = new MatchEvent { TeamId = teamId, Type = EventType.Goal, Points = 1 };

        _matchRepoMock.Setup(r => r.GetByIdWithTeamsAsync(It.IsAny<Guid>())).ReturnsAsync(match);
        _matchRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Match>())).ReturnsAsync(match);
        _matchEventRepoMock.Setup(r => r.CreateAsync(It.IsAny<MatchEvent>())).ReturnsAsync(created);

        await _sut.CreateMatchEventAsync(Guid.NewGuid(), teamId, null, EventType.Goal, 1, null);

        _matchRepoMock.Verify(r => r.UpdateAsync(It.Is<Match>(m => m.Status == MatchStatus.InProgress)), Times.Once);
    }

    [Fact]
    public async Task CreateMatchEventAsync_WhenValidData_ReturnsEvent()
    {
        var teamId = Guid.NewGuid();
        var match = new Match { Status = MatchStatus.InProgress, HomeTeamId = teamId, AwayTeamId = Guid.NewGuid() };
        var created = new MatchEvent { TeamId = teamId, Type = EventType.Goal, Points = 1 };

        _matchRepoMock.Setup(r => r.GetByIdWithTeamsAsync(It.IsAny<Guid>())).ReturnsAsync(match);
        _matchEventRepoMock.Setup(r => r.CreateAsync(It.IsAny<MatchEvent>())).ReturnsAsync(created);

        var (ev, error) = await _sut.CreateMatchEventAsync(Guid.NewGuid(), teamId, null, EventType.Goal, 1, 10);

        Assert.NotNull(ev);
        Assert.Null(error);
        Assert.Equal(EventType.Goal, ev.Type);
    }
}
