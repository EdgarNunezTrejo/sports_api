using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using sports_api.Models;
namespace sports_api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<League> Leagues => Set<League>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<MatchEvent> MatchEvents => Set<MatchEvent>();
    public DbSet<Player> Players => Set<Player>();
    public DbSet<Position> Positions => Set<Position>();
    public DbSet<Sport> Sports => Set<Sport>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Match>()
        .HasOne(m => m.HomeTeam)
        .WithMany()
        .HasForeignKey(m => m.HomeTeamId)
        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Match>()
        .HasOne(m => m.AwayTeam)
        .WithMany()
        .HasForeignKey(m => m.AwayTeamId)
        .OnDelete(DeleteBehavior.Restrict);

        // MatchEvent: FK to Team (not ambiguous, but we want to restrict delete behavior)
        modelBuilder.Entity<MatchEvent>()
            .HasOne(me => me.Team)
            .WithMany()
            .HasForeignKey(me => me.TeamId)
            .OnDelete(DeleteBehavior.Restrict);

        // MatchEvent: FK opcional a Player
        modelBuilder.Entity<MatchEvent>()
            .HasOne(me => me.Player)
            .WithMany()
            .HasForeignKey(me => me.PlayerId)
            .OnDelete(DeleteBehavior.SetNull);

        // Conversation: two FKs to Team (TeamA/TeamB) — ambiguos, it needs Fluent API
        modelBuilder.Entity<Conversation>()
            .HasOne(c => c.TeamA)
            .WithMany()
            .HasForeignKey(c => c.TeamAId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Conversation>()
            .HasOne(c => c.TeamB)
            .WithMany()
            .HasForeignKey(c => c.TeamBId)
            .OnDelete(DeleteBehavior.Restrict);

        // ChatMessage: FK to SenderPlayer (not ambiguous, but we want to restrict delete behavior)
        modelBuilder.Entity<ChatMessage>()
            .HasOne(cm => cm.SenderPlayer)
            .WithMany()
            .HasForeignKey(cm => cm.SenderPlayerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}