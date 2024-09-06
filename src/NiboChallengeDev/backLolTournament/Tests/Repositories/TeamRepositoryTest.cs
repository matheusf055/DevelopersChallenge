using Microsoft.EntityFrameworkCore;
using LolTournament.Data;
using LolTournament.Models;
using LolTournament.Data.Repositories;
using Xunit;

namespace LolTournament.Tests
{
    public class TeamRepositoryTest
    {
        private readonly DatabaseContext _context;
        private readonly TeamRepository _repository;

        public TeamRepositoryTest()
        {
            // Configure the context to use an in-memory database for testing
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            _context = new DatabaseContext(options);
            _repository = new TeamRepository(_context);
        }

        [Fact]
        public async Task AddAsync_ShouldAddTeam()
        {
            // Arrange: Create a new team with the required property
            var team = new Team { Name = "Team A", Region = "Region A" };

            // Act: Add the team to the repository and retrieve it from the database
            await _repository.AddAsync(team);
            var addedTeam = await _context.Teams.FindAsync(team.Id);

            // Assert: Verify that the team was added correctly
            Assert.NotNull(addedTeam);
            Assert.Equal(team.Name, addedTeam.Name);
            Assert.Equal(team.Region, addedTeam.Region);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnTeam()
        {
            // Arrange: Create and add a team to the repository
            var team = new Team { Name = "Team A", Region = "Region A" };
            await _repository.AddAsync(team);

            // Act: Retrieve the team by its ID
            var retrievedTeam = await _repository.GetByIdAsync(team.Id);

            // Assert: Verify that the team was retrieved correctly
            Assert.NotNull(retrievedTeam);
            Assert.Equal(team.Name, retrievedTeam.Name);
            Assert.Equal(team.Region, retrievedTeam.Region);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateTeam()
        {
            // Arrange: Create and add a team to the repository
            var team = new Team { Name = "Team A", Region = "Region A" };
            await _repository.AddAsync(team);

            // Act: Update the team's name and save the changes
            team.Name = "Updated Team A";
            await _repository.UpdateAsync(team);
            var updatedTeam = await _repository.GetByIdAsync(team.Id);

            // Assert: Verify that the team was updated correctly
            Assert.NotNull(updatedTeam);
            Assert.Equal("Updated Team A", updatedTeam.Name);
            Assert.Equal(team.Region, updatedTeam.Region); // Verify that the Region is still correct
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveTeam()
        {
            // Arrange: Create and add a team to the repository
            var team = new Team { Name = "Team A", Region = "Region A" };
            await _repository.AddAsync(team);

            // Act: Remove the team from the repository and try to retrieve it
            await _repository.DeleteAsync(team.Id);
            var deletedTeam = await _context.Teams.FindAsync(team.Id);

            // Assert: Verify that the team was removed correctly
            Assert.Null(deletedTeam);
        }
    }
}
