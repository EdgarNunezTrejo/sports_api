namespace sports_api.Models;

public class Sport
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Name { get; set; } = string.Empty; // "Soccer", "Basketball"

    public List<Position> Positions { get; set; } = new();
    public List<League> Leagues { get; set; } = new();
}