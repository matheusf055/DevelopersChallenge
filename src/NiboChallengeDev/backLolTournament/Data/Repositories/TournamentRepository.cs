using LolTournament.Models;
using LolTournament.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LolTournament.Data.Repositories
{
    public class TournamentRepository : ITournamentRepository
    {
        private readonly DatabaseContext _context;

        // Constructor to initialize the repository with an instance of DatabaseContext
        public TournamentRepository(DatabaseContext context)
        {
            _context = context;
        }

        // Adds a new tournament 
        public async Task AddAsync(Tournament tournament)
        {
            // Adds the new tournament to the context
            await _context.Tournaments.AddAsync(tournament);
            // Saves changes to the database
            await _context.SaveChangesAsync();
        }

        // Find all tournaments
        public async Task<IEnumerable<Tournament>> GetAllAsync()
        {
            // Uses ToListAsync to get a list of all tournaments
            return await _context.Tournaments.ToListAsync();
        }

        // Find a tournament by ID
        public async Task<Tournament> GetByIdAsync(int id)
        {
            // Uses FindAsync to locate the tournament by ID
            return await _context.Tournaments.FindAsync(id);
        }

        // Updates existing tournament
        public async Task UpdateAsync(Tournament tournament)
        {
            // Marks the tournament as modified
            _context.Tournaments.Update(tournament);
            // Saves changes to the database
            await _context.SaveChangesAsync();
        }

        // Deletes a tournament by ID
        public async Task DeleteAsync(int id)
        {
            // Retrieves the tournament by ID
            var tournament = await GetByIdAsync(id);
            if (tournament != null)  // Checks if the tournament exists
            {
                // Removes the tournament from the context
                _context.Tournaments.Remove(tournament);
                // Saves changes to the database
                await _context.SaveChangesAsync();
            }
            // If the tournament is not found, do nothing (silent failure)
        }
    }
}
