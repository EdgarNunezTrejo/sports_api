namespace sports_api.DTOs;

public class PlayerResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public bool IsLeader { get; set; }
    public Guid TeamId { get; set; }
    public Guid? PositionId { get; set; }
}

public class CreatePlayerDto
{
    public string Name { get; set; } = null!;
    public bool IsLeader { get; set; }
    public Guid TeamId { get; set; }
    public Guid? PositionId { get; set; }
}