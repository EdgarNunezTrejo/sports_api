using sports_api.Models;
using sports_api.Repositories;

namespace sports_api.Services;

public class MatchService(
    MatchRepository matchRepository,
    TeamRepository teamRepository,
    LeagueRepository leagueRepository)
{
    public async Task<List<Match>> GetAllMatchesAsync()
    {
        return await matchRepository.GetAllAsync();
    }

    public async Task<Match?> GetMatchByIdAsync(Guid id)
    {
        return await matchRepository.GetByIdAsync(id);
    }

    public async Task<(Match? Match, string? Error)> CreateMatchAsync(
        Guid leagueId, Guid homeTeamId, Guid awayTeamId, DateTime scheduledDate)
    {
        if (homeTeamId == awayTeamId)
            return (null, "A team cannot play against itself");

        var leagueExists = await leagueRepository.LeagueExistsAsync(leagueId);
        if (!leagueExists)
            return (null, "The specified league does not exist");
        var homeTeamLeagueId = await teamRepository.GetLeagueIdAsync(homeTeamId);
        if (homeTeamLeagueId == null)
            return (null, "The specified home team does not exist");

        var awayTeamLeagueId = await teamRepository.GetLeagueIdAsync(awayTeamId);
        if (awayTeamLeagueId == null)
            return (null, "The specified away team does not exist");
        if (homeTeamLeagueId != leagueId || awayTeamLeagueId != leagueId)
            return (null, "Both teams must belong to the specified league");

        var match = new Match
        {
            LeagueId = leagueId,
            HomeTeamId = homeTeamId,
            AwayTeamId = awayTeamId,
            ScheduledDate = scheduledDate,
            Status = MatchStatus.Scheduled
        };

        var created = await matchRepository.CreateAsync(match);
        return (created, null);
    }
}