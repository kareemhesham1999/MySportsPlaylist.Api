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
        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(30); // Update every 30 seconds

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
                    await CheckAndUpdateMatchStatuses(stoppingToken);
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
                
                // Get matches that might have status changes
                var matches = await matchRepository.GetAllAsync();
                
                // In a real implementation, you would check external APIs or data sources
                // to determine if any matches have changed status (e.g., from upcoming to live)
                // For this implementation, we'll simulate status changes for matches near their start time
                
                var currentTime = DateTime.UtcNow;
                
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
                        
                        // Notify connected clients about the status change
                        await _hubContext.Clients.All.SendAsync("ReceiveLiveMatchUpdate", 
                            match.Id.ToString(), 
                            match.Title, 
                            match.Status.ToString(), 
                            cancellationToken: stoppingToken);
                        
                        _logger.LogInformation("Match {MatchId} status changed from {OldStatus} to {NewStatus}", 
                            match.Id, oldStatus, match.Status);
                    }
                }
                
                await matchRepository.SaveChangesAsync();
            }
        }
    }
}