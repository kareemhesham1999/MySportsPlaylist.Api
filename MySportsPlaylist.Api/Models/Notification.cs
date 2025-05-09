using System;

namespace MySportsPlaylist.Api.Models
{
    public class Notification
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
        public string Status { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        // Create common notification types as factory methods
        public static Notification CreatePlaylistAddedNotification(Match match)
        {
            return new Notification
            {
                Title = "Playlist Update",
                Message = "Match added to playlist",
                Details = match.Title,
                Status = match.Status.ToString()
            };
        }
        
        public static Notification CreatePlaylistRemovedNotification(Match match)
        {
            return new Notification
            {
                Title = "Playlist Update",
                Message = "Match removed from playlist",
                Details = match.Title,
                Status = match.Status.ToString()
            };
        }
        
        public static Notification CreateStatusChangedNotification(Match match, string oldStatus)
        {
            return new Notification
            {
                Title = "Match Status Update",
                Message = match.Title,
                Details = $"Match status changed from {oldStatus} to {match.Status}",
                Status = match.Status.ToString()
            };
        }
    }
}