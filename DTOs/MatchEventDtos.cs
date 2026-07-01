using sports_api.Models;

namespace sports_api.DTOs;

public class MatchEventResponseDto
{
    public Guid Id { get; set; }
    public Guid MatchId { get; set; }
    public Guid TeamId { get; set; }
    public Guid? PlayerId { get; set; }
    public string Type { get; set; } = string.Empty;
    public int Points { get; set; }
    public int? Minute { get; set; }
}

public class CreateMatchEventDto
{
    public Guid TeamId { get; set; }
    public Guid? PlayerId { get; set; }
    public EventType Type { get; set; }
    public int Points { get; set; }
    public int? Minute { get; set; }
}

public class MatchScoreDto
{
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
}