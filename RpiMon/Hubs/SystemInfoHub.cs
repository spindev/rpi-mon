using Microsoft.AspNetCore.SignalR;
using RpiMon.Models;
using RpiMon.Services;

namespace RpiMon.Hubs;

public class SystemInfoHub : Hub
{
    private readonly ISystemInfoService _systemInfoService;

    public SystemInfoHub(ISystemInfoService systemInfoService)
    {
        _systemInfoService = systemInfoService;
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
        try
        {
            // Send static info immediately on connection
            var staticInfo = await _systemInfoService.GetStaticSystemInfoAsync();
            await Clients.Caller.SendAsync("ReceiveStaticSystemInfo", staticInfo);

            // Send initial dynamic info
            var dynamicInfo = await _systemInfoService.GetDynamicSystemInfoAsync();
            await Clients.Caller.SendAsync("ReceiveDynamicSystemInfo", dynamicInfo);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in OnConnectedAsync: {ex.Message}");
        }

        await base.OnConnectedAsync();
    }
}