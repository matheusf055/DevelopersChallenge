using AutoMapper;
using LolTournament.Application.DTOs;
using LolTournament.Models;
using LolTournament.Models.Interfaces;

namespace LolTournament.Application.Services
{
    public class MatchService
    {
        private readonly IMatchRepository _matchRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly ITournamentRepository _tournamentRepository;
        private readonly IMapper _mapper;
        private readonly Random _random = new Random(); 

        // Constructor
        public MatchService(IMatchRepository matchRepository, ITeamRepository teamRepository, ITournamentRepository tournamentRepository, IMapper mapper)
        {
            _matchRepository = matchRepository;
            _teamRepository = teamRepository;
            _tournamentRepository = tournamentRepository;
            _mapper = mapper;
        }

        // Adds a new match
        public async Task<MatchResponseDto> AddAsync(MatchCreateDto matchCreateDto)
        {
            if (matchCreateDto == null)
            {
                // Throw an exception if the DTO is null
                throw new ArgumentNullException(nameof(matchCreateDto));
            }

            try
            {
                // Fetch related entities teams and tournament
                var teamA = await _teamRepository.GetByIdAsync(matchCreateDto.TeamAId);
                var teamB = await _teamRepository.GetByIdAsync(matchCreateDto.TeamBId);
                var tournament = await _tournamentRepository.GetByIdAsync(matchCreateDto.TournamentId);

                // Check if all entities exist
                if (teamA == null || teamB == null || tournament == null)
                {
                    throw new KeyNotFoundException("One of the related entities was not found.");
                }

                // Determine the winner randomly
                int winnerId = DetermineRandomWinner(teamA.Id, teamB.Id);

                // Create a new match
                var match = new Match
                {
                    TeamA = teamA,
                    TeamB = teamB,
                    Tournament = tournament,
                    WinnerId = winnerId // Set the winner based on random selection
                };

                // Add the match
                await _matchRepository.AddAsync(match);

                // Map the added match to MatchResponseDto and return
                return _mapper.Map<MatchResponseDto>(match);
            }
            catch (Exception ex)
            {
                // Handle general exceptions in ApplicationException
                throw new ApplicationException("An error occurred while adding the match.", ex);
            }
        }

        // Finds all matches
        public async Task<IEnumerable<MatchResponseDto>> GetAllAsync()
        {
            try
            {
                // Fetch all matches
                var matches = await _matchRepository.GetAllAsync();
                // Map the collection of matches to a collection of MatchResponseDto and return
                return _mapper.Map<IEnumerable<MatchResponseDto>>(matches);
            }
            catch (Exception ex)
            {
                // Handle general exceptions in ApplicationException
                throw new ApplicationException("An error occurred while retrieving matches.", ex);
            }
        }

        // Finds a match by ID
        public async Task<MatchResponseDto> GetByIdAsync(int id)
        {
            try
            {
                // Fetch the match
                var match = await _matchRepository.GetByIdAsync(id);
                if (match == null)
                {
                    // Throw an exception if match is not found
                    throw new KeyNotFoundException($"Match with ID {id} not found.");
                }
                // Map the match entity to MatchResponseDto and return
                return _mapper.Map<MatchResponseDto>(match);
            }
            catch (KeyNotFoundException ex)
            {
                // Handle specific case where the match was not found
                throw new ApplicationException("Match not found.", ex);
            }
            catch (Exception ex)
            {
                // Handle general exceptions in ApplicationException
                throw new ApplicationException("An error occurred while retrieving the match.", ex);
            }
        }

        // Updates a match
        public async Task UpdateAsync(MatchUpdateDto matchUpdateDto)
        {
            if (matchUpdateDto == null)
            {
                // Throw an exception if the DTO is null
                throw new ArgumentNullException(nameof(matchUpdateDto));
            }

            try
            {
                // Map the MatchUpdateDto to a Match entity
                var match = _mapper.Map<Match>(matchUpdateDto);
                // Update the match
                await _matchRepository.UpdateAsync(match);
            }
            catch (Exception ex)
            {
                // Handle general exceptions in ApplicationException
                throw new ApplicationException("An error occurred while updating the match.", ex);
            }
        }

        // Deletes a match by ID
        public async Task DeleteAsync(int id)
        {
            try
            {
                // Fetch the match to be deleted
                var match = await _matchRepository.GetByIdAsync(id);
                if (match == null)
                {
                    // Throw an exception if match is not found
                    throw new KeyNotFoundException($"Match with ID {id} not found.");
                }

                // Delete the match from the repository
                await _matchRepository.DeleteAsync(id);
            }
            catch (KeyNotFoundException ex)
            {
                // Handle specific case where the match was not found
                throw new ApplicationException("Match not found for deletion.", ex);
            }
            catch (Exception ex)
            {
                // Handle general exceptions in ApplicationException
                throw new ApplicationException("An error occurred while deleting the match.", ex);
            }
        }

        // Determine the winner randomly
        private int DetermineRandomWinner(int teamAId, int teamBId)
        {
            // Return a randomly selected winner
            return _random.Next(2) == 0 ? teamAId : teamBId;
        }

    }
}
