using sports_api.Models;
using sports_api.Interfaces;
namespace sports_api.Services;

public class MatchEventService(
    IMatchEventRepository matchEventRepository,
    IMatchRepository matchRepository,
    IPlayerRepository playerRepository)
{
    public async Task<List<MatchEvent>> GetMatchEventsAsync(Guid matchId)
    {
        return await matchEventRepository.GetByMatchIdAsync(matchId);
    }

    public async Task<(MatchEvent? Event, string? Error)> CreateMatchEventAsync(
        Guid matchId, Guid teamId, Guid? playerId, EventType type, int points, int? minute)
    {
        var match = await matchRepository.GetByIdWithTeamsAsync(matchId);
        if (match == null)
            return (null, "The specified match does not exist");

        if (match.Status == MatchStatus.Cancelled)
            return (null, "Cannot add events to a cancelled match");

        var teamBelongsToMatch = match.HomeTeamId == teamId || match.AwayTeamId == teamId;
        if (!teamBelongsToMatch)
            return (null, "The team does not participate in this match");
        if (playerId.HasValue)
        {
            var playerBelongsToTeam = await playerRepository.BelongsToTeamAsync(playerId.Value, teamId);
            if (!playerBelongsToTeam)
                return (null, "The player does not belong to the specified team");
        }

        // If the match is scheduled, we can start it when the first event is created
        if (match.Status == MatchStatus.Scheduled)
        {
            match.Status = MatchStatus.InProgress;
            await matchRepository.UpdateAsync(match);
        }

        var matchEvent = new MatchEvent
        {
            MatchId = matchId,
            TeamId = teamId,
            PlayerId = playerId,
            Type = type,
            Points = points,
            Minute = minute
        };

        var created = await matchEventRepository.CreateAsync(matchEvent);
        return (created, null);
    }

    // Score calculation method
    public async Task<(int HomeScore, int AwayScore)> GetScoreAsync(Guid matchId, Guid homeTeamId, Guid awayTeamId)
    {
        var events = await matchEventRepository.GetByMatchIdAsync(matchId);

        var homeScore = events
            .Where(e => e.TeamId == homeTeamId)
            .Sum(e => e.Points);

        var awayScore = events
            .Where(e => e.TeamId == awayTeamId)
            .Sum(e => e.Points);

        return (homeScore, awayScore);
    }
}