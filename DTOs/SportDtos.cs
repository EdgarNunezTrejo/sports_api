namespace sports_api.DTOs;

public class SportResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class CreateSportDto
{
    public string Name { get; set; } = string.Empty;
}