using Microsoft.AspNetCore.SignalR;

namespace MySportsPlaylist.Api.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendPlaylistNotification(string userId, string message, string matchTitle)
        {
            // Send notification to a specific user
            await Clients.User(userId).SendAsync("ReceiveNotification", message, matchTitle);
        }
        
        public async Task SendLiveMatchUpdate(string matchId, string matchTitle, string status)
        {
            // Broadcast match status updates to all connected clients
            await Clients.All.SendAsync("ReceiveLiveMatchUpdate", matchId, matchTitle, status);
        }
    }
}