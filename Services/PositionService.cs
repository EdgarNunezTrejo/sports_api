using sports_api.Interfaces;
using sports_api.Models;


namespace sports_api.Services;

public class PositionService(
    IPositionRepository positionRepository,
    ISportRepository sportRepository
)
{
    public async Task<List<Position>> GetAllPositionsAsync()
    {
        return await positionRepository.GetAllAsync();
    }

    public async Task<Position?> GetPositionByIdAsync(Guid id)
    {
        return await positionRepository.GetByIdAsync(id);
    }

    public async Task<(Position? Position, string? Error)> CreatePositionAsync(string name, Guid sportId)
    {
        var sport = await sportRepository.GetByIdAsync(sportId);
        if (sport == null)
            return (null, "The specified sport does not exist.");

        var position = new Position { Name = name, SportId = sportId };
        var created = await positionRepository.CreateAsync(position);
        return (created, null);
    }
}