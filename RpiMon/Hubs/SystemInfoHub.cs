using Microsoft.AspNetCore.SignalR;
using RpiMon.Models;
using RpiMon.Services;

namespace RpiMon.Hubs;

public class SystemInfoHub : Hub
{
    private readonly ISystemInfoService _systemInfoService;
    private readonly ILogger<SystemInfoHub> _logger;

    public SystemInfoHub(ISystemInfoService systemInfoService, ILogger<SystemInfoHub> logger)
    {
        _systemInfoService = systemInfoService;
        _logger = logger;
    }

    public async Task GetSystemInfo()
    {
        var systemInfo = await _systemInfoService.GetSystemInfoAsync();
        await Clients.Caller.SendAsync("ReceiveSystemInfo", systemInfo);
    }

    public async Task GetStaticSystemInfo()
    {
        var staticInfo = await _systemInfoService.GetStaticSystemInfoAsync();
        await Clients.Caller.SendAsync("ReceiveStaticSystemInfo", staticInfo);
    }

    public async Task GetDynamicSystemInfo()
    {
        var dynamicInfo = await _systemInfoService.GetDynamicSystemInfoAsync();
        await Clients.Caller.SendAsync("ReceiveDynamicSystemInfo", dynamicInfo);
    }

    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client {ConnectionId} connecting...", Context.ConnectionId);
        
        // Send static info immediately on connection
        var staticInfo = await _systemInfoService.GetStaticSystemInfoAsync();
        await Clients.Caller.SendAsync("ReceiveStaticSystemInfo", staticInfo);

        // Send initial dynamic info
        var dynamicInfo = await _systemInfoService.GetDynamicSystemInfoAsync();
        await Clients.Caller.SendAsync("ReceiveDynamicSystemInfo", dynamicInfo);

        await base.OnConnectedAsync();
        _logger.LogInformation("Client {ConnectionId} connected successfully", Context.ConnectionId);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (exception != null)
        {
            _logger.LogError(exception, "Client {ConnectionId} disconnected with error: {Message}", Context.ConnectionId, exception.Message);
        }
        else
        {
            _logger.LogInformation("Client {ConnectionId} disconnected normally", Context.ConnectionId);
        }
        
        await base.OnDisconnectedAsync(exception);
    }
}