using sports_api.Models;

namespace sports_api.Interfaces;

public interface ITeamRepository
{
    Task<List<Team>> GetAllAsync();
    Task<Team?> GetByIdAsync(Guid id);
    Task<Guid?> GetLeagueIdAsync(Guid teamId);
    Task<Guid?> GetSportIdAsync(Guid teamId);
    Task<Team> CreateAsync(Team team);
    Task<Team?> GetByInviteCodeAsync(string inviteCode);
}