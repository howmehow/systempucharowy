using Microsoft.EntityFrameworkCore;
using TournamentApi.Models;

namespace TournamentApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Tournament> Tournaments { get; set; }
    public DbSet<Bracket> Brackets { get; set; }
    public DbSet<Match> Matches { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.FirstName).IsRequired();
            entity.Property(u => u.LastName).IsRequired();
            entity.Property(u => u.Email).IsRequired();
            entity.Property(u => u.PasswordHash).IsRequired();

            // Configure many-to-many relationship with Tournament
            entity.HasMany(u => u.Tournaments)
                .WithMany(t => t.Participants)
                .UsingEntity(j => j.ToTable("TournamentParticipants"));
        });

        // Tournament configuration
        modelBuilder.Entity<Tournament>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Name).IsRequired();
            entity.Property(t => t.StartDate).IsRequired();
            entity.Property(t => t.Status).IsRequired();

            // Configure one-to-one relationship with Bracket
            entity.HasOne(t => t.Bracket)
                .WithOne(b => b.Tournament)
                .HasForeignKey<Tournament>(t => t.BracketId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Bracket configuration
        modelBuilder.Entity<Bracket>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.TournamentId).IsRequired();

            // Configure one-to-many relationship with Match
            entity.HasMany(b => b.Matches)
                .WithOne(m => m.Bracket)
                .HasForeignKey(m => m.BracketId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Match configuration
        modelBuilder.Entity<Match>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Round).IsRequired();

            // Configure relationship with Player1
            entity.HasOne(m => m.Player1)
                .WithMany(u => u.MatchesAsPlayer1)
                .HasForeignKey(m => m.Player1Id)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure relationship with Player2
            entity.HasOne(m => m.Player2)
                .WithMany(u => u.MatchesAsPlayer2)
                .HasForeignKey(m => m.Player2Id)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure relationship with Winner
            entity.HasOne(m => m.Winner)
                .WithMany(u => u.MatchesWon)
                .HasForeignKey(m => m.WinnerId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
