using sports_api.Models;
using sports_api.Repositories;

namespace sports_api.Services;

public class TeamService(TeamRepository repository, LeagueRepository leagueRepository)
{
    public async Task<List<Team>> GetAllTeamsAsync()
    {
        return await repository.GetAllAsync();
    }

    public async Task<Team?> GetTeamByIdAsync(Guid id)
    {
        return await repository.GetByIdAsync(id);
    }

    public async Task<Team> CreateTeamAsync(string name, Guid leagueId)
    {
        var leagueExists = await leagueRepository.LeagueExistsAsync(leagueId);
        if (!leagueExists)
        {
            return null;
        }

        var team = new Team
        {
            Name = name,
            LeagueId = leagueId,
            InviteCode = Guid.NewGuid().ToString("N")[..8].ToUpper()
        };

        return await repository.CreateAsync(team);
    }
}
