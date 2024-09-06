using AutoMapper;
using LolTournament.Application.DTOs;
using LolTournament.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LolTournament.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly MatchService _matchService;
        private readonly IMapper _mapper;

        // Constructor to inject dependencies
        public MatchController(MatchService matchService, IMapper mapper)
        {
            _matchService = matchService;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates a new match.
        /// </summary>
        /// <param name="matchRequest">The match data to create.</param>
        /// <returns>The created match.</returns>
        /// <response code="201">Returns the created match</response>
        /// <response code="400">If the match data is null or invalid</response>
        /// <response code="500">If an error occurs while creating the match</response>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MatchCreateDto matchRequest)
        {
            if (matchRequest == null)
            {
                // Return a 400 Bad Request if the match data is null
                return BadRequest("Match data is required.");
            }

            try
            {
                // Add the match to the repository
                var createdMatch = await _matchService.AddAsync(matchRequest);
                // Return the created match with a 201 Created response
                return CreatedAtAction(nameof(GetById), new { id = createdMatch.Id }, createdMatch);
            }
            catch (Exception)
            {
                // Return a 500 Internal Server Error if an unexpected error occurs
                return StatusCode(500, "An error occurred while creating the match.");
            }
        }

        /// <summary>
        /// Retrieves a match by its ID.
        /// </summary>
        /// <param name="id">The ID of the match.</param>
        /// <returns>The match with the specified ID.</returns>
        /// <response code="200">Returns the match</response>
        /// <response code="404">If the match is not found</response>
        /// <response code="500">If an error occurs while retrieving the match</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<MatchResponseDto>> GetById(int id)
        {
            try
            {
                // Retrieve the match from the service
                var match = await _matchService.GetByIdAsync(id);
                if (match == null)
                {
                    // Return a 404 Not Found if the match is not found
                    return NotFound($"Match with ID {id} not found.");
                }
                // Return the match with a 200 OK response
                return Ok(match);
            }
            catch (ApplicationException ex) when (ex.Message.Contains("not found"))
            {
                // Return a 404 Not Found if the exception indicates that the match was not found
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                // Return a 500 Internal Server Error if an unexpected error occurs
                return StatusCode(500, "An error occurred while retrieving the match.");
            }
        }

        /// <summary>
        /// Retrieves all matches.
        /// </summary>
        /// <returns>A list of all matches.</returns>
        /// <response code="200">Returns the list of matches</response>
        /// <response code="500">If an error occurs while retrieving the matches</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MatchResponseDto>>> GetAll()
        {
            try
            {
                // Retrieve all matches from the service
                var matches = await _matchService.GetAllAsync();
                // Return the list of matches with a 200 OK response
                return Ok(matches);
            }
            catch (Exception)
            {
                // Return a 500 Internal Server Error if an unexpected error occurs
                return StatusCode(500, "An error occurred while retrieving matches.");
            }
        }

        /// <summary>
        /// Updates a match by its ID.
        /// </summary>
        /// <param name="id">The ID of the match to update.</param>
        /// <param name="matchRequest">The updated match data.</param>
        /// <returns>No content if the update is successful.</returns>
        /// <response code="204">If the update is successful</response>
        /// <response code="400">If the match data is null or ID in URL and body do not match</response>
        /// <response code="500">If an error occurs while updating the match</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MatchUpdateDto matchRequest)
        {
            if (matchRequest == null)
            {
                // Return a 400 Bad Request if the match data is null
                return BadRequest("Match data is required.");
            }

            if (id != matchRequest.Id)
            {
                // Return a 400 Bad Request if ID in URL and body do not match
                return BadRequest("ID in URL and body must match.");
            }

            try
            {
                // Update the match in the repository
                await _matchService.UpdateAsync(matchRequest);
                // Return a 204 No Content response if the update is successful
                return NoContent();
            }
            catch (Exception)
            {
                // Return a 500 Internal Server Error if an unexpected error occurs
                return StatusCode(500, "An error occurred while updating the match.");
            }
        }

        /// <summary>
        /// Deletes a match by its ID.
        /// </summary>
        /// <param name="id">The ID of the match to delete.</param>
        /// <returns>No content if the deletion is successful.</returns>
        /// <response code="204">If the deletion is successful</response>
        /// <response code="404">If the match is not found</response>
        /// <response code="500">If an error occurs while deleting the match</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Delete the match from the repository
                await _matchService.DeleteAsync(id);
                // Return a 204 No Content response if the deletion is successful
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                // Return a 404 Not Found if the match to delete is not found
                return NotFound($"Match with ID {id} not found.");
            }
            catch (Exception)
            {
                // Return a 500 Internal Server Error if an unexpected error occurs
                return StatusCode(500, "An error occurred while deleting the match.");
            }
        }
    }
}
