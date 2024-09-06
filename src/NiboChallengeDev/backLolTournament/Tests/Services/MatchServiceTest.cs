using Moq;
using AutoMapper;
using LolTournament.Application.DTOs;
using LolTournament.Application.Services;
using LolTournament.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LolTournament.Tests.Services
{
    public class MatchServiceTest
    {
        private readonly MatchService _matchService;
        private readonly Mock<IMatchRepository> _mockMatchRepository;
        private readonly Mock<ITeamRepository> _mockTeamRepository;
        private readonly Mock<ITournamentRepository> _mockTournamentRepository;
        private readonly IMapper _mapper;

        public MatchServiceTest()
        {
            // Initialize mocks for the repositories
            _mockMatchRepository = new Mock<IMatchRepository>();
            _mockTeamRepository = new Mock<ITeamRepository>();
            _mockTournamentRepository = new Mock<ITournamentRepository>();

            // Configure AutoMapper
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                // Map Match to MatchResponseDto
                cfg.CreateMap<Models.Match, MatchResponseDto>()
                    .ForMember(dest => dest.Tournament, opt => opt.MapFrom(src => src.Tournament));

                // Map DTOs to Match
                cfg.CreateMap<MatchCreateDto, Models.Match>();
                cfg.CreateMap<MatchUpdateDto, Models.Match>();

                // Additional mappings for related entities
                cfg.CreateMap<Models.Team, TeamResponseDto>();
                cfg.CreateMap<Models.Tournament, TournamentResponseDto>();
            });

            _mapper = mapperConfig.CreateMapper();
            _matchService = new MatchService(_mockMatchRepository.Object, _mockTeamRepository.Object, _mockTournamentRepository.Object, _mapper);
        }

        [Fact]
        public async Task AddAsync_ValidMatchCreateDto_ReturnsMatchResponseDto()
        {
            // Arrange
            var matchCreateDto = new MatchCreateDto
            {
                TeamAId = 1,
                TeamBId = 2,
                TournamentId = 3,
            };

            // Create mock data for teams, tournament
            var teamA = new Models.Team { Id = 1 };
            var teamB = new Models.Team { Id = 2 };
            var tournament = new Models.Tournament { Id = 3 };

            // Setup repository mocks to return the mock data
            _mockTeamRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(teamA);
            _mockTeamRepository.Setup(repo => repo.GetByIdAsync(2)).ReturnsAsync(teamB);
            _mockTournamentRepository.Setup(repo => repo.GetByIdAsync(3)).ReturnsAsync(tournament);

            // Create a new match entity
            var match = new Models.Match
            {
                TeamA = teamA,
                TeamB = teamB,
                Tournament = tournament,
            };

            // Mock the repository's GetByIdAsync method to return the created match
            _mockMatchRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(match);

            // Act
            var result = await _matchService.AddAsync(matchCreateDto);

            // Assert
            Assert.NotNull(result); // Ensure the result is not null
        }

        [Fact]
        public async Task AddAsync_DuplicateMatch_ThrowsApplicationException()
        {
            // Arrange
            var matchCreateDto = new MatchCreateDto
            {
                TeamAId = 1,
                TeamBId = 2,
                TournamentId = 3,
            };

            // Simulate a database exception for a duplicate key
            var sqlException = new Exception("Duplicate key");
            var dbUpdateException = new DbUpdateException("Duplicate key", sqlException);

            // Setup repository to throw the exception when AddAsync is called
            _mockMatchRepository.Setup(repo => repo.AddAsync(It.IsAny<Models.Match>())).ThrowsAsync(dbUpdateException);

            // Act and Assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() => _matchService.AddAsync(matchCreateDto));
            Assert.Equal("An error occurred while adding the match.", ex.Message);
        }

        [Fact]
        public async Task AddAsync_NullMatchCreateDto_ThrowsArgumentNullException()
        {
            // Act and Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _matchService.AddAsync(null));
        }

        [Fact]
        public async Task GetByIdAsync_ValidId_ReturnsMatchResponseDto()
        {
            // Arrange
            var match = new Models.Match { Id = 1 };

            // Setup repository to return the match when GetByIdAsync is called with ID 1
            _mockMatchRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(match);

            // Act
            var result = await _matchService.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result); // Ensure the result is not null
            Assert.Equal(1, result.Id); // Ensure the result has the correct ID
        }

        [Fact]
        public async Task GetByIdAsync_InvalidId_ThrowsApplicationException()
        {
            // Arrange
            // Setup repository to return null when GetByIdAsync is called with any ID
            _mockMatchRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Models.Match)null);

            // Act and Assert
            await Assert.ThrowsAsync<ApplicationException>(() => _matchService.GetByIdAsync(1));
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllMatchResponseDtos()
        {
            // Arrange
            var matches = new List<Models.Match>
            {
                new Models.Match { Id = 1 },
                new Models.Match { Id = 2 }
            };

            // Setup repository to return the list of matches
            _mockMatchRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(matches);

            // Act
            var result = await _matchService.GetAllAsync();

            // Assert
            Assert.NotNull(result); // Ensure the result is not null
            Assert.Equal(2, result.Count()); // Ensure the result contains the expected number of items
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmptyList()
        {
            // Arrange
            var matches = new List<Models.Match>();

            // Setup repository to return an empty list
            _mockMatchRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(matches);

            // Act
            var result = await _matchService.GetAllAsync();

            // Assert
            Assert.NotNull(result); // Ensure the result is not null
            Assert.Empty(result); // Ensure the result is an empty list
        }

        [Fact]
        public async Task UpdateAsync_ValidMatchUpdateDto_UpdatesMatch()
        {
            // Arrange
            var matchUpdateDto = new MatchUpdateDto
            {
                Id = 1,
                TeamAId = 1,
                TeamBId = 2,
                TournamentId = 3,
            };

            // Setup repository to successfully complete the update operation
            _mockMatchRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Models.Match>())).Returns(Task.CompletedTask);

            // Act
            await _matchService.UpdateAsync(matchUpdateDto);

            // Assert
            // Verify that UpdateAsync was called exactly once
            _mockMatchRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Models.Match>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_NullMatchUpdateDto_ThrowsArgumentNullException()
        {
            // Act and Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _matchService.UpdateAsync(null));
        }

        [Fact]
        public async Task DeleteAsync_ValidId_DeletesMatch()
        {
            // Arrange
            var match = new Models.Match { Id = 1 };

            // Setup repository to return the match for deletion and complete the delete operation
            _mockMatchRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(match);
            _mockMatchRepository.Setup(repo => repo.DeleteAsync(1)).Returns(Task.CompletedTask);

            // Act
            await _matchService.DeleteAsync(1);

            // Assert
            // Verify that DeleteAsync was called exactly once
            _mockMatchRepository.Verify(repo => repo.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_InvalidId_ThrowsApplicationException()
        {
            // Arrange
            // Setup repository to return null when GetByIdAsync is called with any ID
            _mockMatchRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Models.Match)null);

            // Act and Assert
            await Assert.ThrowsAsync<ApplicationException>(() => _matchService.DeleteAsync(999));
        }
    }
}
