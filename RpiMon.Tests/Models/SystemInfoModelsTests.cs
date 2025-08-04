using RpiMon.Models;

namespace RpiMon.Tests.Models;

public class StaticSystemInfoTests
{
    [Fact]
    public void StaticSystemInfo_ShouldInitializeWithDefaults()
    {
        // Act
        var staticInfo = new StaticSystemInfo();

        // Assert
        Assert.Equal(string.Empty, staticInfo.OperatingSystem);
        Assert.Equal(string.Empty, staticInfo.Architecture);
        Assert.Equal(string.Empty, staticInfo.Model);
        Assert.Equal(string.Empty, staticInfo.RamSize);
        Assert.Equal(string.Empty, staticInfo.CpuInfo);
        Assert.Equal(0, staticInfo.CpuCoreCount);
    }

    [Fact]
    public void StaticSystemInfo_ShouldSetPropertiesCorrectly()
    {
        // Arrange & Act
        var staticInfo = new StaticSystemInfo
        {
            OperatingSystem = "Raspberry Pi OS",
            Architecture = "Arm64",
            Model = "Raspberry Pi 4 Model B",
            RamSize = "8 GB",
            CpuInfo = "BCM2711 (Cortex-A72)",
            CpuCoreCount = 4
        };

        // Assert
        Assert.Equal("Raspberry Pi OS", staticInfo.OperatingSystem);
        Assert.Equal("Arm64", staticInfo.Architecture);
        Assert.Equal("Raspberry Pi 4 Model B", staticInfo.Model);
        Assert.Equal("8 GB", staticInfo.RamSize);
        Assert.Equal("BCM2711 (Cortex-A72)", staticInfo.CpuInfo);
        Assert.Equal(4, staticInfo.CpuCoreCount);
    }
}

public class DynamicSystemInfoTests
{
    [Fact]
    public void DynamicSystemInfo_ShouldInitializeWithDefaults()
    {
        // Act
        var dynamicInfo = new DynamicSystemInfo();

        // Assert
        Assert.Equal(0.0, dynamicInfo.CpuTemperature);
        Assert.Equal(0.0, dynamicInfo.CpuUsage);
        Assert.Equal(0.0, dynamicInfo.MemoryUsage);
        Assert.Equal(0.0, dynamicInfo.MemoryTotal);
        Assert.Equal(0.0, dynamicInfo.MemoryAvailable);
        Assert.True(dynamicInfo.LastUpdated <= DateTime.Now);
        Assert.True(dynamicInfo.LastUpdated > DateTime.Now.AddSeconds(-1));
    }

    [Fact]
    public void DynamicSystemInfo_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var testTime = DateTime.Now;
        
        // Act
        var dynamicInfo = new DynamicSystemInfo
        {
            CpuTemperature = 55.5,
            CpuUsage = 25.2,
            MemoryUsage = 65.8,
            MemoryTotal = 8192.0,
            MemoryAvailable = 2800.0,
            LastUpdated = testTime
        };

        // Assert
        Assert.Equal(55.5, dynamicInfo.CpuTemperature);
        Assert.Equal(25.2, dynamicInfo.CpuUsage);
        Assert.Equal(65.8, dynamicInfo.MemoryUsage);
        Assert.Equal(8192.0, dynamicInfo.MemoryTotal);
        Assert.Equal(2800.0, dynamicInfo.MemoryAvailable);
        Assert.Equal(testTime, dynamicInfo.LastUpdated);
    }

    [Fact]
    public void DynamicSystemInfo_ShouldHaveReasonableValueRanges()
    {
        // Arrange & Act
        var dynamicInfo = new DynamicSystemInfo
        {
            CpuTemperature = 45.0,
            CpuUsage = 75.0,
            MemoryUsage = 80.0,
            MemoryTotal = 4096.0,
            MemoryAvailable = 1024.0
        };

        // Assert
        Assert.True(dynamicInfo.CpuTemperature >= 0 && dynamicInfo.CpuTemperature <= 100, 
            "CPU temperature should be in reasonable range");
        Assert.True(dynamicInfo.CpuUsage >= 0 && dynamicInfo.CpuUsage <= 100, 
            "CPU usage should be percentage (0-100)");
        Assert.True(dynamicInfo.MemoryUsage >= 0 && dynamicInfo.MemoryUsage <= 100, 
            "Memory usage should be percentage (0-100)");
        Assert.True(dynamicInfo.MemoryTotal > dynamicInfo.MemoryAvailable, 
            "Total memory should be greater than available memory");
    }
}

public class SystemInfoTests
{
    [Fact]
    public void SystemInfo_ShouldInitializeWithDefaults()
    {
        // Act
        var systemInfo = new SystemInfo();

        // Assert
        Assert.Equal(string.Empty, systemInfo.OperatingSystem);
        Assert.Equal(string.Empty, systemInfo.Architecture);
        Assert.Equal(string.Empty, systemInfo.Model);
        Assert.Equal(string.Empty, systemInfo.RamSize);
        Assert.Equal(string.Empty, systemInfo.CpuInfo);
        Assert.Equal(0.0, systemInfo.CpuTemperature);
        Assert.Equal(0.0, systemInfo.CpuUsage);
        Assert.Equal(0.0, systemInfo.MemoryUsage);
        Assert.Equal(0.0, systemInfo.MemoryTotal);
        Assert.Equal(0.0, systemInfo.MemoryAvailable);
        Assert.True(systemInfo.LastUpdated <= DateTime.Now);
        Assert.True(systemInfo.LastUpdated > DateTime.Now.AddSeconds(-1));
    }

    [Fact]
    public void SystemInfo_ShouldCombineStaticAndDynamicProperties()
    {
        // Arrange & Act
        var systemInfo = new SystemInfo
        {
            // Static properties
            OperatingSystem = "Raspberry Pi OS",
            Architecture = "Arm64",
            Model = "Raspberry Pi 4 Model B",
            RamSize = "8 GB",
            CpuInfo = "BCM2711 (Cortex-A72)",
            
            // Dynamic properties
            CpuTemperature = 55.5,
            CpuUsage = 25.2,
            MemoryUsage = 65.8,
            MemoryTotal = 8192.0,
            MemoryAvailable = 2800.0,
            LastUpdated = DateTime.Now
        };

        // Assert - Static properties
        Assert.Equal("Raspberry Pi OS", systemInfo.OperatingSystem);
        Assert.Equal("Arm64", systemInfo.Architecture);
        Assert.Equal("Raspberry Pi 4 Model B", systemInfo.Model);
        Assert.Equal("8 GB", systemInfo.RamSize);
        Assert.Equal("BCM2711 (Cortex-A72)", systemInfo.CpuInfo);
        
        // Assert - Dynamic properties
        Assert.Equal(55.5, systemInfo.CpuTemperature);
        Assert.Equal(25.2, systemInfo.CpuUsage);
        Assert.Equal(65.8, systemInfo.MemoryUsage);
        Assert.Equal(8192.0, systemInfo.MemoryTotal);
        Assert.Equal(2800.0, systemInfo.MemoryAvailable);
    }
}