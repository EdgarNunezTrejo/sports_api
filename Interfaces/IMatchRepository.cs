using sports_api.Models;

namespace sports_api.Interfaces;

public interface IMatchRepository
{
    Task<List<Match>> GetAllAsync();
    Task<Match?> GetByIdAsync(Guid id);
    Task<Match?> GetByIdWithTeamsAsync(Guid id);
    Task<Match> CreateAsync(Match match);
    Task<Match> UpdateAsync(Match match);
}