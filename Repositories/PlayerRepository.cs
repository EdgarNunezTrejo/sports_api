using Microsoft.EntityFrameworkCore;
using sports_api.Data;
using sports_api.Models;

namespace sports_api.Repositories;

public class PlayerRepository(AppDbContext context)
{
    public async Task<List<Player>> GetAllAsync()
    {
        return await context.Players.ToListAsync();
    }

    public async Task<Player?> GetByIdAsync(Guid id)
    {
        return await context.Players.FindAsync(id);
    }

    public async Task<Player> CreateAsync(Player player)
    {
        context.Players.Add(player);
        await context.SaveChangesAsync();
        return player;
    }

    public async Task<bool> BelongsToTeamAsync(Guid playerId, Guid teamId)
    {
        return await context.Players
            .AnyAsync(p => p.Id == playerId && p.TeamId == teamId);
    }

    public async Task<bool> IsLeaderAsync(Guid playerId)
    {
        return await context.Players
            .AnyAsync(p => p.Id == playerId && p.IsLeader);
    }
}
