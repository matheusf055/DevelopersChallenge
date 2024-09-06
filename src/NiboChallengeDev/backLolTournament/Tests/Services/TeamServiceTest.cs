using AutoMapper;
using Moq;
using LolTournament.Application.DTOs;
using LolTournament.Application.Services;
using LolTournament.Models;
using LolTournament.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LolTournament.Tests.Services
{
    public class TeamServiceTest
    {
        private readonly TeamService _teamService;
        private readonly Mock<ITeamRepository> _mockTeamRepository;
        private readonly IMapper _mapper;

        public TeamServiceTest()
        {
            // Initialize the mock repository
            _mockTeamRepository = new Mock<ITeamRepository>();

            // Configure AutoMapper with mappings for Team and DTOs
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Team, TeamResponseDto>(); // Map Team to TeamResponseDto
                cfg.CreateMap<TeamCreateDto, Team>(); // Map TeamCreateDto to Team
                cfg.CreateMap<TeamUpdateDto, Team>(); // Map TeamUpdateDto to Team
            });

            _mapper = mapperConfig.CreateMapper();
            _teamService = new TeamService(_mockTeamRepository.Object, _mapper);
        }

        [Fact]
        public async Task AddAsync_ValidTeamCreateDto_ReturnsTeamResponseDto()
        {
            // Arrange
            var teamCreateDto = new TeamCreateDto { Name = "New Team" };
            var team = new Team { Id = 1, Name = "New Team" };

            // Setup mock repository methods to simulate adding a team and retrieving it
            _mockTeamRepository.Setup(repo => repo.AddAsync(It.IsAny<Team>())).Returns(Task.CompletedTask);
            _mockTeamRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(team);

            // Act
            var result = await _teamService.AddAsync(teamCreateDto);

            // Assert
            Assert.NotNull(result); // Ensure the result is not null
            Assert.Equal("New Team", result.Name); // Verify the result has the expected name
        }

        [Fact]
        public async Task AddAsync_DuplicateTeam_ThrowsApplicationException()
        {
            // Arrange
            var teamCreateDto = new TeamCreateDto { Name = "Existing Team" };
            var sqlException = new Exception("Duplicate key");
            var dbUpdateException = new DbUpdateException("Duplicate key", sqlException);

            // Simulate a database exception for a duplicate key
            _mockTeamRepository.Setup(repo => repo.AddAsync(It.IsAny<Team>())).ThrowsAsync(dbUpdateException);

            // Act and Assert
            var ex = await Assert.ThrowsAsync<DbUpdateException>(() => _teamService.AddAsync(teamCreateDto));
            Assert.Equal("Duplicate key", ex.Message); // Verify the exception message
        }

        [Fact]
        public async Task AddAsync_NullTeamCreateDto_ThrowsArgumentNullException()
        {
            // Act and Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _teamService.AddAsync(null));
        }

        [Fact]
        public async Task GetByIdAsync_ValidId_ReturnsTeamResponseDto()
        {
            // Arrange
            var team = new Team { Id = 1, Name = "Test Team" };
            _mockTeamRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(team);

            // Act
            var result = await _teamService.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result); // Ensure the result is not null
            Assert.Equal(1, result.Id); // Verify the result has the expected ID
            Assert.Equal("Test Team", result.Name); // Verify the result has the expected name
        }

        [Fact]
        public async Task GetByIdAsync_InvalidId_ThrowsApplicationException()
        {
            // Arrange
            // Simulate no team found for the given ID
            _mockTeamRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Team)null);

            // Act and Assert
            await Assert.ThrowsAsync<ApplicationException>(() => _teamService.GetByIdAsync(999));
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllTeamResponseDtos()
        {
            // Arrange
            var teams = new List<Team>
            {
                new Team { Id = 1, Name = "Team A" },
                new Team { Id = 2, Name = "Team B" }
            };

            // Setup mock repository to return the list of teams
            _mockTeamRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(teams);

            // Act
            var result = await _teamService.GetAllAsync();

            // Assert
            Assert.NotNull(result); // Ensure the result is not null
            Assert.Equal(2, result.Count()); // Verify the number of results
            Assert.Contains(result, t => t.Name == "Team A"); // Verify "Team A" is in the result
            Assert.Contains(result, t => t.Name == "Team B"); // Verify "Team B" is in the result
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmptyList()
        {
            // Arrange
            var teams = new List<Team>();

            // Setup mock repository to return an empty list
            _mockTeamRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(teams);

            // Act
            var result = await _teamService.GetAllAsync();

            // Assert
            Assert.NotNull(result); // Ensure the result is not null
            Assert.Empty(result); // Verify the result is an empty list
        }

        [Fact]
        public async Task UpdateAsync_ValidTeamUpdateDto_UpdatesTeam()
        {
            // Arrange
            var teamUpdateDto = new TeamUpdateDto { Id = 1, Name = "Updated Team" };
            _mockTeamRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Team>())).Returns(Task.CompletedTask);

            // Act
            await _teamService.UpdateAsync(teamUpdateDto);

            // Assert
            _mockTeamRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Team>()), Times.Once); // Verify the update method was called once
        }

        [Fact]
        public async Task UpdateAsync_NullTeamUpdateDto_ThrowsArgumentNullException()
        {
            // Act and Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _teamService.UpdateAsync(null));
        }

        [Fact]
        public async Task DeleteAsync_ValidId_DeletesTeam()
        {
            // Arrange
            var team = new Team { Id = 1, Name = "Team to Delete" };

            // Setup mock repository to return the team for deletion and complete the delete operation
            _mockTeamRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(team);
            _mockTeamRepository.Setup(repo => repo.DeleteAsync(1)).Returns(Task.CompletedTask);

            // Act
            await _teamService.DeleteAsync(1);

            // Assert
            _mockTeamRepository.Verify(repo => repo.DeleteAsync(1), Times.Once); // Verify the delete method was called once
        }

        [Fact]
        public async Task DeleteAsync_InvalidId_ThrowsApplicationException()
        {
            // Arrange
            // Simulate no team found for the given ID
            _mockTeamRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Team)null);

            // Act and Assert
            await Assert.ThrowsAsync<ApplicationException>(() => _teamService.DeleteAsync(999));
        }
    }
}
