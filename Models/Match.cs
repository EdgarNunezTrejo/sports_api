namespace sports_api.Models;

public enum MatchStatus
{
    Scheduled,
    Played,
    Cancelled,
    InProgress
}

public class Match
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

    public Guid LeagueId { get; set; }
    public League? League { get; set; }

    public Guid HomeTeamId { get; set; }
    public Team? HomeTeam { get; set; }

    public Guid AwayTeamId { get; set; }
    public Team? AwayTeam { get; set; }

    public DateTime ScheduledDate { get; set; }
    public MatchStatus Status { get; set; } = MatchStatus.Scheduled;

    public List<MatchEvent> Events { get; set; } = new();
}