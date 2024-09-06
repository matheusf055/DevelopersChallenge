using Microsoft.EntityFrameworkCore;
using LolTournament.Models;

namespace LolTournament.Data
{
    public class DatabaseContext : DbContext
    {
        // Constructor to initialize the DbContext with options
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        // DbSet properties for accessing the database tables
        public DbSet<Team> Teams { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }

        // Configures the entity relationships and table mappings
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configures the relationship between Match and TeamA
            modelBuilder.Entity<Match>()
                .HasOne(m => m.TeamA)  // Match has one TeamA
                .WithMany(t => t.MatchesAsTeamA)  // TeamA has many MatchesAsTeamA
                .HasForeignKey(m => m.TeamAId)  // Foreign key for TeamA in Match
                .OnDelete(DeleteBehavior.Restrict);  // Restricts deletion of TeamA if there are dependent Matches

            // Configures the relationship between Match and TeamB
            modelBuilder.Entity<Match>()
                .HasOne(m => m.TeamB)  // Match has one TeamB
                .WithMany(t => t.MatchesAsTeamB)  // TeamB has many MatchesAsTeamB
                .HasForeignKey(m => m.TeamBId)  // Foreign key for TeamB in Match
                .OnDelete(DeleteBehavior.Restrict);  // Restricts deletion of TeamB if there are dependent Matches

            // Configures the relationship between Match and Winner
            modelBuilder.Entity<Match>()
                .HasOne(m => m.Winner)  // Match has one Winner
                .WithMany(t => t.MatchesAsWinner)  // Winner has many MatchesAsWinner
                .HasForeignKey(m => m.WinnerId)  // Foreign key for Winner in Match
                .OnDelete(DeleteBehavior.Restrict);  // Restricts deletion of Winner if there are dependent Matches

            // Configures the relationship between Match and Tournament
            modelBuilder.Entity<Match>()
                .HasOne(m => m.Tournament)  // Match has one Tournament
                .WithMany(t => t.Matches)  // Tournament has many Matches
                .HasForeignKey(m => m.TournamentId)  // Foreign key for Tournament in Match
                .OnDelete(DeleteBehavior.Restrict);  // Restricts deletion of Tournament if there are dependent Matches

            // Maps the entities to their respective database tables
            modelBuilder.Entity<Team>().ToTable("Teams");
            modelBuilder.Entity<Match>().ToTable("Matches");
            modelBuilder.Entity<Tournament>().ToTable("Tournaments");
        }
    }
}
