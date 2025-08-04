using Microsoft.AspNetCore.SignalR;
using RpiMon.Hubs;
using RpiMon.Services;

namespace RpiMon.Services;

public class SystemInfoBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SystemInfoBackgroundService> _logger;

    public SystemInfoBackgroundService(IServiceProvider serviceProvider, ILogger<SystemInfoBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var systemInfoService = scope.ServiceProvider.GetRequiredService<ISystemInfoService>();
                var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<SystemInfoHub>>();

                var systemInfo = await systemInfoService.GetSystemInfoAsync();
                
                // Broadcast to all connected clients
                await hubContext.Clients.All.SendAsync("ReceiveSystemInfo", systemInfo, stoppingToken);
                
                _logger.LogInformation("System info broadcasted at {Time}", DateTime.Now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while broadcasting system info");
            }

            // Wait 5 seconds before next update
            await Task.Delay(5000, stoppingToken);
        }
    }
}