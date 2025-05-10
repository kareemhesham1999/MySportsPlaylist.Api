using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using MySportsPlaylist.Api.Hubs;
using MySportsPlaylist.Api.Models;
using MySportsPlaylist.Api.Repositories;

namespace MySportsPlaylist.Api.Services
{
    /// <summary>
    /// Background service that monitors live match statuses.
    /// This service runs on a 5-minute interval, but actual status updates 
    /// are now handled by MatchDemoService. Currently, this service only
    /// logs monitoring information without updating match statuses.
    /// </summary>
    public class LiveMatchStatusService : BackgroundService
    {
        private readonly ILogger<LiveMatchStatusService> _logger;
        private readonly TimeSpan _updateInterval = TimeSpan.FromMinutes(5);

        public LiveMatchStatusService(
            ILogger<LiveMatchStatusService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Live Match Status Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation(
                        "Monitoring matches but not updating statuses (handled by MatchDemoService)");
                    // No status updates performed here (handled by MatchDemoService)
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred in LiveMatchStatusService.");
                }

                await Task.Delay(_updateInterval, stoppingToken);
            }

            _logger.LogInformation("Live Match Status Service is stopping.");
        }
    }
}