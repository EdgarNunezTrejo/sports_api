namespace sports_api.DTOs;

public class PositionResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid SportId { get; set; }
}

public class CreatePositionDto
{
    public string Name { get; set; } = string.Empty;
    public Guid SportId { get; set; }
}