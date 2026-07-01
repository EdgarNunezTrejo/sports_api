namespace sports_api.Models;

public class Position
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Name { get; set; } = string.Empty; // "Goalkeeper", "Point Guard"

    public Guid SportId { get; set; }
    public Sport? Sport { get; set; }
}