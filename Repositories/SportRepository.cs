using Microsoft.EntityFrameworkCore;
using sports_api.Data;
using sports_api.Models;

namespace sports_api.Repositories;

public class SportRepository(AppDbContext context)
{
    public async Task<List<Sport>> GetAllAsync()
    {
        return await context.Sports.ToListAsync();
    }

    public async Task<Sport?> GetByIdAsync(Guid id)
    {
        return await context.Sports.FindAsync(id);
    }

    public async Task<Sport> CreateAsync(Sport sport)
    {
        context.Sports.Add(sport);
        await context.SaveChangesAsync();
        return sport;
    }
}
