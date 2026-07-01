using sports_api.Models;

namespace sports_api.Interfaces;

public interface IPlayerRepository
{
    Task<List<Player>> GetAllAsync();
    Task<Player?> GetByIdAsync(Guid id);
    Task<bool> BelongsToTeamAsync(Guid playerId, Guid teamId);
    Task<bool> IsLeaderAsync(Guid playerId);
    Task<Player> CreateAsync(Player player);
}