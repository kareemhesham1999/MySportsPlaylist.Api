using Microsoft.EntityFrameworkCore;
using MySportsPlaylist.Api.Data;
using MySportsPlaylist.Api.Models;

namespace MySportsPlaylist.Api.Repositories
{
    public class MatchRepository : IMatchRepository
    {
        private readonly ApplicationDbContext _context;

        public MatchRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Match>> GetAllAsync()
        {
            return await _context.Matches.ToListAsync();
        }

        public async Task<Match> GetByIdAsync(int id)
        {
            return await _context.Matches.FindAsync(id);
        }

        public async Task<IEnumerable<Match>> GetLiveMatchesAsync()
        {
            return await _context.Matches.Where(m => m.Status == MatchStatus.Live).ToListAsync();
        }

        public async Task<IEnumerable<Match>> GetReplayMatchesAsync()
        {
            return await _context.Matches.Where(m => m.Status == MatchStatus.Replay).ToListAsync();
        }

        public async Task<IEnumerable<Match>> SearchMatchesAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return await GetAllAsync();

            query = query.ToLower();
            
            return await _context.Matches
                .Where(m => m.Title.ToLower().Contains(query) ||
                            m.Competition.ToLower().Contains(query))
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Matches.AnyAsync(m => m.Id == id);
        }

        public async Task AddAsync(Match match)
        {
            await _context.Matches.AddAsync(match);
        }

        public Task UpdateAsync(Match match)
        {
            _context.Entry(match).State = EntityState.Modified;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Match match)
        {
            _context.Matches.Remove(match);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}