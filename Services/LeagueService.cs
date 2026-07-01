using sports_api.Models;
using sports_api.Repositories;

namespace sports_api.Services;

public class LeagueService(LeagueRepository repository)
{
    public async Task<List<League>> GetAllAsync()
    {
        return await repository.GetAllAsync();
    }

    public async Task<League?> GetByIdAsync(Guid id)
    {
        return await repository.GetByIdAsync(id);
    }

    public async Task<League> CreateAsync(League league)
    {
        var newLeague = new League
        {
            Name = league.Name,
            Sport = league.Sport,
        };

        return await repository.CreateAsync(newLeague);
    }
}
