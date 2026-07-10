using sports_api.Interfaces;
using sports_api.Models;

namespace sports_api.Services;

public class PlayerService(
    IPlayerRepository playerRepository,
    IPositionRepository positionRepository,
    ITeamRepository teamRepository
    )
{
    public async Task<List<Player>> GetAllPlayersAsync()
    {
        return await playerRepository.GetAllAsync();
    }

    public async Task<Player?> GetPlayerByIdAsync(Guid id)
    {
        return await playerRepository.GetByIdAsync(id);
    }
    public async Task<(Player? Player, string? Error)> CreatePlayerAsync(
        string name, bool isLeader, Guid teamId, Guid? positionId)
    {
        var teamSportId = await teamRepository.GetSportIdAsync(teamId);
        if (teamSportId == null)
            return (null, "The specified team does not exist");

        if (positionId.HasValue)
        {
            var positionSportId = await positionRepository.GetSportIdAsync(positionId.Value);
            if (positionSportId == null)
                return (null, "The specified position does not exist");

            if (positionSportId != teamSportId)
                return (null, "The position does not belong to the sport of the team's league");
        }

        var player = new Player
        {
            Name = name,
            IsLeader = isLeader,
            TeamId = teamId,
            PositionId = positionId,
            Status = PlayerStatus.Active
        };

        var created = await playerRepository.CreateAsync(player);
        return (created, null);
    }
}
