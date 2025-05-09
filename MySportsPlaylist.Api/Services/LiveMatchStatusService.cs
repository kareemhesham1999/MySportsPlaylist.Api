using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using MySportsPlaylist.Api.Hubs;
using MySportsPlaylist.Api.Models;
using MySportsPlaylist.Api.Repositories;

namespace MySportsPlaylist.Api.Services
{
    public class LiveMatchStatusService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<LiveMatchStatusService> _logger;
        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(15); // Update every 30 seconds
        private readonly TimeSpan _testNotificationInterval = TimeSpan.FromSeconds(15); // Test notification every 5 minutes
        private DateTime _lastTestNotificationTime = DateTime.MinValue;

        public LiveMatchStatusService(
            IServiceProvider serviceProvider,
            IHubContext<NotificationHub> hubContext,
            ILogger<LiveMatchStatusService> logger)
        {
            _serviceProvider = serviceProvider;
            _hubContext = hubContext;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Live Match Status Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Checking match statuses");
                    await CheckAndUpdateMatchStatuses(stoppingToken);

                    // Send test notifications if it's time (For testing purposes)
                    if (DateTime.UtcNow - _lastTestNotificationTime >= _testNotificationInterval)
                    {
                        await SendTestNotification(stoppingToken);
                        _lastTestNotificationTime = DateTime.UtcNow;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while checking match statuses.");
                }
                
                await Task.Delay(_updateInterval, stoppingToken);
            }

            _logger.LogInformation("Live Match Status Service is stopping.");
        }

        private async Task CheckAndUpdateMatchStatuses(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var matchRepository = scope.ServiceProvider.GetRequiredService<IMatchRepository>();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                
                // Get matches that might have status changes
                var matches = await matchRepository.GetAllAsync();
                
                // In a real implementation, you would check external APIs or data sources
                // to determine if any matches have changed status (e.g., from upcoming to live)
                // For this implementation, we'll simulate status changes for matches near their start time
                
                var currentTime = DateTime.UtcNow;
                List<Match> updatedMatches = new List<Match>();
                
                foreach (var match in matches)
                {
                    bool statusChanged = false;
                    MatchStatus oldStatus = match.Status;
                    
                    // Simple logic to update match status based on time
                    // In a real app, this would be based on external data
                    if (match.Date <= currentTime && match.Date.AddMinutes(90) > currentTime && match.Status != MatchStatus.Live)
                    {
                        match.Status = MatchStatus.Live;
                        statusChanged = true;
                    }
                    else if (match.Date.AddMinutes(90) <= currentTime && match.Status == MatchStatus.Live)
                    {
                        match.Status = MatchStatus.Replay;
                        statusChanged = true;
                    }
                    
                    if (statusChanged)
                    {
                        // Update the match in the database
                        await matchRepository.UpdateAsync(match);
                        updatedMatches.Add(match);
                        
                        // Send notification through the notification service
                        await notificationService.SendMatchStatusNotification(match, oldStatus.ToString());
                        
                        _logger.LogInformation("Match {MatchId} status changed from {OldStatus} to {NewStatus}", 
                            match.Id, oldStatus, match.Status);
                    }
                }
                
                if (updatedMatches.Count > 0)
                {
                    await matchRepository.SaveChangesAsync();
                    _logger.LogInformation("Updated {Count} match statuses", updatedMatches.Count);
                }
            }
        }
        private async Task SendTestNotification(CancellationToken stoppingToken)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                    
                    var notification = new Notification
                    {
                        Title = "Test Notification",
                        Message = $"Test Match - {DateTime.UtcNow:HH:mm:ss}",
                        Details = "This is a test notification mocking a live match status update.",
                        Status = "Live"
                    };
                    
                    await notificationService.SendToAllUsers(notification);
                    
                    _logger.LogInformation("Sent test notification at {Time}", 
                        DateTime.UtcNow.ToString("HH:mm:ss"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending test notification");
            }
        }

    }
}