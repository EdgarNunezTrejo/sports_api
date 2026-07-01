using sports_api.Models;
using sports_api.Repositories;

namespace sports_api.Services;

public class SportService(SportRepository repository)
{
    public async Task<List<Sport>> GetAllSportsAsync()
    {
        return await repository.GetAllAsync();
    }

    public async Task<Sport?> GetSportByIdAsync(Guid id)
    {
        return await repository.GetByIdAsync(id);
    }

    public async Task<Sport> CreateSportAsync(string name)
    {
        var sport = new Sport { Name = name };
        return await repository.CreateAsync(sport);
    }
}
