using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using RpiMon.Hubs;
using RpiMon.Models;
using RpiMon.Services;

namespace RpiMon.Tests.Hubs;

public class SystemInfoHubTests
{
    private readonly Mock<ISystemInfoService> _mockSystemInfoService;
    private readonly Mock<ILogger<SystemInfoHub>> _mockLogger;
    private readonly Mock<IHubCallerClients> _mockClients;
    private readonly Mock<ISingleClientProxy> _mockClientProxy;
    private readonly Mock<IGroupManager> _mockGroups;
    private readonly Mock<HubCallerContext> _mockContext;
    private readonly SystemInfoHub _hub;

    public SystemInfoHubTests()
    {
        _mockSystemInfoService = new Mock<ISystemInfoService>();
        _mockLogger = new Mock<ILogger<SystemInfoHub>>();
        _mockClients = new Mock<IHubCallerClients>();
        _mockClientProxy = new Mock<ISingleClientProxy>();
        _mockGroups = new Mock<IGroupManager>();
        _mockContext = new Mock<HubCallerContext>();

        _hub = new SystemInfoHub(_mockSystemInfoService.Object, _mockLogger.Object)
        {
            Clients = _mockClients.Object,
            Groups = _mockGroups.Object,
            Context = _mockContext.Object
        };

        _mockClients.Setup(clients => clients.Caller).Returns(_mockClientProxy.Object);
        _mockContext.Setup(context => context.ConnectionId).Returns("test-connection-id");
    }

    [Fact]
    public async Task GetSystemInfo_ShouldCallServiceAndSendToClient()
    {
        // Arrange
        var systemInfo = new SystemInfo
        {
            OperatingSystem = "Test OS",
            Architecture = "x64",
            Model = "Test Model",
            RamSize = "4 GB",
            CpuInfo = "Test CPU",
            CpuTemperature = 45.0,
            CpuUsage = 25.0,
            MemoryUsage = 60.0,
            MemoryTotal = 4096,
            MemoryAvailable = 2048,
            LastUpdated = DateTime.Now
        };

        _mockSystemInfoService.Setup(service => service.GetSystemInfoAsync())
            .ReturnsAsync(systemInfo);

        // Act
        await _hub.GetSystemInfo();

        // Assert
        _mockSystemInfoService.Verify(service => service.GetSystemInfoAsync(), Times.Once);
        _mockClientProxy.Verify(
            client => client.SendCoreAsync("ReceiveSystemInfo", 
                It.Is<object[]>(args => args.Length == 1 && args[0] == systemInfo),
                default),
            Times.Once);
    }

    [Fact]
    public async Task GetStaticSystemInfo_ShouldCallServiceAndSendToClient()
    {
        // Arrange
        var staticInfo = new StaticSystemInfo
        {
            OperatingSystem = "Test OS",
            Architecture = "x64",
            Model = "Test Model",
            RamSize = "4 GB",
            CpuInfo = "Test CPU",
            CpuCoreCount = 4
        };

        _mockSystemInfoService.Setup(service => service.GetStaticSystemInfoAsync())
            .ReturnsAsync(staticInfo);

        // Act
        await _hub.GetStaticSystemInfo();

        // Assert
        _mockSystemInfoService.Verify(service => service.GetStaticSystemInfoAsync(), Times.Once);
        _mockClientProxy.Verify(
            client => client.SendCoreAsync("ReceiveStaticSystemInfo", 
                It.Is<object[]>(args => args.Length == 1 && args[0] == staticInfo),
                default),
            Times.Once);
    }

    [Fact]
    public async Task GetDynamicSystemInfo_ShouldCallServiceAndSendToClient()
    {
        // Arrange
        var dynamicInfo = new DynamicSystemInfo
        {
            CpuTemperature = 45.0,
            CpuUsage = 25.0,
            MemoryUsage = 60.0,
            MemoryTotal = 4096,
            MemoryAvailable = 2048,
            LastUpdated = DateTime.Now
        };

        _mockSystemInfoService.Setup(service => service.GetDynamicSystemInfoAsync())
            .ReturnsAsync(dynamicInfo);

        // Act
        await _hub.GetDynamicSystemInfo();

        // Assert
        _mockSystemInfoService.Verify(service => service.GetDynamicSystemInfoAsync(), Times.Once);
        _mockClientProxy.Verify(
            client => client.SendCoreAsync("ReceiveDynamicSystemInfo", 
                It.Is<object[]>(args => args.Length == 1 && args[0] == dynamicInfo),
                default),
            Times.Once);
    }

    [Fact]
    public async Task JoinGroup_ShouldAddConnectionToGroup()
    {
        // Arrange
        var groupName = "test-group";

        // Act
        await _hub.JoinGroup(groupName);

        // Assert
        _mockGroups.Verify(
            groups => groups.AddToGroupAsync("test-connection-id", groupName, default),
            Times.Once);
    }

    [Fact]
    public async Task LeaveGroup_ShouldRemoveConnectionFromGroup()
    {
        // Arrange
        var groupName = "test-group";

        // Act
        await _hub.LeaveGroup(groupName);

        // Assert
        _mockGroups.Verify(
            groups => groups.RemoveFromGroupAsync("test-connection-id", groupName, default),
            Times.Once);
    }

    [Fact]
    public async Task OnConnectedAsync_ShouldSendBothStaticAndDynamicInfo()
    {
        // Arrange
        var staticInfo = new StaticSystemInfo
        {
            OperatingSystem = "Test OS",
            Architecture = "x64",
            Model = "Test Model",
            RamSize = "4 GB",
            CpuInfo = "Test CPU",
            CpuCoreCount = 4
        };

        var dynamicInfo = new DynamicSystemInfo
        {
            CpuTemperature = 45.0,
            CpuUsage = 25.0,
            MemoryUsage = 60.0,
            MemoryTotal = 4096,
            MemoryAvailable = 2048,
            LastUpdated = DateTime.Now
        };

        _mockSystemInfoService.Setup(service => service.GetStaticSystemInfoAsync())
            .ReturnsAsync(staticInfo);
        _mockSystemInfoService.Setup(service => service.GetDynamicSystemInfoAsync())
            .ReturnsAsync(dynamicInfo);

        // Act
        await _hub.OnConnectedAsync();

        // Assert
        _mockSystemInfoService.Verify(service => service.GetStaticSystemInfoAsync(), Times.Once);
        _mockSystemInfoService.Verify(service => service.GetDynamicSystemInfoAsync(), Times.Once);
        
        _mockClientProxy.Verify(
            client => client.SendCoreAsync("ReceiveStaticSystemInfo", 
                It.Is<object[]>(args => args.Length == 1 && args[0] == staticInfo),
                default),
            Times.Once);
            
        _mockClientProxy.Verify(
            client => client.SendCoreAsync("ReceiveDynamicSystemInfo", 
                It.Is<object[]>(args => args.Length == 1 && args[0] == dynamicInfo),
                default),
            Times.Once);
    }
}