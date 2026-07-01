using sports_api.Models;

namespace sports_api.Interfaces;

public interface IMatchEventRepository
{
    Task<List<MatchEvent>> GetByMatchIdAsync(Guid matchId);
    Task<MatchEvent> CreateAsync(MatchEvent matchEvent);
}