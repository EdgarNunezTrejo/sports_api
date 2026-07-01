namespace sports_api.DTOs;

public class LeagueDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Country { get; set; } = null!;
}

public class CreateLeagueDto
{
    public string Name { get; set; } = null!;
    public string Country { get; set; } = null!;
}