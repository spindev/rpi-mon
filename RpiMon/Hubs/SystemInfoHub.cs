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

    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }
}