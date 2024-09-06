namespace LolTournament.Application.DTOs
{
    public class MatchUpdateDto
    {
        public int Id { get; set; }
        public int TeamAId { get; set; }
        public int TeamBId { get; set; }
        public int TournamentId { get; set; }
        public int WinnerId { get; set; }
    }
}
