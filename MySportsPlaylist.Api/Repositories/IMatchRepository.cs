using MySportsPlaylist.Api.Models;

namespace MySportsPlaylist.Api.Repositories
{
    public interface IMatchRepository
    {
        Task<IEnumerable<Match>> GetAllAsync();
        Task<Match> GetByIdAsync(int id);
        Task<IEnumerable<Match>> GetLiveMatchesAsync();
        Task<IEnumerable<Match>> GetReplayMatchesAsync();
        Task<IEnumerable<Match>> SearchMatchesAsync(string query);
        Task<bool> ExistsAsync(int id);
        Task AddAsync(Match match);
        Task UpdateAsync(Match match);
        Task DeleteAsync(Match match);
        Task SaveChangesAsync();
    }
}