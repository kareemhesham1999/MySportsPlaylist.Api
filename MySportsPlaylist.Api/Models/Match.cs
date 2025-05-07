using System.ComponentModel.DataAnnotations;

namespace MySportsPlaylist.Api.Models
{
    public enum MatchStatus
    {
        Live,
        Replay
    }

    public class Match
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Competition { get; set; }
        
        [Required]
        public DateTime Date { get; set; }
        
        [Required]
        public MatchStatus Status { get; set; }
        
        public string? StreamUrl { get; set; }
        
        public ICollection<Playlist> Playlists { get; set; }
    }
}