using Microsoft.EntityFrameworkCore;
using sports_api.Data;
using sports_api.Interfaces;
using sports_api.Models;

namespace sports_api.Repositories;

public class PositionRepository(AppDbContext context) : IPositionRepository
{
    public async Task<List<Position>> GetAllAsync()
    {
        return await context.Positions.ToListAsync();
    }

    public async Task<Position?> GetByIdAsync(Guid id)
    {
        return await context.Positions.FindAsync(id);
    }

    public async Task<Position> CreateAsync(Position position)
    {
        context.Positions.Add(position);
        await context.SaveChangesAsync();
        return position;
    }

    public async Task<Guid?> GetSportIdAsync(Guid positionId)
    {
        return await context.Positions
            .Where(p => p.Id == positionId)
            .Select(p => p.SportId)
            .FirstOrDefaultAsync();
    }
}
