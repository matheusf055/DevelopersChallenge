using LolTournament.Models;
using LolTournament.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LolTournament.Data.Repositories
{
    public class MatchRepository : IMatchRepository
    {
        private readonly DatabaseContext _context;

        // Constructor to initialize the repository with a DatabaseContext instance
        public MatchRepository(DatabaseContext context)
        {
            _context = context;
        }

        // Adds a new match 
        public async Task AddAsync(Match match)
        {
            // Adds the new match to the context
            await _context.Matches.AddAsync(match);
            // Saves changes to the database
            await _context.SaveChangesAsync();
        }

        // Retrieves all matches
        public async Task<IEnumerable<Match>> GetAllAsync()
        {
            // Fetches all matches from the database including related entities (TeamA, TeamB, Tournament, and Winner)
            return await _context.Matches
                .Include(m => m.TeamA)  // Load the TeamA entity
                .Include(m => m.TeamB)  // Load the TeamB entity
                .Include(m => m.Tournament)  // Load the Tournament entity
                .Include(m => m.Winner)  // Load the Winner entity
                .ToListAsync();  // Retrieve all matches as a list
        }

        // Find a match by ID
        public async Task<Match> GetByIdAsync(int id)
        {
            // Fetches the match from the database including related entities (TeamA, TeamB, Tournament, and Winner)
            return await _context.Matches
                .Include(m => m.TeamA)  // Load the TeamA entity
                .Include(m => m.TeamB)  // Load the TeamB entity
                .Include(m => m.Tournament)  // Load the Tournament entity
                .Include(m => m.Winner)  // Load the Winner entity
                .FirstOrDefaultAsync(m => m.Id == id);  // Find the match with the specified ID
        }

        // Updates an match
        public async Task UpdateAsync(Match match)
        {
            // Updates the match in the context
            _context.Matches.Update(match);
            // Saves changes to the database
            await _context.SaveChangesAsync();
        }

        // Deletes a match by ID
        public async Task DeleteAsync(int id)
        {
            // Retrieves the match to be deleted by its ID
            var match = await GetByIdAsync(id);
            if (match != null)  // Check if the match exists
            {
                // Removes the match from the context
                _context.Matches.Remove(match);
                // Saves changes to the database
                await _context.SaveChangesAsync();
            }
            // If the match is not found, no action is taken (silent fail)
        }
    }
}
