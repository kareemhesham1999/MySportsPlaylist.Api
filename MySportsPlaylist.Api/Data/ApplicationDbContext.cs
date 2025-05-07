using Microsoft.EntityFrameworkCore;
using MySportsPlaylist.Api.Models;

namespace MySportsPlaylist.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Playlist> Playlists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure unique constraints
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Define relationship between User and Playlist
            modelBuilder.Entity<Playlist>()
                .HasOne(p => p.User)
                .WithMany(u => u.Playlists)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Define relationship between Match and Playlist
            modelBuilder.Entity<Playlist>()
                .HasOne(p => p.Match)
                .WithMany(m => m.Playlists)
                .HasForeignKey(p => p.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ensure a user can only have a match once in their playlist
            modelBuilder.Entity<Playlist>()
                .HasIndex(p => new { p.UserId, p.MatchId })
                .IsUnique();
        }
    }
}