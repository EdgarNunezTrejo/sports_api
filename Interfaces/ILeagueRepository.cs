using sports_api.Models;

namespace sports_api.Interfaces;

public interface ILeagueRepository
{
    Task<List<League>> GetAllAsync();
    Task<League?> GetByIdAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<League> CreateAsync(League league);
}