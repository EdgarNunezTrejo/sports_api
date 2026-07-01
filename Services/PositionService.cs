using sports_api.Models;
using sports_api.Repositories;

namespace sports_api.Services;

public class PositionService
{
    private readonly PositionRepository _positionRepository;
    private readonly SportRepository _sportRepository;

    public PositionService(PositionRepository positionRepository, SportRepository sportRepository)
    {
        _positionRepository = positionRepository;
        _sportRepository = sportRepository;
    }

    public async Task<List<Position>> GetAllPositionsAsync()
    {
        return await _positionRepository.GetAllAsync();
    }

    public async Task<Position?> GetPositionByIdAsync(Guid id)
    {
        return await _positionRepository.GetByIdAsync(id);
    }

    public async Task<(Position? Position, string? Error)> CreatePositionAsync(string name, Guid sportId)
    {
        var sport = await _sportRepository.GetByIdAsync(sportId);
        if (sport == null)
            return (null, "The specified sport does not exist.");

        var position = new Position { Name = name, SportId = sportId };
        var created = await _positionRepository.CreateAsync(position);
        return (created, null);
    }
}