using AutoMapper;
using LolTournament.Application.DTOs;
using LolTournament.Models;
using LolTournament.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace LolTournament.Application.Services
{
    public class TournamentService
    {
        private readonly ITournamentRepository _tournamentRepository;
        private readonly IMatchRepository _matchRepository;
        private readonly IMapper _mapper;

        // Constructor
        public TournamentService(ITournamentRepository tournamentRepository, IMatchRepository matchRepository, IMapper mapper)
        {
            _tournamentRepository = tournamentRepository;
            _matchRepository = matchRepository; 
            _mapper = mapper;
        }

        // Add a new tournament
        public async Task<int> AddAsync(TournamentCreateDto tournamentRequest)
        {
            if (tournamentRequest == null)
            {
                throw new ArgumentNullException(nameof(tournamentRequest));
            }

            try
            {
                var tournament = _mapper.Map<Tournament>(tournamentRequest);
                await _tournamentRepository.AddAsync(tournament);

                // Determine the winner after creating the tournament
                var winnerId = await DetermineWinnerAsync(tournament.Id);
                if (winnerId.HasValue)
                {
                    tournament.WinnerId = winnerId.Value;
                    await _tournamentRepository.UpdateAsync(tournament);
                }

                return tournament.Id;
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is MySqlException sqlException && sqlException.Number == 1062)
                {
                    throw new InvalidOperationException("A tournament with the same name already exists.");
                }
                throw new ApplicationException("An error occurred while adding the tournament.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An unexpected error occurred while adding the tournament.", ex);
            }
        }

        // Determine the winner of the tournament
        public async Task<int?> DetermineWinnerAsync(int tournamentId)
        {
            try
            {
                // Get all matches for the tournament
                var matches = await _matchRepository.GetAllAsync();
                var tournamentMatches = matches.Where(m => m.Tournament.Id == tournamentId);

                // Ensure there are matches for the tournament
                if (!tournamentMatches.Any())
                {
                    return null; // No matches found, hence no winner
                }

                // Assuming the last match determines the winner
                var lastMatch = tournamentMatches
                    .FirstOrDefault();

                // Ensure the last match has a winner
                if (lastMatch?.WinnerId != null)
                {
                    return lastMatch.WinnerId;
                }

                return null; // No winner in the last match
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while determining the tournament winner.", ex);
            }
        }

        // Find all tournaments
        public async Task<IEnumerable<TournamentResponseDto>> GetAllAsync()
        {
            try
            {
                var tournaments = await _tournamentRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<TournamentResponseDto>>(tournaments);
            }
            catch (Exception ex)
            {
                // Handling errors
                throw new ApplicationException("An error occurred while retrieving tournaments.", ex);
            }
        }

        // Find a tournament by ID
        public async Task<TournamentResponseDto> GetByIdAsync(int id)
        {
            try
            {
                var tournament = await _tournamentRepository.GetByIdAsync(id);
                if (tournament == null)
                {
                    // Throwing a KeyNotFoundException if the tournament is not found
                    throw new KeyNotFoundException($"Tournament with ID {id} not found.");
                }
                return _mapper.Map<TournamentResponseDto>(tournament);
            }
            catch (KeyNotFoundException ex)
            {
                // Rethrowing a more generic ApplicationException for consistency
                throw new ApplicationException("Tournament not found.", ex);
            }
            catch (Exception ex)
            {
                // Handling errors
                throw new ApplicationException("An error occurred while retrieving the tournament.", ex);
            }
        }

        // Update a tournament
        public async Task UpdateAsync(TournamentUpdateDto tournamentRequest)
        {
            if (tournamentRequest == null)
            {
                // Throwing an ArgumentNullException if the request DTO is null
                throw new ArgumentNullException(nameof(tournamentRequest));
            }

            try
            {
                var tournament = _mapper.Map<Tournament>(tournamentRequest);
                await _tournamentRepository.UpdateAsync(tournament);
            }
            catch (Exception ex)
            {
                // Handling errors
                throw new ApplicationException("An error occurred while updating the tournament.", ex);
            }
        }

        // Delete a tournament by ID
        public async Task DeleteAsync(int id)
        {
            try
            {
                var tournament = await _tournamentRepository.GetByIdAsync(id);
                if (tournament == null)
                {
                    // Throwing a KeyNotFoundException if the tournament is not found
                    throw new KeyNotFoundException($"Tournament with ID {id} not found.");
                }

                await _tournamentRepository.DeleteAsync(id);
            }
            catch (KeyNotFoundException ex)
            {
                // Rethrowing a more generic ApplicationException for consistency
                throw new ApplicationException("Tournament not found for deletion.", ex);
            }
            catch (Exception ex)
            {
                // Handling errors
                throw new ApplicationException("An error occurred while deleting the tournament.", ex);
            }
        }
    }
}
