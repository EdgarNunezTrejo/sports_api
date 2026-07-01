using Microsoft.EntityFrameworkCore;
using sports_api.Data;
using sports_api.Models;

namespace sports_api.Repositories;

public class LeagueRepository(AppDbContext context)
{
    public async Task<List<League>> GetAllAsync()
    {
        return await context.Leagues.ToListAsync();
    }

    public async Task<League?> GetByIdAsync(Guid id)
    {
        return await context.Leagues.FindAsync(id);
    }

    public async Task<League> CreateAsync(League league)
    {
        context.Leagues.Add(league);
        await context.SaveChangesAsync();
        return league;
    }

    public async Task<bool> LeagueExistsAsync(Guid leagueId)
    {
        return await context.Leagues.AnyAsync(l => l.Id == leagueId);
    }
}
