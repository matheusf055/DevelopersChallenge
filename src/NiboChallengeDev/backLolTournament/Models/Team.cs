using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LolTournament.Models
{
    // Specifies that the Name property must be unique
    [Index(nameof(Name), IsUnique = true)]
    public class Team
    {
        // Unique identifier for the team
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Name of the team
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        // Name of the Region
        [Required]   
        public string Region { get; set; }

        // Collection of matches where the team is Team A
        public ICollection<Match> MatchesAsTeamA { get; set; } = new List<Match>();

        // Collection of matches where the team is Team B
        public ICollection<Match> MatchesAsTeamB { get; set; } = new List<Match>();

        // Collection of matches where the team is the Winner
        public ICollection<Match> MatchesAsWinner { get; set; } = new List<Match>();
    }
}
