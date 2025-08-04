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

                // Only broadcast dynamic info - static info is sent once on connection
                var dynamicInfo = await systemInfoService.GetDynamicSystemInfoAsync();
                
                // Broadcast to all connected clients
                await hubContext.Clients.All.SendAsync("ReceiveDynamicSystemInfo", dynamicInfo, stoppingToken);
                
                _logger.LogInformation("Dynamic system info broadcasted at {Time}", DateTime.Now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while broadcasting dynamic system info");
            }

            // Wait 5 seconds before next update
            await Task.Delay(5000, stoppingToken);
        }
    }
}