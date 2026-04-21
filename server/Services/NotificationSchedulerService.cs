using Microsoft.Extensions.Hosting;

namespace SmartPOS.API.Services
{
    public class NotificationSchedulerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NotificationSchedulerService> _logger;

        public NotificationSchedulerService(IServiceProvider serviceProvider, ILogger<NotificationSchedulerService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(15));

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessScheduledCampaignsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed while processing scheduled campaigns");
                }

                await timer.WaitForNextTickAsync(stoppingToken);
            }
        }

        private async Task ProcessScheduledCampaignsAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            var processed = await notificationService.ProcessScheduledCampaignsAsync(cancellationToken);

            if (processed > 0)
            {
                _logger.LogInformation("Processed {Count} scheduled campaign(s)", processed);
            }
        }
    }
}
