namespace sports_api.DTOs;

public class MatchResponseDto
{
    public Guid Id { get; set; }
    public Guid LeagueId { get; set; }
    public Guid HomeTeamId { get; set; }
    public Guid AwayTeamId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class CreateMatchDto
{
    public Guid LeagueId { get; set; }
    public Guid HomeTeamId { get; set; }
    public Guid AwayTeamId { get; set; }
    public DateTime ScheduledDate { get; set; }
}