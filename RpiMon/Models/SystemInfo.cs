namespace RpiMon.Models;

public class SystemInfo
{
    public string OperatingSystem { get; set; } = string.Empty;
    public string Architecture { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string RamSize { get; set; } = string.Empty;
    public string CpuInfo { get; set; } = string.Empty;
    public double CpuTemperature { get; set; }
    public double CpuUsage { get; set; }
    public double MemoryUsage { get; set; }
    public double MemoryTotal { get; set; }
    public double MemoryAvailable { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.Now;
}