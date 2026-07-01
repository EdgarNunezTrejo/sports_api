using Microsoft.EntityFrameworkCore;
using sports_api.Data;
using sports_api.Models;

namespace sports_api.Repositories;

public class MatchEventRepository(AppDbContext context)
{
    public async Task<List<MatchEvent>> GetByMatchIdAsync(Guid matchId)
    {
        return await context.MatchEvents
            .Where(me => me.MatchId == matchId)
            .OrderBy(me => me.Minute)
            .ToListAsync();
    }

    public async Task<MatchEvent> CreateAsync(MatchEvent matchEvent)
    {
        context.MatchEvents.Add(matchEvent);
        await context.SaveChangesAsync();
        return matchEvent;
    }
}