using Microsoft.EntityFrameworkCore;
using sports_api.Data;
using sports_api.Interfaces;
using sports_api.Models;

namespace sports_api.Repositories;

public class TeamRepository(AppDbContext context) : ITeamRepository
{
    public async Task<List<Team>> GetAllAsync()
    {
        return await context.Teams.ToListAsync();
    }

    public async Task<Team?> GetByIdAsync(Guid id)
    {
        return await context.Teams.FindAsync(id);
    }

    public async Task<Team> CreateAsync(Team team)
    {
        context.Teams.Add(team);
        await context.SaveChangesAsync();
        return team;
    }

    public async Task<Guid?> GetSportIdAsync(Guid teamId)
    {
        return await context.Teams
            .Where(t => t.Id == teamId)
            .Select(t => t.League!.SportId)
            .FirstOrDefaultAsync();
    }

    public async Task<Guid?> GetLeagueIdAsync(Guid teamId)
    {
        return await context.Teams
            .Where(t => t.Id == teamId)
            .Select(t => (Guid?)t.LeagueId)
            .FirstOrDefaultAsync();
    }

    public async Task<Team?> GetByInviteCodeAsync(string inviteCode)
    {
        return await context.Teams
            .FirstOrDefaultAsync(t => t.InviteCode == inviteCode.ToUpper());
    }
}
