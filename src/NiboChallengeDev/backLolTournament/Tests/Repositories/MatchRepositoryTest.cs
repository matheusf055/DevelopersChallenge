using Microsoft.EntityFrameworkCore;
using LolTournament.Data;
using LolTournament.Models;
using LolTournament.Data.Repositories;
using Xunit;

namespace LolTournament.Tests
{
    public class MatchRepositoryTests
    {
        private readonly DatabaseContext _context;
        private readonly MatchRepository _repository;

        public MatchRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            _context = new DatabaseContext(options);
            _repository = new MatchRepository(_context);
        }

        [Fact]
        public async Task AddAsync_ShouldAddMatch()
        {
            // Arrange: Set up the necessary test data
            var teamA = new Team { Name = "Team A", Region = "Region A" }; 
            var teamB = new Team { Name = "Team B", Region = "Region B" }; 
            var tournament = new Tournament { Name = "Tournament" };

            // Add the test data to the in-memory database
            await _context.Teams.AddRangeAsync(teamA, teamB);
            await _context.Tournaments.AddAsync(tournament);
            await _context.SaveChangesAsync();

            // Create a new match with the test data
            var match = new Match 
            { 
                TeamAId = teamA.Id, 
                TeamBId = teamB.Id, 
                TournamentId = tournament.Id, 
                WinnerId = teamA.Id, 
            };

            // Act: Add the match to the repository
            await _repository.AddAsync(match);

            // Retrieve the added match from the database to verify
            var addedMatch = await _context.Matches
                .Include(m => m.TeamA)
                .Include(m => m.TeamB)
                .Include(m => m.Tournament)
                .Include(m => m.Winner)
                .FirstOrDefaultAsync(m => m.Id == match.Id); 

            // Assert: Check that the match was added correctly
            Assert.NotNull(addedMatch);
            Assert.Equal(teamA.Id, addedMatch.TeamAId);
            Assert.Equal(teamB.Id, addedMatch.TeamBId);
            Assert.Equal(tournament.Id, addedMatch.TournamentId);
            Assert.Equal(teamA.Id, addedMatch.WinnerId);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnMatch()
        {
            // Arrange: Set up the necessary test data
            var teamA = new Team { Name = "Team A", Region = "Region A" }; 
            var teamB = new Team { Name = "Team B", Region = "Region B" }; 
            var tournament = new Tournament { Name = "Tournament" };
            
            // Add the test data to the in-memory database
            await _context.Teams.AddRangeAsync(teamA, teamB);
            await _context.Tournaments.AddAsync(tournament);
            await _context.SaveChangesAsync();

            // Create and add a new match to the repository
            var match = new Match 
            { 
                TeamAId = teamA.Id, 
                TeamBId = teamB.Id, 
                TournamentId = tournament.Id, 
                WinnerId = teamA.Id, 
            };

            await _repository.AddAsync(match);

            // Act: Retrieve the match by its ID
            var retrievedMatch = await _repository.GetByIdAsync(match.Id);

            // Assert: Verify that the retrieved match matches the expected data
            Assert.NotNull(retrievedMatch);
            Assert.Equal(teamA.Id, retrievedMatch.TeamAId);
            Assert.Equal(teamB.Id, retrievedMatch.TeamBId);
            Assert.Equal(tournament.Id, retrievedMatch.TournamentId);
            Assert.Equal(teamA.Id, retrievedMatch.WinnerId);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateMatch()
        {
            // Arrange: Set up the necessary test data
            var teamA = new Team { Name = "Team A", Region = "Region A" }; 
            var teamB = new Team { Name = "Team B", Region = "Region B" }; 
            var tournament = new Tournament { Name = "Tournament" };

            // Add the test data to the in-memory database
            await _context.Teams.AddRangeAsync(teamA, teamB);
            await _context.Tournaments.AddAsync(tournament);
            await _context.SaveChangesAsync();

            // Create and add a new match to the repository
            var match = new Match 
            { 
                TeamAId = teamA.Id, 
                TeamBId = teamB.Id, 
                TournamentId = tournament.Id, 
                WinnerId = teamA.Id, 
            };

            await _repository.AddAsync(match);

            // Act: Update the match's details and save changes
            match.WinnerId = teamB.Id; // Example of updating a property
            await _repository.UpdateAsync(match);

            // Retrieve the updated match from the repository
            var updatedMatch = await _repository.GetByIdAsync(match.Id);

            // Assert: Verify that the match was updated correctly
            Assert.NotNull(updatedMatch);
            Assert.Equal(match.WinnerId, updatedMatch.WinnerId);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveMatch()
        {
            // Arrange: Set up the necessary test data
            var teamA = new Team { Name = "Team A", Region = "Region A" }; 
            var teamB = new Team { Name = "Team B", Region = "Region B" }; 
            var tournament = new Tournament { Name = "Tournament" };

            // Add the test data to the in-memory database
            await _context.Teams.AddRangeAsync(teamA, teamB);
            await _context.Tournaments.AddAsync(tournament);
            await _context.SaveChangesAsync();

            // Create and add a new match to the repository
            var match = new Match 
            { 
                TeamAId = teamA.Id, 
                TeamBId = teamB.Id, 
                TournamentId = tournament.Id, 
                WinnerId = teamA.Id, 
            };

            await _repository.AddAsync(match);

            // Act: Delete the match from the repository
            await _repository.DeleteAsync(match.Id);

            // Retrieve the match to verify it was deleted
            var deletedMatch = await _context.Matches.FindAsync(match.Id);

            // Assert: Verify that the match has been removed
            Assert.Null(deletedMatch);
        }
    }
}
