using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySportsPlaylist.Api.Models;
using MySportsPlaylist.Api.Repositories;
using System.Security.Claims;

namespace MySportsPlaylist.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PlaylistsController : ControllerBase
    {
        private readonly IPlaylistRepository _playlistRepository;
        private readonly IMatchRepository _matchRepository;

        public PlaylistsController(IPlaylistRepository playlistRepository, IMatchRepository matchRepository)
        {
            _playlistRepository = playlistRepository;
            _matchRepository = matchRepository;
        }

        // GET: api/Playlists
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Match>>> GetUserPlaylist()
        {
            int userId = GetCurrentUserId();
            var matches = await _playlistRepository.GetUserPlaylistMatchesAsync(userId);
            return Ok(matches);
        }

        // POST: api/Playlists/{matchId}
        [HttpPost("{matchId}")]
        public async Task<ActionResult> AddToPlaylist(int matchId)
        {
            int userId = GetCurrentUserId();
            
            // Check if the match exists
            var match = await _matchRepository.GetByIdAsync(matchId);
            if (match == null)
            {
                return NotFound(new { message = "Match not found" });
            }

            // Check if already in playlist
            bool isInPlaylist = await _playlistRepository.IsMatchInUserPlaylistAsync(userId, matchId);
            if (isInPlaylist)
            {
                return BadRequest(new { message = "Match already in playlist" });
            }

            // Add to playlist
            var playlistItem = new Playlist
            {
                UserId = userId,
                MatchId = matchId,
                DateAdded = DateTime.UtcNow
            };

            await _playlistRepository.AddPlaylistItemAsync(playlistItem);
            await _playlistRepository.SaveChangesAsync();

            return Ok(new { message = "Match added to playlist" });
        }

        // DELETE: api/Playlists/{matchId}
        [HttpDelete("{matchId}")]
        public async Task<ActionResult> RemoveFromPlaylist(int matchId)
        {
            int userId = GetCurrentUserId();
            
            // Find the playlist entry
            var playlistItem = await _playlistRepository.GetUserPlaylistItemAsync(userId, matchId);
            if (playlistItem == null)
            {
                return NotFound(new { message = "Match not found in playlist" });
            }

            // Remove from playlist
            await _playlistRepository.RemovePlaylistItemAsync(playlistItem);
            await _playlistRepository.SaveChangesAsync();

            return Ok(new { message = "Match removed from playlist" });
        }

        // GET: api/Playlists/contains/{matchId}
        [HttpGet("contains/{matchId}")]
        public async Task<ActionResult<bool>> CheckIfInPlaylist(int matchId)
        {
            int userId = GetCurrentUserId();
            bool exists = await _playlistRepository.IsMatchInUserPlaylistAsync(userId, matchId);
            return exists;
        }
        
        // Helper method to get the current user ID from the token claims
        private int GetCurrentUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                         User.FindFirst("sub")?.Value;
                         
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int id))
            {
                throw new InvalidOperationException("Unable to determine user ID from token");
            }
            
            return id;
        }
    }
}