using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MySportsPlaylist.Api.Models
{
    public class Playlist
    {
        [Key] public int Id { get; set; }

        [Required] public int UserId { get; set; }

        [Required] public int MatchId { get; set; }

        public DateTime DateAdded { get; set; }

        [ForeignKey("UserId")] public User User { get; set; }

        [ForeignKey("MatchId")] public Match Match { get; set; }
    }
}