using Microsoft.EntityFrameworkCore;
using MySportsPlaylist.Api.Data;
using MySportsPlaylist.Api.Models;

namespace MySportsPlaylist.Api.Repositories
{
    public class PlaylistRepository : IPlaylistRepository
    {
        private readonly ApplicationDbContext _context;

        public PlaylistRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Match>> GetUserPlaylistMatchesAsync(int userId)
        {
            return await _context.Playlists
                .Where(p => p.UserId == userId)
                .Include(p => p.Match)
                .Select(p => p.Match)
                .ToListAsync();
        }

        public async Task<bool> IsMatchInUserPlaylistAsync(int userId, int matchId)
        {
            return await _context.Playlists
                .AnyAsync(p => p.UserId == userId && p.MatchId == matchId);
        }

        public async Task<Playlist> GetUserPlaylistItemAsync(int userId, int matchId)
        {
            return await _context.Playlists
                .FirstOrDefaultAsync(p => p.UserId == userId && p.MatchId == matchId);
        }

        public async Task AddPlaylistItemAsync(Playlist playlistItem)
        {
            await _context.Playlists.AddAsync(playlistItem);
        }

        public Task RemovePlaylistItemAsync(Playlist playlistItem)
        {
            _context.Playlists.Remove(playlistItem);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}