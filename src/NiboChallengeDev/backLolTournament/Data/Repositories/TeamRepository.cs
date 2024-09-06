using LolTournament.Models;
using LolTournament.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LolTournament.Data.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        private readonly DatabaseContext _context;

        // Constructor to initialize the repository with an instance of DatabaseContext
        public TeamRepository(DatabaseContext context)
        {
            _context = context;
        }

        // Adds a new team 
        public async Task AddAsync(Team team)
        {
            // Adds the new team to the context
            await _context.Teams.AddAsync(team);
            // Saves changes to the database
            await _context.SaveChangesAsync();
        }

        // Retrieves all teams
        public async Task<IEnumerable<Team>> GetAllAsync()
        {
            // Uses ToListAsync to get a list of all teams
            return await _context.Teams.ToListAsync();
        }

        // Find a team by ID
        public async Task<Team> GetByIdAsync(int id)
        {
            // Uses FindAsync to locate the team by ID
            return await _context.Teams.FindAsync(id);
        }

        // Updates existing team
        public async Task UpdateAsync(Team team)
        {
            // Marks the team as modified
            _context.Teams.Update(team);
            // Saves changes to the database
            await _context.SaveChangesAsync();
        }

        // Deletes a team by ID
        public async Task DeleteAsync(int id)
        {
            // Retrieves the team by ID
            var team = await GetByIdAsync(id);
            if (team != null)  // Checks if the team exists
            {
                // Removes the team from the context
                _context.Teams.Remove(team);
                // Saves changes to the database
                await _context.SaveChangesAsync();
            }
            // If the team is not found, do nothing (silent failure)
        }
    }
}
