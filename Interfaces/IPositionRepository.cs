using sports_api.Models;

namespace sports_api.Interfaces;

public interface IPositionRepository
{
    Task<List<Position>> GetAllAsync();
    Task<Position?> GetByIdAsync(Guid id);
    Task<Guid?> GetSportIdAsync(Guid positionId);
    Task<Position> CreateAsync(Position position);
}