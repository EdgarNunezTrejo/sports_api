using sports_api.Models;

namespace sports_api.Interfaces;

public interface ISportRepository
{
    Task<List<Sport>> GetAllAsync();
    Task<Sport?> GetByIdAsync(Guid id);
    Task<Sport> CreateAsync(Sport sport);
}