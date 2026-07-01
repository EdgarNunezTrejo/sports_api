namespace sports_api.Models;

public class Team
{
    public Guid Id {get; set;} = Guid.CreateVersion7();
    public string Name {get; set;} = string.Empty;
    public string InviteCode {get;set;} = string.Empty;

    public Guid LeagueId {get; set;}
    public League League {get; set;} = new();

    public List<Player> Players {get; set;} = new();
}