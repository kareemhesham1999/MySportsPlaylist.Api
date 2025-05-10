using Microsoft.Extensions.Hosting;
using MySportsPlaylist.Api.Models;
using MySportsPlaylist.Api.Repositories;

namespace MySportsPlaylist.Api.Services
{
    /// <summary>
    /// A background service for demonstration purposes that simulates match status changes.
    /// This service randomly updates match statuses between Live and Replay at regular intervals
    /// and sends notifications about these changes. It is only registered in the development environment.
    /// </summary>
    public class MatchDemoService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MatchDemoService> _logger;
        private readonly TimeSpan _randomUpdateInterval = TimeSpan.FromSeconds(15);
        private DateTime _lastRandomUpdateTime = DateTime.MinValue;
        private readonly Random _random = new Random();

        public MatchDemoService(
            IServiceProvider serviceProvider,
            ILogger<MatchDemoService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Match Demo Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Randomly update match statuses
                    if (DateTime.UtcNow - _lastRandomUpdateTime >= _randomUpdateInterval)
                    {
                        await RandomlyUpdateMatchStatuses(stoppingToken);
                        _lastRandomUpdateTime = DateTime.UtcNow;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred in Match Demo Service.");
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }

            _logger.LogInformation("Match Demo Service is stopping.");
        }

        private async Task RandomlyUpdateMatchStatuses(CancellationToken stoppingToken)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var matchRepository = scope.ServiceProvider.GetRequiredService<IMatchRepository>();
                    var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                    // Get all matches
                    var matches = await matchRepository.GetAllAsync();
                    var matchList = matches.ToList();

                    if (matchList.Count == 0)
                    {
                        _logger.LogInformation("No matches found to randomly update");
                        return;
                    }

                    // Choose a random number of matches to update (1 to 3)
                    int matchesToUpdate = _random.Next(1, Math.Min(4, matchList.Count + 1));
                    _logger.LogInformation("Randomly updating {Count} matches", matchesToUpdate);

                    List<Match> updatedMatches = new List<Match>();

                    for (int i = 0; i < matchesToUpdate; i++)
                    {
                        // Pick a random match
                        int matchIndex = _random.Next(matchList.Count);
                        var match = matchList[matchIndex];

                        // Remove the match from the list to avoid updating it twice
                        matchList.RemoveAt(matchIndex);
                        if (matchList.Count == 0) break;

                        // Save the old status for notification
                        MatchStatus oldStatus = match.Status;

                        // Randomly change status
                        match.Status = match.Status == MatchStatus.Live ? MatchStatus.Replay : MatchStatus.Live;

                        // Update the match in the database
                        await matchRepository.UpdateAsync(match);
                        updatedMatches.Add(match);

                        // Send notification through the notification service
                        // await notificationService.SendMatchStatusNotification(match, oldStatus.ToString());

                        _logger.LogInformation(
                            "Match {MatchId} status randomly changed from {OldStatus} to {NewStatus}",
                            match.Id, oldStatus, match.Status);
                    }

                    if (updatedMatches.Count > 0)
                    {
                        await matchRepository.SaveChangesAsync();
                        _logger.LogInformation("Randomly updated {Count} match statuses", updatedMatches.Count);

                        // Send a summary notification for testing purposes
                        await SendMatchUpdateSummaryNotification(updatedMatches, notificationService);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error randomly updating match statuses");
            }
        }

        private async Task SendMatchUpdateSummaryNotification(List<Match> updatedMatches,
            INotificationService notificationService)
        {
            try
            {
                var summaryMessage =
                    $"Matches updated (Automatic - For Demo): {string.Join(", ", updatedMatches.Select(m => m.Id))}";

                var notification = new Notification
                {
                    Title = "Match Status Update",
                    Message = summaryMessage,
                    Details = "The following matches have been updated: " + string.Join(", ",
                        updatedMatches.Select(m => $"Match ID: {m.Id}, New Status: {m.Status}")),
                    Status = "Updated"
                };

                await notificationService.SendToAllUsers(notification);

                _logger.LogInformation("Sent match update summary notification");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending match update summary notification");
            }
        }
    }
}