namespace LolTournament.Models.Interfaces
{
    public interface ITournamentRepository
    {
        // Adds a new tournament 
        Task AddAsync(Tournament tournament);

        // Find all tournaments
        Task<IEnumerable<Tournament>> GetAllAsync();

        // Find a tournament by ID
        Task<Tournament> GetByIdAsync(int id);

        // Updates an tournament 
        Task UpdateAsync(Tournament tournament);

        // Deletes a tournament by ID
        Task DeleteAsync(int id);
    }
}
