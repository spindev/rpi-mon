namespace RpiMon.Models;

public class DynamicSystemInfo
{
    public double CpuTemperature { get; set; }
    public double CpuUsage { get; set; }
    public double MemoryUsage { get; set; }
    public double MemoryTotal { get; set; }
    public double MemoryAvailable { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.Now;
}