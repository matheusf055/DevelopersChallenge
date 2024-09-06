using AutoMapper;
using Moq;
using LolTournament.Application.DTOs;
using LolTournament.Application.Services;
using LolTournament.Models;
using LolTournament.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LolTournament.Tests.Services
{
    public class TournamentServiceTest
    {
        private readonly TournamentService _tournamentService;
        private readonly Mock<ITournamentRepository> _mockTournamentRepository;
        private readonly Mock<IMatchRepository> _mockMatchRepository; // Adicionado
        private readonly IMapper _mapper;

        public TournamentServiceTest()
        {
            // Initialize the mock repositories
            _mockTournamentRepository = new Mock<ITournamentRepository>();
            _mockMatchRepository = new Mock<IMatchRepository>(); // Inicializado

            // Configure AutoMapper with mappings for Tournament and DTOs
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Tournament, TournamentResponseDto>(); // Map Tournament to TournamentResponseDto
                cfg.CreateMap<TournamentCreateDto, Tournament>(); // Map TournamentCreateDto to Tournament
                cfg.CreateMap<TournamentUpdateDto, Tournament>(); // Map TournamentUpdateDto to Tournament
            });

            _mapper = mapperConfig.CreateMapper();
            _tournamentService = new TournamentService(_mockTournamentRepository.Object, _mockMatchRepository.Object, _mapper); // Corrigido
        }

        [Fact]
        public async Task GetByIdAsync_ValidId_ReturnsTournamentResponseDto()
        {
            // Arrange
            var tournament = new Tournament { Id = 1, Name = "Test Tournament" };
            _mockTournamentRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(tournament);

            // Act
            var result = await _tournamentService.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result); // Ensure the result is not null
            Assert.Equal(1, result.Id); // Verify the result has the expected ID
            Assert.Equal("Test Tournament", result.Name); // Verify the result has the expected name
        }

        [Fact]
        public async Task AddAsync_ValidTournamentCreateDto_ReturnsTournamentId()
        {
            // Arrange
            var tournamentCreateDto = new TournamentCreateDto { Name = "New Tournament" };
            var expectedId = 1;
            var tournament = new Tournament { Id = expectedId, Name = "New Tournament" };

            // Setup mock repository to simulate ID assignment and return expected tournament
            _mockTournamentRepository.Setup(repo => repo.AddAsync(It.IsAny<Tournament>()))
                .Callback<Tournament>(t => t.Id = expectedId) // Assign ID in the callback
                .Returns(Task.CompletedTask);

            _mockTournamentRepository.Setup(repo => repo.GetByIdAsync(expectedId)).ReturnsAsync(tournament);

            // Act
            var result = await _tournamentService.AddAsync(tournamentCreateDto);

            // Assert
            Assert.Equal(expectedId, result); // Verify the returned ID matches the expected ID
        }

        [Fact]
        public async Task AddAsync_DuplicateTournament_ThrowsInvalidOperationException()
        {
            // Arrange
            var tournamentCreateDto = new TournamentCreateDto { Name = "Existing Tournament" };
            var sqlException = new Exception("Duplicate key");
            var dbUpdateException = new DbUpdateException("Duplicate key", sqlException);

            // Simulate a database exception for duplicate key
            _mockTournamentRepository.Setup(repo => repo.AddAsync(It.IsAny<Tournament>())).ThrowsAsync(dbUpdateException);

            // Act and Assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() => _tournamentService.AddAsync(tournamentCreateDto));
            Assert.Equal("An error occurred while adding the tournament.", ex.Message); // Verify the exception message
        }

        [Fact]
        public async Task AddAsync_NullTournamentCreateDto_ThrowsArgumentNullException()
        {
            // Act and Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _tournamentService.AddAsync(null));
        }

        [Fact]
        public async Task GetByIdAsync_InvalidId_ThrowsApplicationException()
        {
            // Arrange
            _mockTournamentRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Tournament)null); // Simulate a case where no tournament is found for the given ID

            // Act and Assert
            await Assert.ThrowsAsync<ApplicationException>(() => _tournamentService.GetByIdAsync(999)); // Verify that an ApplicationException is thrown
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllTournamentResponseDtos()
        {
            // Arrange
            var tournaments = new List<Tournament>
            {
                new Tournament { Id = 1, Name = "Tournament A" },
                new Tournament { Id = 2, Name = "Tournament B" }
            };

            _mockTournamentRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(tournaments);

            // Act
            var result = await _tournamentService.GetAllAsync();

            // Assert
            Assert.NotNull(result); // Ensure the result is not null
            Assert.Equal(2, result.Count()); // Verify the result contains the correct number of items
            Assert.Contains(result, t => t.Name == "Tournament A"); // Check that Tournament A is included
            Assert.Contains(result, t => t.Name == "Tournament B"); // Check that Tournament B is included
        }

        [Fact]
        public async Task UpdateAsync_ValidTournamentUpdateDto_UpdatesTournament()
        {
            // Arrange
            var tournamentUpdateDto = new TournamentUpdateDto { Id = 1, Name = "Updated Tournament" };
            _mockTournamentRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Tournament>())).Returns(Task.CompletedTask);

            // Act
            await _tournamentService.UpdateAsync(tournamentUpdateDto);

            // Assert
            _mockTournamentRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Tournament>()), Times.Once); // Verify that UpdateAsync was called once
        }

        [Fact]
        public async Task UpdateAsync_NullTournamentUpdateDto_ThrowsArgumentNullException()
        {
            // Act and Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _tournamentService.UpdateAsync(null)); // Verify that ArgumentNullException is thrown
        }

        [Fact]
        public async Task DeleteAsync_ValidId_DeletesTournament()
        {
            // Arrange
            var tournament = new Tournament { Id = 1, Name = "Tournament to Delete" };

            _mockTournamentRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(tournament);
            _mockTournamentRepository.Setup(repo => repo.DeleteAsync(1)).Returns(Task.CompletedTask);

            // Act
            await _tournamentService.DeleteAsync(1);

            // Assert
            _mockTournamentRepository.Verify(repo => repo.DeleteAsync(1), Times.Once); // Verify that DeleteAsync was called once
        }

        [Fact]
        public async Task DeleteAsync_InvalidId_ThrowsApplicationException()
        {
            // Arrange
            _mockTournamentRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Tournament)null); // Simulate a case onde nenhum torneio é encontrado para o ID fornecido

            // Act and Assert
            await Assert.ThrowsAsync<ApplicationException>(() => _tournamentService.DeleteAsync(999)); // Verify that an ApplicationException is thrown
        }
    }
}
