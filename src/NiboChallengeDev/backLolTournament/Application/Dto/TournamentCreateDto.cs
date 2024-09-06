namespace LolTournament.Application.DTOs
{
    public class TournamentCreateDto
    {
        public string Name { get; set; }
        public DateOnly StartDate { get; set; }
        public int? WinnerId { get; set; }
    }
}