using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LolTournament.Models
{
    // Specifies that the Name property must be unique
    [Index(nameof(Name), IsUnique = true)]
    public class Tournament
    {
        // Unique identifier for the tournament
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Name of the tournament; 
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        // Start date of the tournament; 
        [Required]
        public DateOnly StartDate { get; set; }

        // Collection of matches associated with the tournament
        public ICollection<Match> Matches { get; set; } = new List<Match>();

        // Winner of the tournament
        public int? WinnerId { get; set; }

        // Navigation property for the winner
        [ForeignKey(nameof(WinnerId))]
        public Team Winner { get; set; }
    }
}
