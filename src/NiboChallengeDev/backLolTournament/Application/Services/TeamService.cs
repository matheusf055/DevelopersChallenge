using AutoMapper;
using LolTournament.Application.DTOs;
using LolTournament.Models;
using LolTournament.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace LolTournament.Application.Services
{
    public class TeamService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IMapper _mapper;

        // Constructor
        public TeamService(ITeamRepository teamRepository, IMapper mapper)
        {
            _teamRepository = teamRepository;
            _mapper = mapper;
        }

        // Adds a new team
        public async Task<TeamResponseDto> AddAsync(TeamCreateDto teamCreateDto)
        {
            if (teamCreateDto == null)
            {
                // Throw an exception if the DTO is null
                throw new ArgumentNullException(nameof(teamCreateDto));
            }

            try
            {
                // Map DTO to entity and add to repository
                var team = _mapper.Map<Team>(teamCreateDto);
                await _teamRepository.AddAsync(team);
                return _mapper.Map<TeamResponseDto>(team);
            }
            catch (DbUpdateException ex)
            {
                // Handle unique constraint violation 
                if (ex.InnerException is MySqlException sqlException && sqlException.Number == 1062)
                {
                    throw new InvalidOperationException("A team with the same name already exists.");
                }
                throw;
            }
            catch (Exception ex)
            {
                // Handle errors
                throw new ApplicationException("An error occurred while adding the team.", ex);
            }
        }

        // Find all teams
        public async Task<IEnumerable<TeamResponseDto>> GetAllAsync()
        {
            try
            {
                var teams = await _teamRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<TeamResponseDto>>(teams);
            }
            catch (Exception ex)
            {
                // Handle errors
                throw new ApplicationException("An error occurred while retrieving teams.", ex);
            }
        }

        // Find a team by ID
        public async Task<TeamResponseDto> GetByIdAsync(int id)
        {
            try
            {
                var team = await _teamRepository.GetByIdAsync(id);
                if (team == null)
                {
                    // Throw an exception if the team with the given ID is not found
                    throw new KeyNotFoundException($"Team with ID {id} not found.");
                }
                return _mapper.Map<TeamResponseDto>(team);
            }
            catch (KeyNotFoundException ex)
            {
                // Handle cases where the team is not found
                throw new ApplicationException("Team not found.", ex);
            }
            catch (Exception ex)
            {
                // Handle errors
                throw new ApplicationException("An error occurred while retrieving the team.", ex);
            }
        }

        // Update an team
        public async Task UpdateAsync(TeamUpdateDto teamUpdateRequestDto)
        {
            if (teamUpdateRequestDto == null)
            {
                // Throw an exception if DTO is null
                throw new ArgumentNullException(nameof(teamUpdateRequestDto));
            }

            try
            {
                var team = _mapper.Map<Team>(teamUpdateRequestDto);
                await _teamRepository.UpdateAsync(team);
            }
            catch (Exception ex)
            {
                // Handle errors
                throw new ApplicationException("An error occurred while updating the team.", ex);
            }
        }

        // Delete a team by ID
        public async Task DeleteAsync(int id)
        {
            try
            {
                var team = await _teamRepository.GetByIdAsync(id);
                if (team == null)
                {
                    // Throw an exception if the team with the given ID is not found
                    throw new KeyNotFoundException($"Team with ID {id} not found.");
                }

                await _teamRepository.DeleteAsync(id);
            }
            catch (KeyNotFoundException ex)
            {
                // Handle cases where the team is not found for deletion
                throw new ApplicationException("Team not found for deletion.", ex);
            }
            catch (Exception ex)
            {
                // Handle errors
                throw new ApplicationException("An error occurred while deleting the team.", ex);
            }
        }
    }
}
