using Microsoft.EntityFrameworkCore;
using MySportsPlaylist.Api.Models;
using System.Security.Cryptography;
using System.Text;

namespace MySportsPlaylist.Api.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Create database if it doesn't exist
            context.Database.EnsureCreated();

            // Look for any existing users
            if (context.Users.Any())
            {
                return; // DB has been seeded
            }

            SeedUsers(context);
            SeedMatches(context);

            context.SaveChanges();
        }

        private static void SeedUsers(ApplicationDbContext context)
        {
            var users = new User[]
            {
                new User
                {
                    Username = "admin",
                    Email = "admin@sportsplaylist.com",
                    PasswordHash = HashPassword("Admin123!"),
                    Role = UserRole.Admin
                },
                new User
                {
                    Username = "user1",
                    Email = "user1@example.com",
                    PasswordHash = HashPassword("User123!"),
                    Role = UserRole.User
                }
            };

            context.Users.AddRange(users);
        }

        private static void SeedMatches(ApplicationDbContext context)
        {
            var matches = new Match[]
            {
                new Match
                {
                    Title = "Liverpool vs Manchester United",
                    Competition = "Premier League",
                    Date = DateTime.Now.AddHours(2),
                    Status = MatchStatus.Live,
                    StreamUrl = "https://example.com/stream/live1"
                },
                new Match
                {
                    Title = "Barcelona vs Real Madrid",
                    Competition = "La Liga",
                    Date = DateTime.Now.AddHours(-48),
                    Status = MatchStatus.Replay,
                    StreamUrl = "https://example.com/stream/replay1"
                },
                new Match
                {
                    Title = "Bayern Munich vs Borussia Dortmund",
                    Competition = "Bundesliga",
                    Date = DateTime.Now.AddHours(5),
                    Status = MatchStatus.Live,
                    StreamUrl = "https://example.com/stream/live2"
                },
                new Match
                {
                    Title = "Paris Saint-Germain vs Marseille",
                    Competition = "Ligue 1",
                    Date = DateTime.Now.AddHours(-24),
                    Status = MatchStatus.Replay,
                    StreamUrl = "https://example.com/stream/replay2"
                }
            };

            context.Matches.AddRange(matches);
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}