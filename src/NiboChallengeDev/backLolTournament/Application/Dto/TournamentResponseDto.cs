namespace LolTournament.Application.DTOs
{
    public class TournamentResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateOnly StartDate { get; set; }
    }
}
