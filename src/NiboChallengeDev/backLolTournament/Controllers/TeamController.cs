using LolTournament.Application.DTOs;
using LolTournament.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LolTournament.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly TeamService _teamService;

        // Constructor to inject dependencies
        public TeamController(TeamService teamService)
        {
            _teamService = teamService;
        }

        /// <summary>
        /// Create a new team.
        /// </summary>
        /// <param name="teamCreateDto">Details of the team to be created.</param>
        /// <returns>The ID of the newly created team.</returns>
        /// <response code="201">Returns the ID of the created team.</response>
        /// <response code="400">If the team data is invalid.</response>
        /// <response code="409">If there is a conflict, such as a duplicate team.</response>
        /// <response code="500">If there is an error processing the request.</response>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TeamCreateDto teamCreateDto)
        {
            if (teamCreateDto == null)
            {
                // Return a 400 Bad Request if the team data is null
                return BadRequest("Invalid team data.");
            }

            try
            {
                var team = await _teamService.AddAsync(teamCreateDto);
                // Return a 201 Created response with the location of the new team
                return CreatedAtAction(nameof(GetById), new { id = team.Id }, team);
            }
            catch (InvalidOperationException ex)
            {
                // Return a 409 Conflict if there's a conflict, e.g., a duplicate team
                return Conflict(new { message = ex.Message });
            }
            catch (Exception)
            {
                // Return a 500 Internal Server Error if an unexpected error occurs
                return StatusCode(500, new { message = "An error occurred while processing your request." });
            }
        }

        /// <summary>
        /// Get a team by ID.
        /// </summary>
        /// <param name="id">ID of the team to retrieve.</param>
        /// <returns>The details of the team.</returns>
        /// <response code="200">Returns the details of the team.</response>
        /// <response code="404">If the team with the given ID is not found.</response>
        /// <response code="500">If there is an error retrieving the team.</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<TeamResponseDto>> GetById(int id)
        {
            try
            {
                // Retrieve the team from the service
                var team = await _teamService.GetByIdAsync(id);
                if (team == null)
                {
                    // Return a 404 Not Found if the team is not found
                    return NotFound($"Team with ID {id} not found.");
                }
                // Return the team with a 200 OK response
                return Ok(team);
            }
            catch (ApplicationException ex) when (ex.Message.Contains("not found"))
            {
                // Return a 404 Not Found if the exception indicates that the team was not found
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                // Return a 500 Internal Server Error if an unexpected error occurs
                return StatusCode(500, "An error occurred while retrieving the team.");
            }
        }

        /// <summary>
        /// Get all teams.
        /// </summary>
        /// <returns>A list of all teams.</returns>
        /// <response code="200">Returns a list of all teams.</response>
        /// <response code="500">If there is an error retrieving the teams.</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamResponseDto>>> GetAll()
        {
            try
            {
                // Retrieve all teams from the service
                var teams = await _teamService.GetAllAsync();
                // Return the list of teams with a 200 OK response
                return Ok(teams);
            }
            catch (Exception)
            {
                // Return a 500 Internal Server Error if an unexpected error occurs
                return StatusCode(500, "An error occurred while retrieving teams.");
            }
        }

        /// <summary>
        /// Update an existing team.
        /// </summary>
        /// <param name="id">ID of the team to update.</param>
        /// <param name="teamUpdateRequestDto">Updated details of the team.</param>
        /// <returns>No content if the update was successful.</returns>
        /// <response code="204">If the update was successful.</response>
        /// <response code="400">If the team data is invalid or the ID does not match.</response>
        /// <response code="500">If there is an error updating the team.</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TeamUpdateDto teamUpdateRequestDto)
        {
            if (teamUpdateRequestDto == null)
            {
                // Return a 400 Bad Request if the team data is null
                return BadRequest("Team data is required.");
            }
            if (id != teamUpdateRequestDto.Id)
            {
                // Return a 400 Bad Request if the ID in URL and body do not match
                return BadRequest("ID in URL and body must match.");
            }

            try
            {
                // Update the team in the repository
                await _teamService.UpdateAsync(teamUpdateRequestDto);
                // Return a 204 No Content response if the update is successful
                return NoContent();
            }
            catch (Exception)
            {
                // Return a 500 Internal Server Error if an unexpected error occurs
                return StatusCode(500, "An error occurred while updating the team.");
            }
        }

        /// <summary>
        /// Delete a team by ID.
        /// </summary>
        /// <param name="id">ID of the team to delete.</param>
        /// <returns>No content if the deletion was successful.</returns>
        /// <response code="204">If the deletion was successful.</response>
        /// <response code="404">If the team with the given ID is not found.</response>
        /// <response code="500">If there is an error deleting the team.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Delete the team from the repository
                await _teamService.DeleteAsync(id);
                // Return a 204 No Content response if the deletion is successful
                return NoContent();
            }
            catch (ApplicationException ex) when (ex.Message.Contains("not found"))
            {
                // Return a 404 Not Found if the exception indicates that the team was not found
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                // Return a 500 Internal Server Error if an unexpected error occurs
                return StatusCode(500, "An error occurred while deleting the team.");
            }
        }
    }
}
