using Microsoft.EntityFrameworkCore;
using LolTournament.Data;
using LolTournament.Models;
using LolTournament.Data.Repositories;

namespace LolTournament.Tests
{
    public class TournamentRepositoryTest
    {
        private readonly DatabaseContext _context;
        private readonly TournamentRepository _repository;

        public TournamentRepositoryTest()
        {
            // Configure the context to use an in-memory database for testing
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            _context = new DatabaseContext(options);
            _repository = new TournamentRepository(_context);
        }

        [Fact]
        public async Task AddAsync_ShouldAddTournament()
        {
            // Arrange: Create a new Tournament instance to add to the repository
            var tournament = new Tournament { Name = "Tournament A" };

            // Act: Add the tournament to the repository and retrieve it from the database
            await _repository.AddAsync(tournament);
            var addedTournament = await _context.Tournaments.FindAsync(tournament.Id);

            // Assert: Verify that the tournament was added correctly to the database
            Assert.NotNull(addedTournament);
            Assert.Equal(tournament.Name, addedTournament.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnTournament()
        {
            // Arrange: Create and add a tournament to the repository
            var tournament = new Tournament { Name = "Tournament A" };
            await _repository.AddAsync(tournament);

            // Act: Retrieve the tournament by its ID
            var retrievedTournament = await _repository.GetByIdAsync(tournament.Id);

            // Assert: Verify that the tournament was retrieved correctly from the database
            Assert.NotNull(retrievedTournament);
            Assert.Equal(tournament.Name, retrievedTournament.Name);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateTournament()
        {
            // Arrange: Create and add a tournament to the repository
            var tournament = new Tournament { Name = "Tournament A" };
            await _repository.AddAsync(tournament);

            // Act: Update the tournament's name and save the changes
            tournament.Name = "Updated Tournament A";
            await _repository.UpdateAsync(tournament);
            var updatedTournament = await _repository.GetByIdAsync(tournament.Id);

            // Assert: Verify that the tournament was updated correctly in the database
            Assert.NotNull(updatedTournament);
            Assert.Equal("Updated Tournament A", updatedTournament.Name);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveTournament()
        {
            // Arrange: Create and add a tournament to the repository
            var tournament = new Tournament { Name = "Tournament A" };
            await _repository.AddAsync(tournament);

            // Act: Remove the tournament from the repository and try to retrieve it
            await _repository.DeleteAsync(tournament.Id);
            var deletedTournament = await _context.Tournaments.FindAsync(tournament.Id);

            // Assert: Verify that the tournament was removed correctly from the database
            Assert.Null(deletedTournament);
        }
    }
}
