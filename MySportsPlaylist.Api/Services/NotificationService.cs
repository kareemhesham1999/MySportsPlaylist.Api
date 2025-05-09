using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using MySportsPlaylist.Api.Hubs;
using MySportsPlaylist.Api.Models;

namespace MySportsPlaylist.Api.Services
{
    public interface INotificationService
    {
        Task SendPlaylistNotification(int userId, string action, Match match);
        Task SendMatchStatusNotification(Match match, string oldStatus);
        Task SendToAllUsers(Notification notification);
        Task SendToUser(int userId, Notification notification);
    }

    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            IHubContext<NotificationHub> hubContext,
            ILogger<NotificationService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task SendPlaylistNotification(int userId, string action, Match match)
        {
            Notification notification;
            
            if (action == "added")
            {
                notification = Notification.CreatePlaylistAddedNotification(match);
            }
            else if (action == "removed")
            {
                notification = Notification.CreatePlaylistRemovedNotification(match);
            }
            else
            {
                notification = new Notification
                {
                    Title = "Playlist Update",
                    Message = $"Match {action} playlist",
                    Details = match.Title,
                    Status = match.Status.ToString()
                };
            }
            
            await SendToUser(userId, notification);
        }

        public async Task SendMatchStatusNotification(Match match, string oldStatus)
        {
            var notification = Notification.CreateStatusChangedNotification(match, oldStatus);
            await SendToAllUsers(notification);
        }
        
        public async Task SendToAllUsers(Notification notification)
        {
            try
            {
                await _hubContext.Clients.All.SendAsync("ReceiveNotification", notification);
                _logger.LogInformation("Broadcast notification: {Title} - {Message}", notification.Title, notification.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to broadcast notification");
            }
        }
        
        public async Task SendToUser(int userId, Notification notification)
        {
            try
            {
                await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", notification);
                _logger.LogInformation("Sent notification to user {UserId}: {Title} - {Message}", 
                    userId, notification.Title, notification.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notification to user {UserId}", userId);
            }
        }
    }
}