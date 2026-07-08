namespace sports_api.Models;

public class League
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Name { get; set; } = string.Empty;
    public string Season { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public List<Team> Teams { get; set; } = new();
    public List<Match> Matches { get; set; } = new();
    public Guid SportId { get; set; }
    public Sport? Sport { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}