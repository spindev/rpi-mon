using Microsoft.Extensions.Logging;
using Moq;
using RpiMon.Models;
using RpiMon.Services;
using System.Runtime.InteropServices;

namespace RpiMon.Tests.Services;

public class SystemInfoServiceTests
{
    private readonly SystemInfoService _systemInfoService;
    private readonly Mock<ILogger<SystemInfoService>> _mockLogger;
    
    public SystemInfoServiceTests()
    {
        _mockLogger = new Mock<ILogger<SystemInfoService>>();
        _systemInfoService = new SystemInfoService(_mockLogger.Object);
    }

    [Fact]
    public async Task GetSystemInfoAsync_ShouldReturnValidSystemInfo()
    {
        // Act
        var result = await _systemInfoService.GetSystemInfoAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.OperatingSystem);
        Assert.NotNull(result.Architecture);
        Assert.NotNull(result.Model);
        Assert.NotNull(result.RamSize);
        Assert.NotNull(result.CpuInfo);
        Assert.True(result.LastUpdated <= DateTime.Now);
        Assert.True(result.LastUpdated > DateTime.Now.AddMinutes(-1));
    }

    [Fact]
    public async Task GetStaticSystemInfoAsync_ShouldReturnValidStaticInfo()
    {
        // Act
        var result = await _systemInfoService.GetStaticSystemInfoAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.OperatingSystem);
        Assert.NotEmpty(result.OperatingSystem);
        Assert.NotNull(result.Architecture);
        Assert.NotEmpty(result.Architecture);
        Assert.NotNull(result.Model);
        Assert.NotEmpty(result.Model);
        Assert.NotNull(result.RamSize);
        Assert.NotEmpty(result.RamSize);
        Assert.NotNull(result.CpuInfo);
        Assert.NotEmpty(result.CpuInfo);
        Assert.True(result.CpuCoreCount > 0);
    }

    [Fact]
    public async Task GetDynamicSystemInfoAsync_ShouldReturnValidDynamicInfo()
    {
        // Act
        var result = await _systemInfoService.GetDynamicSystemInfoAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.CpuTemperature >= 0);
        Assert.True(result.CpuUsage >= 0);
        Assert.True(result.MemoryUsage >= 0);
        Assert.True(result.MemoryTotal >= 0);
        Assert.True(result.MemoryAvailable >= 0);
        Assert.True(result.LastUpdated <= DateTime.Now);
        Assert.True(result.LastUpdated > DateTime.Now.AddMinutes(-1));
    }

    [Fact]
    public async Task GetStaticSystemInfoAsync_ShouldReturnConsistentData()
    {
        // Act
        var result1 = await _systemInfoService.GetStaticSystemInfoAsync();
        var result2 = await _systemInfoService.GetStaticSystemInfoAsync();

        // Assert - Static info should be the same
        Assert.Equal(result1.OperatingSystem, result2.OperatingSystem);
        Assert.Equal(result1.Architecture, result2.Architecture);
        Assert.Equal(result1.Model, result2.Model);
        Assert.Equal(result1.RamSize, result2.RamSize);
        Assert.Equal(result1.CpuInfo, result2.CpuInfo);
        Assert.Equal(result1.CpuCoreCount, result2.CpuCoreCount);
    }

    [Fact]
    public async Task GetDynamicSystemInfoAsync_ShouldHaveReasonableValues()
    {
        // Act
        var result = await _systemInfoService.GetDynamicSystemInfoAsync();

        // Assert
        Assert.True(result.CpuUsage <= 100, "CPU usage should not exceed 100%");
        Assert.True(result.MemoryUsage <= 100, "Memory usage should not exceed 100%");
        Assert.True(result.MemoryTotal > result.MemoryAvailable, "Total memory should be greater than available memory");
        
        // CPU temperature should be reasonable (if available)
        if (result.CpuTemperature > 0)
        {
            Assert.True(result.CpuTemperature > 0 && result.CpuTemperature < 150, 
                "CPU temperature should be between 0 and 150°C");
        }
    }

    [Fact]
    public async Task SystemInfo_ShouldCombineStaticAndDynamicData()
    {
        // Arrange
        var staticInfo = await _systemInfoService.GetStaticSystemInfoAsync();
        var dynamicInfo = await _systemInfoService.GetDynamicSystemInfoAsync();
        
        // Act
        var combinedInfo = await _systemInfoService.GetSystemInfoAsync();

        // Assert
        Assert.Equal(staticInfo.OperatingSystem, combinedInfo.OperatingSystem);
        Assert.Equal(staticInfo.Architecture, combinedInfo.Architecture);
        Assert.Equal(staticInfo.Model, combinedInfo.Model);
        Assert.Equal(staticInfo.RamSize, combinedInfo.RamSize);
        Assert.Equal(staticInfo.CpuInfo, combinedInfo.CpuInfo);
        
        Assert.True(Math.Abs(dynamicInfo.CpuTemperature - combinedInfo.CpuTemperature) < 5, 
            "Combined CPU temperature should be close to dynamic temperature");
        Assert.True(Math.Abs(dynamicInfo.MemoryUsage - combinedInfo.MemoryUsage) < 5, 
            "Combined memory usage should be close to dynamic usage");
    }

    [Fact]
    public void Architecture_ShouldReturnValidValue()
    {
        // Act
        var result = RuntimeInformation.OSArchitecture.ToString();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains(result, new[] { "X64", "X86", "Arm", "Arm64" });
    }

    [Fact]
    public void CpuCoreCount_ShouldBePositive()
    {
        // Act
        var coreCount = Environment.ProcessorCount;

        // Assert
        Assert.True(coreCount > 0, "CPU core count should be positive");
        Assert.True(coreCount <= 128, "CPU core count should be reasonable (≤128)");
    }
}