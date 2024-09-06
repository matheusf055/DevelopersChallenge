using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LolTournament.Models
{
    public class Match
    {
        // Unique identifier for the match
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Foreign key for Team A
        [Required]
        [ForeignKey(nameof(TeamA))]
        public int TeamAId { get; set; }

        // Property for Team A
        [Required]
        public Team TeamA { get; set; }

        // Foreign key for Team B
        [Required]
        [ForeignKey(nameof(TeamB))]
        public int TeamBId { get; set; }

        // Property for Team B
        [Required]
        public Team TeamB { get; set; }

        // Foreign key for the Tournament
        [Required]
        [ForeignKey(nameof(Tournament))]
        public int TournamentId { get; set; }

        // Property for the Tournament
        [Required]
        public Tournament Tournament { get; set; }

        // Foreign key for the Winner
        [ForeignKey(nameof(Winner))]
        public int WinnerId { get; set; }

        // Property for the Winner
        public Team Winner { get; set; }
    }
}
