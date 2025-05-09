using Microsoft.AspNetCore.SignalR;

namespace MySportsPlaylist.Api.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendPlaylistNotification(string userId, string message, string matchTitle)
        {
            // Create a formatted notification object
            var notification = new
            {
                title = "Playlist Update",
                message = message,
                details = matchTitle,
                timestamp = DateTime.UtcNow
            };
            
            // Send notification to a specific user
            await Clients.User(userId).SendAsync("ReceiveNotification", notification);
        }
        
        public async Task SendLiveMatchUpdate(string matchId, string matchTitle, string status)
        {
            // Create a formatted notification object
            var notification = new
            {
                title = "Live Match Update",
                message = matchTitle,
                details = $"Match is now {status}",
                status = status,
                timestamp = DateTime.UtcNow
            };
            
            // Broadcast formatted notification to all connected clients
            await Clients.All.SendAsync("ReceiveNotification", notification);
        }
    }
}