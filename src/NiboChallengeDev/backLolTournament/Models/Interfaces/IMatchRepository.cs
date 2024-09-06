namespace LolTournament.Models.Interfaces
{
    public interface IMatchRepository
    {
        // Adds a new match 
        Task AddAsync(Match match);

        // Find all matches
        Task<IEnumerable<Match>> GetAllAsync();  

        // Find a match by ID
        Task<Match> GetByIdAsync(int id); 

        // Updates an match
        Task UpdateAsync(Match match);

        // Deletes a match by ID
        Task DeleteAsync(int id);
    }
}
