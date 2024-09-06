namespace LolTournament.Models.Interfaces
{
    public interface ITeamRepository
    {
        // Adds a new team 
        Task AddAsync(Team team);

        // Find all teams
        Task<IEnumerable<Team>> GetAllAsync();

        // Find a team by ID
        Task<Team> GetByIdAsync(int id);

        // Updates an team 
        Task UpdateAsync(Team team);

        // Deletes a team by ID
        Task DeleteAsync(int id);
    }
}
