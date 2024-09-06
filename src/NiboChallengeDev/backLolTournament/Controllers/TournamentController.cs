using LolTournament.Application.DTOs;
using LolTournament.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LolTournament.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentController : ControllerBase
    {
        private readonly TournamentService _tournamentService;

        // Constructor to inject the TournamentService
        public TournamentController(TournamentService tournamentService)
        {
            _tournamentService = tournamentService;
        }

        /// <summary>
        /// Create a new tournament.
        /// </summary>
        /// <param name="tournamentRequest">Details of the tournament to be created.</param>
        /// <returns>The ID of the newly created tournament.</returns>
        /// <response code="201">Returns the ID of the created tournament.</response>
        /// <response code="400">If the tournament data is invalid or created tournament not found.</response>
        /// <response code="409">If there is a conflict, such as a duplicate tournament.</response>
        /// <response code="500">If there is an error processing the request.</response>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TournamentCreateDto tournamentRequest)
        {
            if (tournamentRequest == null)
            {
                // Return a 400 Bad Request if the tournament data is null
                return BadRequest("Tournament data is required.");
            }

            if (tournamentRequest.StartDate < DateOnly.FromDateTime(DateTime.Today))
            {
                return BadRequest("The start date cannot be in the past.");
            }

            try
            {
                // Add the tournament and get its ID
                var tournamentId = await _tournamentService.AddAsync(tournamentRequest);
                // Retrieve the created tournament to confirm it was created
                var createdTournament = await _tournamentService.GetByIdAsync(tournamentId);

                if (createdTournament == null)
                {
                    // Return a 400 Bad Request if the created tournament is not found
                    return BadRequest("Created tournament not found.");
                }

                // Return a 201 Created response with the location of the new tournament
                return CreatedAtAction(nameof(GetById), new { id = createdTournament.Id }, createdTournament);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
            {
                // Return a 409 Conflict if there's a conflict
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Return a 500 Internal Server Error if an unexpected error occurs
                return StatusCode(500, $"An error occurred while creating the tournament: {ex.Message}");
            }
        }

        /// <summary>
        /// Get a winner by ID.
        /// </summary>
        /// <param name="id">ID of the winner to retrieve.</param>
        /// <returns>The details of the tournament's winner.</returns>
        /// <response code="200">Returns the details of the winner.</response>
        /// <response code="404">If the winner with the given ID is not found.</response>
        /// <response code="500">If there is an error retrieving the winner.</response>
        [HttpGet("{id}/winner")]
        public async Task<ActionResult<int?>> GetTournamentWinner(int id)
        {
            try
            {
                var winnerId = await _tournamentService.DetermineWinnerAsync(id);
                if (winnerId.HasValue)
                {
                    return Ok(winnerId.Value);
                }
                return NotFound("No winner has been determined yet.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving the tournament winner: {ex.Message}");
            }
        }

        /// <summary>
        /// Get a tournament by ID.
        /// </summary>
        /// <param name="id">ID of the tournament to retrieve.</param>
        /// <returns>The details of the tournament.</returns>
        /// <response code="200">Returns the details of the tournament.</response>
        /// <response code="404">If the tournament with the given ID is not found.</response>
        /// <response code="500">If there is an error retrieving the tournament.</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<TournamentResponseDto>> GetById(int id)
        {
            try
            {
                // Retrieve the tournament from the service
                var tournament = await _tournamentService.GetByIdAsync(id);
                if (tournament == null)
                {
                    // Return a 404 Not Found if the tournament is not found
                    return NotFound($"Tournament with ID {id} not found.");
                }
                // Return the tournament with a 200 OK response
                return Ok(tournament);
            }
            catch (ApplicationException ex) when (ex.Message.Contains("not found"))
            {
                // Return a 404 Not Found if the exception indicates that the tournament was not found
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                // Return a 500 Internal Server Error if an unexpected error occurs
                return StatusCode(500, "An error occurred while retrieving the tournament.");
            }
        }

        /// <summary>
        /// Get all tournaments.
        /// </summary>
        /// <returns>A list of all tournaments.</returns>
        /// <response code="200">Returns a list of all tournaments.</response>
        /// <response code="500">If there is an error retrieving the tournaments.</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TournamentResponseDto>>> GetAll()
        {
            try
            {
                // Retrieve all tournaments from the service
                var tournaments = await _tournamentService.GetAllAsync();
                // Return the list of tournaments with a 200 OK response
                return Ok(tournaments);
            }
            catch (Exception)
            {
                // Return a 500 Internal Server Error if an unexpected error occurs
                return StatusCode(500, "An error occurred while retrieving tournaments.");
            }
        }

        /// <summary>
        /// Update an existing tournament.
        /// </summary>
        /// <param name="id">ID of the tournament to update.</param>
        /// <param name="tournamentRequest">Updated details of the tournament.</param>
        /// <returns>No content if the update was successful.</returns>
        /// <response code="204">If the update was successful.</response>
        /// <response code="400">If the tournament data is invalid or the ID does not match.</response>
        /// <response code="500">If there is an error updating the tournament.</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TournamentUpdateDto tournamentRequest)
        {
            if (tournamentRequest == null)
            {
                // Return a 400 Bad Request if the tournament data is null
                return BadRequest("Tournament data is required.");
            }
            if (id != tournamentRequest.Id)
            {
                // Return a 400 Bad Request if the ID in URL and body do not match
                return BadRequest("ID in URL and body must match.");
            }

            try
            {
                // Update the tournament in the repository
                await _tournamentService.UpdateAsync(tournamentRequest);
                // Return a 204 No Content response if the update is successful
                return NoContent();
            }
            catch (Exception)
            {
                // Return a 500 Internal Server Error if an unexpected error occurs
                return StatusCode(500, "An error occurred while updating the tournament.");
            }
        }

        /// <summary>
        /// Delete a tournament by ID.
        /// </summary>
        /// <param name="id">ID of the tournament to delete.</param>
        /// <returns>No content if the deletion was successful.</returns>
        /// <response code="204">If the deletion was successful.</response>
        /// <response code="404">If the tournament with the given ID is not found.</response>
        /// <response code="500">If there is an error deleting the tournament.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Delete the tournament from the repository
                await _tournamentService.DeleteAsync(id);
                // Return a 204 No Content response if the deletion is successful
                return NoContent();
            }
            catch (ApplicationException ex) when (ex.Message.Contains("not found"))
            {
                // Return a 404 Not Found if the exception indicates that the tournament was not found
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                // Return a 500 Internal Server Error if an unexpected error occurs
                return StatusCode(500, "An error occurred while deleting the tournament.");
            }
        }
    }
}
