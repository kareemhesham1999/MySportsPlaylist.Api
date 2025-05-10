using MySportsPlaylist.Api.Models;

namespace MySportsPlaylist.Api.Repositories
{
    public interface IPlaylistRepository
    {
        Task<IEnumerable<Match>> GetUserPlaylistMatchesAsync(int userId);
        Task<bool> IsMatchInUserPlaylistAsync(int userId, int matchId);
        Task<Playlist> GetUserPlaylistItemAsync(int userId, int matchId);
        Task AddPlaylistItemAsync(Playlist playlistItem);
        Task RemovePlaylistItemAsync(Playlist playlistItem);
        Task SaveChangesAsync();
    }
}