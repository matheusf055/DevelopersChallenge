namespace LolTournament.Application.DTOs
{
    public class MatchResponseDto
    {
        public int Id { get; set; }
        public TeamResponseDto TeamA { get; set; }
        public TeamResponseDto TeamB { get; set; }
        public TournamentResponseDto Tournament { get; set; }
        public TeamResponseDto Winner { get; set; }
    }
}
