namespace sports_api.Models;

public enum PlayerStatus
{
    Active,
    Inactive,
    Suspended
}

public class Player
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Name { get; set; } = string.Empty;
    public bool IsLeader { get; set; }

    public Guid TeamId { get; set; }
    public Team? Team { get; set; }
    public Guid? PositionId { get; set; }  // nullable porque es opcional
    public Position? Position { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }
    public PlayerStatus Status {get; set;} = PlayerStatus.Active;
}