namespace sports_api.DTOs;

public class TeamResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string InviteCode { get; set; } = string.Empty;
    public Guid LeagueId { get; set; }
}

public class CreateTeamDto
{
    public string Name {get; set; } = string.Empty;
    public Guid LeagueId {get; set; }
}