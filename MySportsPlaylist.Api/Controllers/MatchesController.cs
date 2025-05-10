using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySportsPlaylist.Api.Models;
using MySportsPlaylist.Api.Repositories;

namespace MySportsPlaylist.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchesController : ControllerBase
    {
        private readonly IMatchRepository _matchRepository;

        public MatchesController(IMatchRepository matchRepository)
        {
            _matchRepository = matchRepository;
        }

        // GET: api/Matches
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Match>>> GetMatches()
        {
            var matches = await _matchRepository.GetAllAsync();
            return Ok(matches);
        }

        // GET: api/Matches/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Match>> GetMatch(int id)
        {
            var match = await _matchRepository.GetByIdAsync(id);

            if (match == null)
            {
                return NotFound();
            }

            return match;
        }

        // GET: api/Matches/live
        [HttpGet("live")]
        public async Task<ActionResult<IEnumerable<Match>>> GetLiveMatches()
        {
            var liveMatches = await _matchRepository.GetLiveMatchesAsync();
            return Ok(liveMatches);
        }

        // GET: api/Matches/replay
        [HttpGet("replay")]
        public async Task<ActionResult<IEnumerable<Match>>> GetReplayMatches()
        {
            var replayMatches = await _matchRepository.GetReplayMatchesAsync();
            return Ok(replayMatches);
        }

        // GET: api/Matches/search?query=barcelona
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Match>>> SearchMatches([FromQuery] string query)
        {
            var matches = await _matchRepository.SearchMatchesAsync(query);
            return Ok(matches);
        }

        // POST: api/Matches
        // For admin use only
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Match>> PostMatch(Match match)
        {
            await _matchRepository.AddAsync(match);
            await _matchRepository.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMatch), new { id = match.Id }, match);
        }

        // PUT: api/Matches/5
        // For admin use only
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutMatch(int id, Match match)
        {
            if (id != match.Id)
            {
                return BadRequest();
            }

            // Check if match exists
            var existingMatch = await _matchRepository.GetByIdAsync(id);
            if (existingMatch == null)
            {
                return NotFound();
            }

            try
            {
                await _matchRepository.UpdateAsync(match);
                await _matchRepository.SaveChangesAsync();
            }
            catch (Exception)
            {
                if (!await _matchRepository.ExistsAsync(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Matches/5
        // For admin use only
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteMatch(int id)
        {
            var match = await _matchRepository.GetByIdAsync(id);
            if (match == null)
            {
                return NotFound();
            }

            await _matchRepository.DeleteAsync(match);
            await _matchRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}