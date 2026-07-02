using sports_api.Models;
using sports_api.Interfaces;

namespace sports_api.Services;

public class TeamService(
    ITeamRepository repository,
    ILeagueRepository leagueRepository
    )
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
        var leagueExists = await leagueRepository.ExistsAsync(leagueId);
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
