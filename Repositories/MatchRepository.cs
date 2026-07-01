using Microsoft.EntityFrameworkCore;
using sports_api.Data;
using sports_api.Models;

namespace sports_api.Repositories;

public class MatchRepository(AppDbContext context)
{
    public async Task<List<Match>> GetAllAsync()
    {
        return await context.Matches.ToListAsync();
    }

    public async Task<Match?> GetByIdAsync(Guid id)
    {
        return await context.Matches.FindAsync(id);
    }

    public async Task<Match> CreateAsync(Match match)
    {
        context.Matches.Add(match);
        await context.SaveChangesAsync();
        return match;
    }

    public async Task<Match?> GetByIdWithTeamsAsync(Guid id)
    {
        return await context.Matches
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Match> UpdateAsync(Match match)
    {
        context.Matches.Update(match);
        await context.SaveChangesAsync();
        return match;
    }
}