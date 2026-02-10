using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using AMRVI.Data;

namespace AMRVI.Services
{
    public class AndonMonitorService : BackgroundService
    {
        private readonly ILogger<AndonMonitorService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHubContext<AMRVI.Hubs.NotificationHub> _hubContext;
        private DateTime _lastCheckTime = DateTime.MinValue;

        public AndonMonitorService(
            ILogger<AndonMonitorService> logger,
            IServiceProvider serviceProvider,
            IHubContext<AMRVI.Hubs.NotificationHub> hubContext)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Andon Monitor Service started.");

            // Wait 10 seconds before first check to allow app to fully start
            await Task.Delay(10000, stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckForAndonUpdates();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error checking Andon updates");
                }

                // Wait 5 seconds before next check
                await Task.Delay(5000, stoppingToken);
            }

            _logger.LogInformation("Andon Monitor Service stopped.");
        }

        private async Task CheckForAndonUpdates()
        {
            try
            {
                // Create a scope to get scoped services
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ProductionDbContext>();

                // Check production table via Raw SQL for changes
                var latestLogs = await context.ScwLogs
                    .FromSqlRaw("SELECT * FROM [produksi].[tb_elwp_produksi_scw_logs] WHERE [ResolvedAt] IS NULL")
                    .ToListAsync();

                var hasChanges = latestLogs.Any(a => a.CreatedAt > _lastCheckTime);

                if (hasChanges)
                {
                    _logger.LogInformation("Andon data changed, broadcasting update...");
                    
                    // Update last check time
                    _lastCheckTime = DateTime.Now;

                    // Broadcast to all connected clients
                    await _hubContext.Clients.All.SendAsync("AndonDataUpdated", "ALL");
                    
                    _logger.LogInformation("Andon update broadcasted successfully.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CheckForAndonUpdates");
            }
        }
    }
}
